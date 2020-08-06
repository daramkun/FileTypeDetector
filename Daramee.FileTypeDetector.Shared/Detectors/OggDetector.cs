namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Audio )]
	[FormatCategory ( FormatCategories.Video )]
	class OggDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] OGG_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x4F, 0x67, 0x67, 0x53 } },
		};

		public override string Extension => "ogg";

		protected override SignatureInformation [] SignatureInformations => OGG_SignatureInfo;

		public override string ToString () => "OGG Detector";
	}
}
