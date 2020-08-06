namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Video )]
	[FormatCategory ( FormatCategories.Audio )]
	class AudioVideoInterleaveDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] AVI_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x52, 0x49, 0x46, 0x46 } },
			new SignatureInformation () { Position = 8, Signature = new byte [] { 0x41, 0x56, 0x49, 0x20 }, Presignature = new byte [] { 0x52, 0x49, 0x46, 0x46 } },
		};

		public override string Extension => "avi";

		protected override SignatureInformation [] SignatureInformations => AVI_SignatureInfo;

		public override string ToString () => "Audio Video Interleave(AVI) Detector";
	}
}
