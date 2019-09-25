using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class InitializationDetector : AbstractRegexSignatureDetector
	{
		public override string Precondition => "txt";
		public override string Extension => "ini";

		protected override Regex Signature => new Regex ( "^(\\[(.*)\\]\r?\n((((.*)=(.*))*|(;(.*)))\r?\n)*)+" );

		public override string ToString () => "Initialization File Detector";
	}
}
