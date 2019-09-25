using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Audio )]
	class M4ADetector : AbstractISOBaseMediaFileDetailDetector
	{
		public override string Extension => "m4a";

		protected override IEnumerable<string> NextSignature { get { yield return "M4A "; } }

		public override string ToString () => "MP4 Contained Advanced Audio Coding(AAC) Decoder";
	}
}
