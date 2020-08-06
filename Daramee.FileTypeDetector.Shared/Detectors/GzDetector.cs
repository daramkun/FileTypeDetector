namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Compression )]
	class GzDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] GZ_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x1F, 0x8B } },
		};

		public override string Extension => "gz";

		protected override SignatureInformation [] SignatureInformations => GZ_SignatureInfo;

		public override string ToString () => "GZ Detector";
	}
}
