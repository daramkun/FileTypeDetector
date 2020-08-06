namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	[FormatCategory ( FormatCategories.Video )]
	class WebPDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] WEBP_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x52, 0x49, 0x46, 0x46 } },
			new SignatureInformation () { Position = 8, Signature = new byte [] { 0x57, 0x45, 0x42, 0x50 }, Presignature = new byte [] { 0x52, 0x49, 0x46, 0x46 } },
		};

		public override string Extension => "webp";

		protected override SignatureInformation [] SignatureInformations => WEBP_SignatureInfo;

		public override string ToString () => "WebP Detector";
	}
}
