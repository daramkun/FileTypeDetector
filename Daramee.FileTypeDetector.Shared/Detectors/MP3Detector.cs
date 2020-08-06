namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Audio )]
	class MP3Detector : AbstractSignatureDetector
	{
		static SignatureInformation [] MP3_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xFF, 0xFB } },
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x49, 0x44, 0x33 } },
		};

		public override string Extension => "mp3";

		protected override SignatureInformation [] SignatureInformations => MP3_SignatureInfo;

		public override string ToString () => "MP3 Detector";
	}
}
