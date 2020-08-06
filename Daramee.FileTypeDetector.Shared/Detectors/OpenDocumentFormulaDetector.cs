using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class OpenDocumentFormulaDetector : AbstractZipDetailDetector
	{
		public override IEnumerable<string> Files { get { yield return "mimetype"; } }

		public override string Precondition => "zip";
		public override string Extension => "odf";

		protected override bool IsValid ( string filename, ZipArchiveEntry entry )
		{
			if ( filename == "mimetype" )
			{
				using ( Stream mimetypeStream = entry.Open () )
				{
					byte [] buffer = new byte [ "application/vnd.oasis.opendocument.formula".Length ];
					if ( mimetypeStream.Read ( buffer, 0, buffer.Length ) != buffer.Length )
						return false;
					if ( Encoding.GetEncoding ( "ascii" ).GetString ( buffer, 0, buffer.Length ) != "application/vnd.oasis.opendocument.formula" )
						return false;
				}
			}
			return base.IsValid ( filename, entry );
		}

		public override string ToString () => "OpenDocument Formula File Detector";
	}
}
