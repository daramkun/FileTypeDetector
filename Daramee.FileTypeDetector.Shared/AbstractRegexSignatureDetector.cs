using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Daramee.FileTypeDetector
{
	public abstract class AbstractRegexSignatureDetector : IDetector
	{
		public abstract string Extension { get; }
		public virtual string Precondition { get { return null; } }

		protected abstract Regex Signature { get; }

		public virtual bool Detect ( Stream stream )
		{
			int readBufferSize = Signature.ToString ().Length * 8;
			char [] buffer = new char [ readBufferSize ];

			var encodings = new []
			{
				Encoding.GetEncoding ( "ascii" ),
#if !NETFX_CORE
				Encoding.GetEncoding ( "ks_c_5601-1987" ),
				Encoding.GetEncoding ( "iso-2022-kr" ),
				Encoding.GetEncoding ( "shift_jis" ),
				Encoding.GetEncoding ( "csISO2022JP" ),
				Encoding.GetEncoding ( "windows-1250" ),
				Encoding.GetEncoding ( "windows-1251" ),
				Encoding.GetEncoding ( "windows-1252" ),
				Encoding.GetEncoding ( "windows-1253" ),
				Encoding.GetEncoding ( "windows-1254" ),
#endif
				Encoding.UTF8,
				Encoding.GetEncoding ( "utf-7" ),
				Encoding.GetEncoding ( "utf-32" ),
				Encoding.Unicode,
				Encoding.BigEndianUnicode
			};
			foreach ( var encoding in encodings )
			{
				stream.Position = 0;
				using ( StreamReader reader = new StreamReader ( stream, encoding, true, readBufferSize, true ) )
				{
					reader.ReadBlock ( buffer, 0, readBufferSize );
					if ( Signature.IsMatch ( new string ( buffer ) ) )
						return true;
				}
			}

			return false;
		}
	}
}
