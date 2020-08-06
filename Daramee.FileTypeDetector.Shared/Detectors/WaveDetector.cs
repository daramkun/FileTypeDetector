namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Audio )]
	class WaveDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] WAV_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x52, 0x49, 0x46, 0x46 } },
			new SignatureInformation () { Position = 8, Signature = new byte [] { 0x57, 0x41, 0x56, 0x45 }, Presignature = new byte [] { 0x52, 0x49, 0x46, 0x46 } },
		};

		public override string Extension => "wav";

		protected override SignatureInformation [] SignatureInformations => WAV_SignatureInfo;

		public override string ToString () => "Wave(WAV) Detector";
	}
}
