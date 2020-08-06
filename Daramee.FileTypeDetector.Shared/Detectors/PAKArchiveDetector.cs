namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	class PAKArchiveDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] PAK_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x1A, 0x0B } },
		};

		public override string Extension => "pak";

		protected override SignatureInformation [] SignatureInformations => PAK_SignatureInfo;

		public override string ToString () => "PAK Archive Detector";
	}
}
