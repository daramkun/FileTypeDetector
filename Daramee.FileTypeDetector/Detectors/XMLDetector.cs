using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class XMLDetector : AbstractRegexSignatureDetector
	{
		public override string Precondition => "txt";
		public override string Extension => "xml";

		protected override Regex Signature => new Regex (
			"^<\\?xml[ \t\n\r]+version=\"[0-9]+\\.[0-9]+\"[ \t\n\r]+([a-zA-Z0-9]+=\"[a-zA-Z0-9\\-_]+\"[ \t\n\r]*)*[ \t\n\r]+\\?>"
		);

		public override string ToString () => "eXtensible Markup Language Document Detector";
	}
}
