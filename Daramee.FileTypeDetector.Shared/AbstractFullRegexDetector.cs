using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Daramee.FileTypeDetector
{
    public abstract class AbstractFullRegexDetector : IDetector
    {
		public abstract string Extension { get; }
		public virtual string Precondition { get { return "txt"; } }

		protected abstract Regex Pattern { get; }

		public virtual bool Detect ( Stream stream )
		{
			var encodings = new []
			{
				Encoding.UTF8,
				Encoding.Unicode,
				Encoding.GetEncoding ( "utf-7" ),
				Encoding.GetEncoding ( "utf-32" ),
				Encoding.BigEndianUnicode,
				Encoding.GetEncoding ( "ascii" ),
				Encoding.GetEncoding ( "ks_c_5601-1987" ),
				Encoding.GetEncoding ( "iso-2022-kr" ),
				Encoding.GetEncoding ( "shift_jis" ),
				Encoding.GetEncoding ( "csISO2022JP" ),
				Encoding.GetEncoding ( "windows-1250" ),
				Encoding.GetEncoding ( "windows-1251" ),
				Encoding.GetEncoding ( "windows-1252" ),
				Encoding.GetEncoding ( "windows-1253" ),
				Encoding.GetEncoding ( "windows-1254" ),
			};
			foreach ( var encoding in encodings )
			{
				stream.Position = 0;
				using ( StreamReader reader = new StreamReader ( stream, encoding, true, 4096, true ) )
				{
					string buffer = reader.ReadToEnd ();
					if ( Pattern.Replace ( buffer, "" ) == string.Empty )
						return true;
				}
			}

			return false;
		}
	}
}
