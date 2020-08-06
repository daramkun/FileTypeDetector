using System.Text;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class CertDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] CRT_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = Encoding.GetEncoding ( "ascii" ).GetBytes ( "-----BEGIN CERTIFICATE-----" ) },
		};

		public override string Precondition => "txt";
		public override string Extension => "crt";

		protected override SignatureInformation [] SignatureInformations => CRT_SignatureInfo;

		public override string ToString () => "Certificate Detector";
	}
}
