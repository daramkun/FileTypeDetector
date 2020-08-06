namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Compression )]
	class ZDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] Z_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x1F, 0x9D } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x1F, 0xA0 } },
		};

		public override string Extension => "z";

		protected override SignatureInformation [] SignatureInformations => Z_SignatureInfo;

		public override string ToString () => "Z Detector";
	}
}
