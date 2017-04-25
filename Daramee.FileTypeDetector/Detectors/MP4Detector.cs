using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class MP4Detector : AbstractISOBaseMediaFileDetailDetector
	{
		public override string Extension => "mp4";

		protected override IEnumerable<string> NextSignature { get { yield return "isom"; } }

		public override string ToString () => "MP4 Detector";
	}
}
