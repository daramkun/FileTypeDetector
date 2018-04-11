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

		protected override IEnumerable<string> NextSignature { get { yield return "3gp"; } }

		public override string ToString () => "3GPP Detector";
	}
}
