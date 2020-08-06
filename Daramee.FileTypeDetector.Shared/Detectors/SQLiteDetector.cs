namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class SQLiteDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] SQLITE_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x53, 0x51, 0x4C, 0x69, 0x74, 0x65, 0x20, 0x66, 0x6F, 0x72, 0x6D, 0x61, 0x74, 0x20, 0x33, 0x00 } },
		};

		public override string Extension => "sqlite";

		protected override SignatureInformation [] SignatureInformations => SQLITE_SignatureInfo;

		public override string ToString () => "SQLite Detector";
	}
}
