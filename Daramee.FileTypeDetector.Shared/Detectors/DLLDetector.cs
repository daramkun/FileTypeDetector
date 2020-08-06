using System.IO;
using System.Text;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.System )]
	[FormatCategory ( FormatCategories.Executable )]
	class DLLDetector : IDetector
	{
		public string Precondition => "exe";
		public string Extension => "dll";

		public bool Detect ( Stream stream )
		{
			stream.Position = 60;
			using ( BinaryReader reader = new BinaryReader ( stream, Encoding.UTF8, true ) )
			{
				stream.Position = reader.ReadInt32 () + 4 + 18;
				short characteristics = reader.ReadInt16 ();
				if ( ( characteristics & 0x2000 ) == 0 )
					return false;
			}

			return true;
		}

		public override string ToString () => "Windows Dynamic Linkage Library Detector";
	}
}
