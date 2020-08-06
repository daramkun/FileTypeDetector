using System.IO;
using System.Text;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class TextDetector : IDetector
	{
		static byte [] SignatureBuffer = new byte [ 4 ];
		static char [] TextBuffer = new char [ 4096 ];
		static byte [] ReadBuffer = new byte [ 4096 ];
		static byte [] EncodingBuffer = new byte [ 4096 ];
		static Encoding [] UTF8Encodings = new [] { Encoding.UTF8 };
		static Encoding [] UTF16Encodings = new [] { Encoding.Unicode };
		static Encoding [] UTF16BEEncodings = new [] { Encoding.BigEndianUnicode };
		static Encoding [] UTF32Encodings = new [] { Encoding.GetEncoding ( "utf-32" ) };
		static Encoding [] UTF7Encodings = new [] { Encoding.GetEncoding ( "utf-7" ) };
		static Encoding [] OtherwiseEncodings = new []
		{
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
			Encoding.GetEncoding ( "ascii" ),
			Encoding.UTF8,
			Encoding.GetEncoding ( "utf-7" ),
			Encoding.GetEncoding ( "utf-32" ),
			Encoding.Unicode,
			Encoding.BigEndianUnicode
		};

		public string Extension => "txt";
		public string Precondition => null;

		public bool Detect ( Stream stream )
		{
			var rawLength = stream.Read ( SignatureBuffer, 0, SignatureBuffer.Length );
			stream.Seek ( 0, SeekOrigin.Begin );

			Encoding [] encodings;

			if ( SignatureBuffer [ 0 ] == 0xef && SignatureBuffer [ 1 ] == 0xbb && SignatureBuffer [ 2 ] == 0xbf )
			{
				encodings = UTF8Encodings;
				stream.Position = 3;
			}
			else if ( SignatureBuffer [ 0 ] == 0xfe && SignatureBuffer [ 1 ] == 0xff )
			{
				encodings = UTF16Encodings;
				stream.Position = 2;
			}
			else if ( SignatureBuffer [ 0 ] == 0xff && SignatureBuffer [ 1 ] == 0xfe )
			{
				encodings = UTF16BEEncodings;
				stream.Position = 2;
			}
			else if ( SignatureBuffer [ 0 ] == 0 && SignatureBuffer [ 1 ] == 0 && SignatureBuffer [ 2 ] == 0xfe && SignatureBuffer [ 3 ] == 0xff )
			{
				encodings = UTF32Encodings;
				stream.Position = 4;
			}
			else if ( SignatureBuffer [ 0 ] == 0x2b && SignatureBuffer [ 1 ] == 0x2f && SignatureBuffer [ 2 ] == 0x76 )
			{
				encodings = UTF7Encodings;
				stream.Position = 3;
			}
			else
			{
				encodings = OtherwiseEncodings;
				stream.Position = 0;
			}

			int readed = stream.Read ( ReadBuffer, 0, /*2048*/1024 );

			foreach ( var encoding in encodings )
			{
				for ( int count = readed; count >= ( readed - 16 ); --count )
				{
					bool succeed = true;

					int texted = encoding.GetChars ( ReadBuffer, 0, count, TextBuffer, 0 );

					for ( int i = 0; i < texted; ++i )
					{
						char ch = TextBuffer [ i ];
						if ( ( char.IsControl ( ch ) && ch != '\r' && ch != '\n' && ch != '\t' ) || ch == '\0' )
						{
							succeed = false;
							break;
						}
					}

					int byted = encoding.GetBytes ( TextBuffer, 0, texted, EncodingBuffer, 0 );

					if ( succeed/* && readed == byted*/ )
					{
						for ( int i = 0; i < count; ++i )
						{
							if ( ReadBuffer [ i ] != EncodingBuffer [ i ] )
							{
								succeed = false;
								break;
							}
						}
					}
					else
						continue;

					if ( succeed )
						return true;
				}
			}

			return false;

		}

		public override string ToString () => "Text File Detector";
	}
}
