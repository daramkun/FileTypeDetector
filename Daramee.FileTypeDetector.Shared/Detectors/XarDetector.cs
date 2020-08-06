namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Compression )]
	class XarDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] XAR_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x78, 0x61, 0x72, 0x21 } },
		};

		public override string Extension => "xar";

		protected override SignatureInformation [] SignatureInformations => XAR_SignatureInfo;

		public override string ToString () => "XAR Detector";
	}
}
