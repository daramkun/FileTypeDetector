namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class PFXDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] PFX_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x30, 0x82, 0x06 } },
		};

		public override string Extension => "pfx";

		protected override SignatureInformation [] SignatureInformations => PFX_SignatureInfo;

		public override string ToString () => "Microsoft Personal inFormation eXchange Certificate Detector";
	}
}
