namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class Jpeg2000Detector : AbstractSignatureDetector
	{
		static SignatureInformation [] JPEG_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x00, 0x00, 0x00, 0x0C, 0x6A, 0x50, 0x20, 0x20 } },
		};

		public override string Extension => "jp2";

		protected override SignatureInformation [] SignatureInformations => JPEG_SignatureInfo;

		public override string ToString () => "JPEG2000 Detector";
	}
}
