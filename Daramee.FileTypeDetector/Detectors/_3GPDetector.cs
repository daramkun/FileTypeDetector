using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class _3GPDetector : AbstractISOBaseMediaFileDetailDetector
	{
		public override string Extension => "3gp";

		protected override IEnumerable<string> NextSignature
		{ get { yield return "3gp6"; yield return "3gp5"; yield return "3gp4"; yield return "3gp3"; yield return "3gp2"; yield return "3gp1"; } }

		public override string ToString () => "3GPP Detector";
	}
}
