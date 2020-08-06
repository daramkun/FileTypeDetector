using System.Collections.Generic;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Video )]
	[FormatCategory ( FormatCategories.Audio )]
	class _3GPDetector : AbstractISOBaseMediaFileDetailDetector
	{
		public override string Extension => "3gp";

		protected override IEnumerable<string> NextSignature { get { yield return "3gp"; } }

		public override string ToString () => "3GPP Detector";
	}
}
