using System.Collections.Generic;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Video )]
	class M4VDetector : AbstractISOBaseMediaFileDetailDetector
	{
		public override string Extension => "m4v";

		protected override IEnumerable<string> NextSignature { get { yield return "mp42"; } }

		public override string ToString () => "MP4 Contained H.264(AVC) Decoder";
	}
}
