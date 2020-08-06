using System.IO;
using System.Text;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Executable )]
	class EXEDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] EXE_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x4D, 0x5A } },
		};

		public override string Extension => "exe";

		protected override SignatureInformation [] SignatureInformations => EXE_SignatureInfo;

		public override bool Detect ( Stream stream )
		{
			if ( base.Detect ( stream ) )
			{
				stream.Position = 60;
				using ( BinaryReader reader = new BinaryReader ( stream, Encoding.UTF8, true ) )
				{
					stream.Position = reader.ReadInt32 ();
					if ( reader.ReadByte () != 0x50 || reader.ReadByte () != 0x45 || reader.ReadByte () != 0 || reader.ReadByte () != 0 )
						return false;
				}
				return true;
			}
			return false;
		}

		public override string ToString () => "Portable Execution File Format Detector";
	}
}
