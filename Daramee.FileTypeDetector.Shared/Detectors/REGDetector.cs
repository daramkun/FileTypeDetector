using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class REGDetector : AbstractRegexSignatureDetector
	{
		public override string Precondition => "txt";
		public override string Extension => "reg";

		protected override Regex Signature => new Regex ( "^Windows Registry Editor Version [0-9]+.[0-9][0-9]\r?\n" );

		public override string ToString () => "Windows Registry Detector";
	}
}
