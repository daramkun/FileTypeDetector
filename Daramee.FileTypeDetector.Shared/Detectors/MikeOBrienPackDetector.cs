namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	class MikeOBrienPackDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] MPQ_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x4D, 0x50, 0x51, 0x1A } },
		};

		public override string Extension => "mpq";

		protected override SignatureInformation [] SignatureInformations => MPQ_SignatureInfo;

		public override string ToString () => "Mo'PaQ Detector";
	}
}
