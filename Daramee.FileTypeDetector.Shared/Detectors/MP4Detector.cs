using System.Collections.Generic;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Video )]
	[FormatCategory ( FormatCategories.Audio )]
	[FormatCategory ( FormatCategories.Image )]
	class MP4Detector : AbstractISOBaseMediaFileDetailDetector
	{
		public override string Extension => "mp4";

		protected override IEnumerable<string> NextSignature { get { yield return "isom"; } }

		public override string ToString () => "MP4 Detector";
	}
}
