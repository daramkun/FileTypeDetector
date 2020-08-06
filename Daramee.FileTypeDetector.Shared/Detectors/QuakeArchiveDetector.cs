namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	class QuakeArchiveDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] PAK_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x50, 0x41, 0x43, 0x4B } },
		};

		public override string Extension => "pak";

		protected override SignatureInformation [] SignatureInformations => PAK_SignatureInfo;

		public override string ToString () => "Quake™ Package Detector";
	}
}
