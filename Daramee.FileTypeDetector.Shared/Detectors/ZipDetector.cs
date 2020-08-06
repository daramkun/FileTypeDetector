namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Compression )]
	[FormatCategory ( FormatCategories.Archive )]
	class ZipDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] ZIP_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x50, 0x4b, 0x03, 0x04 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x50, 0x4b, 0x05, 0x06 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x50, 0x4b, 0x07, 0x08 } },
		};

		public override string Extension => "zip";

		protected override SignatureInformation [] SignatureInformations => ZIP_SignatureInfo;

		public override string ToString () => "ZIP Detector";
	}
}
