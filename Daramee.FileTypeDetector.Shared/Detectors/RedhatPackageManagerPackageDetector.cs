namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	class RedhatPackageManagerPackageDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] RPM_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xED, 0xAB, 0xEE, 0xDB } },
		};

		public override string Extension => "rpm";

		protected override SignatureInformation [] SignatureInformations => RPM_SignatureInfo;

		public override string ToString () => "RedHat Package Manager Package File Detector";
	}
}
