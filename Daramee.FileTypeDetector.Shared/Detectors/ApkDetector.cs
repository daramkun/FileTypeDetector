using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class ApkDetector : AbstractZipDetailDetector
	{
		public override IEnumerable<string> Files { get { yield return "classes.dex"; yield return "AndroidManifest.xml"; } }

		public override string Precondition => "zip";
		public override string Extension => "apk";

		public override string ToString () => "Android Package(APK) Detector";
	}
}
