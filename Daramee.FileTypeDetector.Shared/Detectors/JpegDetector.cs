namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class JpegDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] JPEG_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xFF, 0xD8, 0xFF, 0xFE, 0x00 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xFF, 0xD8, 0xFF, 0xDB } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xFF, 0xD8, 0xFF, 0xE0 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xFF, 0xD8, 0xFF, 0xE1 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xFF, 0xD8, 0xFF, 0xE2 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xFF, 0xD8, 0xFF, 0xE3 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xFF, 0xD8, 0xFF, 0xE8 } },
		};

		public override string Extension => "jpg";

		protected override SignatureInformation [] SignatureInformations => JPEG_SignatureInfo;

		public override string ToString () => "JPEG Detector";
	}
}
