namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.System )]
	class WindowsMemoryDumpDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] DMP_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x4D, 0x44, 0x4D, 0x50, 0x93, 0xA7 } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x50, 0x41, 0x47, 0x45, 0x44, 0x55 } },
		};

		public override string Extension => "dmp";

		protected override SignatureInformation [] SignatureInformations => DMP_SignatureInfo;

		public override string ToString () => "Windows Memory Dump File Detector";
	}
}
