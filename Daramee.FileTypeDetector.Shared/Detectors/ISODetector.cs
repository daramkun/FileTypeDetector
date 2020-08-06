namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	class ISODetector : AbstractSignatureDetector
	{
		static SignatureInformation [] ISO_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x43, 0x44, 0x30, 0x30, 0x31 } },
		};

		public override string Extension => "iso";

		protected override SignatureInformation [] SignatureInformations => ISO_SignatureInfo;

		public override string ToString () => "ISO-9660 Disc Image Detector";
	}
}
