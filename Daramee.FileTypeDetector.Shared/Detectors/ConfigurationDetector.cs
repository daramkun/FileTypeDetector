using System.IO;
using System.Xml;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class ConfigurationDetector : IDetector
	{
		public string Precondition => "txt";
		public string Extension => "config";

		public bool Detect ( Stream stream )
		{
			try
			{
				System.Xml.XmlReader reader = XmlReader.Create ( stream, new XmlReaderSettings () { } );
				if ( reader.Read () )
				{
					if ( reader.IsStartElement () )
					{
						if ( reader.Name == "configuration" )
						{
							return true;
						}
					}
				}
			}
			catch { }

			return false;
		}

		public override string ToString () => "Microsoft .NET Configuration File Detector";
	}
}
