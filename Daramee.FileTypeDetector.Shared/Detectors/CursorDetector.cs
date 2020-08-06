namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Image )]
	class CursorDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] CUR_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x00, 0x00, 0x02, 0x00 } },
		};

		public override string Extension => "cur";

		protected override SignatureInformation [] SignatureInformations => CUR_SignatureInfo;

		public override string ToString () => "Cursor Detector";
	}
}
