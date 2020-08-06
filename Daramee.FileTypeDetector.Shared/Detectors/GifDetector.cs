namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Video )]
	[FormatCategory ( FormatCategories.Image )]
	class GifDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] GIF_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 } },
		};

		public override string Extension => "gif";

		protected override SignatureInformation [] SignatureInformations => GIF_SignatureInfo;

		public override string ToString () => "Graphics Interchange Format Detector";
	}
}
