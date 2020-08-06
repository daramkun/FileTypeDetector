namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Executable )]
	class JavaClassDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] CLASS_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0xCA, 0xFE, 0xBA, 0xBE } },
		};

		public override string Extension => "class";

		protected override SignatureInformation [] SignatureInformations => CLASS_SignatureInfo;

		public override string ToString () => "Java Bytecode Detector";
	}
}
