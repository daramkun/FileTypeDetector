using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Daramee.FileTypeDetector
{
	[StructLayout ( LayoutKind.Sequential )]
	public struct SignatureInformation
	{
		public int Position;
		public byte [] Signature;
		public byte [] Presignature;
	}

	public abstract class AbstractSignatureDetector : IDetector
	{
		public abstract string Extension { get; }
		public virtual string Precondition { get { return null; } }

		protected abstract SignatureInformation [] SignatureInformations { get; }

		private int cachedLongestLength = -1;

		public virtual bool Detect ( Stream stream )
		{
			if ( cachedLongestLength == -1 )
				foreach ( var sig in SignatureInformations )
					cachedLongestLength = ( cachedLongestLength < sig.Signature.Length ) ? sig.Signature.Length : cachedLongestLength;

			byte [] buffer = new byte [ cachedLongestLength ];

			byte [] preSignature = null;
			bool correct = false;
			while ( true )
			{
				bool found = false;
				foreach ( var siginfo in
					from si
					in SignatureInformations
					where CompareArray ( si.Presignature, preSignature )
					orderby si.Position
					select si )
				{
					correct = false;
					stream.Position = siginfo.Position;
					stream.Read ( buffer, 0, cachedLongestLength );
					if ( CompareArray ( siginfo.Signature, buffer ) )
					{
						preSignature = siginfo.Signature;
						correct = true;
						found = true;
						break;
					}
				}
				if ( !found )
					break;
			}

			return correct;
		}

		private bool CompareArray ( byte [] a1, byte [] a2 )
		{
			if ( a1 == null && a2 == null ) return true;
			if ( a1 == null || a2 == null ) return false;

			bool failed = false;
			int min = ( ( a1.Length > a2.Length ) ? a2.Length : a1.Length );
			for ( int i = 0; i < min; ++i )
			{
				if ( a1 [ i ] != a2 [ i ] )
				{
					failed = true;
					break;
				}
			}
			return !failed;
		}
	}
}
