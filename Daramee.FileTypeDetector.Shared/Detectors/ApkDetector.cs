using System.Collections.Generic;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Archive )]
	[FormatCategory ( FormatCategories.Compression )]
	[FormatCategory ( FormatCategories.Executable )]
	class ApkDetector : AbstractZipDetailDetector
	{
		public override IEnumerable<string> Files { get { yield return "classes.dex"; yield return "AndroidManifest.xml"; } }

		public override string Precondition => "zip";
		public override string Extension => "apk";

		public override string ToString () => "Android Package(APK) Detector";
	}
}
