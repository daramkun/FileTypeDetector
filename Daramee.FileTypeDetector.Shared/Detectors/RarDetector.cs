namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	[FormatCategory ( FormatCategories.Compression )]
	class RarDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] RAR_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x01, 0x00 } },
		};

		public override string Extension => "rar";

		protected override SignatureInformation [] SignatureInformations => RAR_SignatureInfo;

		public override string ToString () => "RAR Detector";
	}
}
