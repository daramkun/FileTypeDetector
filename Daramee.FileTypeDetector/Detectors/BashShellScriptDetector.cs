using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class BashShellScriptDetector : AbstractRegexSignatureDetector
	{
		public override string Precondition => "txt";
		public override string Extension => "sh";

		protected override Regex Signature => new Regex ( "^#!\\/(.+)\n" );

		public override string ToString () => "Bash Shell Script Detector";
	}
}
