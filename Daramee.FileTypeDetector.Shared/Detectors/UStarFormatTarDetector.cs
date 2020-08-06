namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	class UStarFormatTarDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] TAR_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0x101, Signature = new byte [] { 0x75, 0x73, 0x74, 0x61, 0x72, 0x00, 0x30, 0x30 } },
			new SignatureInformation () { Position = 0x101, Signature = new byte [] { 0x75, 0x73, 0x74, 0x61, 0x72, 0x20, 0x20, 0x00 } },
		};

		public override string Extension => "tar";

		protected override SignatureInformation [] SignatureInformations => TAR_SignatureInfo;

		public override string ToString () => "UStar(TAR) Detector";
	}
}
