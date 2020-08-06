namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	[FormatCategory ( FormatCategories.Compression )]
	class _7zDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] _7Z_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x37, 0x7A, 0xBC, 0xAF, 0x27, 0x1C } },
		};

		public override string Extension => "7z";

		protected override SignatureInformation [] SignatureInformations => _7Z_SignatureInfo;

		public override string ToString () => "7Z Detector";
	}
}
