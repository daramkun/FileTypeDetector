namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.System )]
	class WindowsShortcutDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] LNK_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x4C, 0x00, 0x00, 0x00, 0x01, 0x14, 0x02, 0x00 } },
		};

		public override string Extension => "lnk";

		protected override SignatureInformation [] SignatureInformations => LNK_SignatureInfo;

		public override string ToString () => "Windows Shortcut File Detector";
	}
}
