using System.Collections.Generic;
using System.Text;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	class MicrosoftInstallerDetector : AbstractCompoundFileDetailDetector
	{
		public override IEnumerable<string> Chunks { get { yield return "SummaryInformation"; } }

		public override string Extension => "msi";

		protected override bool IsValidChunk ( string chunkName, byte [] chunkData )
		{
			if ( chunkName == "SummaryInformation" )
			{
				if ( Encoding.GetEncoding ( "ascii" ).GetString ( chunkData, 0, chunkData.Length ).IndexOf ( "Installation Database" ) != -1 )
					return true;
			}
			return false;
		}

		public override string ToString () => "Microsoft Installer Setup File Detector";
	}
}
