namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class PngDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] PNG_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } },
		};

		public override string Extension => "png";

		protected override SignatureInformation [] SignatureInformations => PNG_SignatureInfo;

		public override string ToString () => "Portable Network Graphics Detector";
	}
}
