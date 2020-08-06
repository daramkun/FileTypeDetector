namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class KTXDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] KTX_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xAB, 0x4B, 0x54, 0x58, 0x20, 0x31, 0x31, 0xBB, 0x0D, 0x0A, 0x1A, 0x0A } },
		};

		public override string Extension => "ktx";

		protected override SignatureInformation [] SignatureInformations => KTX_SignatureInfo;

		public override string ToString () => "Khronos Texture Detector";
	}
}
