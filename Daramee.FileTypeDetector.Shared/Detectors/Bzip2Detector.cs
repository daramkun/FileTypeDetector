namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Compression )]
	class Bzip2Detector : AbstractSignatureDetector
	{
		static SignatureInformation [] BZ2_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x42, 0x5A, 0x68 } },
		};

		public override string Extension => "bz2";

		protected override SignatureInformation [] SignatureInformations => BZ2_SignatureInfo;

		public override string ToString () => "Bunzip2 Detector";
	}
}
