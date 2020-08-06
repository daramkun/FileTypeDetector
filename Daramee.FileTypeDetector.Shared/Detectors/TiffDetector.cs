namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class TiffDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] TIFF_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x49, 0x49, 0x49 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x49, 0x49, 0x2A, 0x00 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x4D, 0x4D, 0x00, 0x2A } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x4D, 0x4D, 0x00, 0x2B } },
		};

		public override string Extension => "tif";

		protected override SignatureInformation [] SignatureInformations => TIFF_SignatureInfo;

		public override string ToString () => "Tagged Image File Format Detector";
	}
}
