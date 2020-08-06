namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class JpegXRDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] JPEG_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x49, 0x49, 0xBC, 0x01 } },
		};

		public override string Extension => "hdp";

		protected override SignatureInformation [] SignatureInformations => JPEG_SignatureInfo;

		public override string ToString () => "HD Photo(JPEG XR) Detector";
	}
}
