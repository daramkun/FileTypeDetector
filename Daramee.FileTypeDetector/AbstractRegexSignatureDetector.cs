using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector
{
	public abstract class AbstractRegexSignatureDetector : IDetector
	{
		public abstract string Extension { get; }
		public virtual string Precondition { get { return null; } }

		protected abstract Regex Signature { get; }

		public virtual bool Detect ( Stream stream )
		{
			int readBufferSize = Signature.ToString ().Length * 4;

			var encodings = new [] { Encoding.UTF8, Encoding.Unicode, Encoding.BigEndianUnicode };
			foreach ( var encoding in encodings )
			{
				stream.Position = 0;
				using ( StreamReader reader = new StreamReader ( stream, encoding, true, readBufferSize, true ) )
				{
					char [] buffer = new char [ readBufferSize ];
					reader.ReadBlock ( buffer, 0, readBufferSize );
					if ( Signature.IsMatch ( new string ( buffer ) ) )
						return true;
				}
			}

			return false;
		}
	}
}
