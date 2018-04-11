using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class PDBDetector : AbstractRegexSignatureDetector
	{
		public override string Extension => "pdb";

		protected override Regex Signature => new Regex ( "^Microsoft C\\/C\\+\\+ MSF [0-9].[0-9][0-9]\r\n" );

		public override string ToString () => "Microsoft Program Database Detector";
	}
}
