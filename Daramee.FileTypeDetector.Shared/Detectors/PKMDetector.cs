using System.Text;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class PKMDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] PKM_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = Encoding.ASCII.GetBytes ( "PKM 10" ) },
		};

		public override string Extension => "pkm";

		protected override SignatureInformation [] SignatureInformations => PKM_SignatureInfo;

		public override string ToString () => "PKM Detector";
	}
}
