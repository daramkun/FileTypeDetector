using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
