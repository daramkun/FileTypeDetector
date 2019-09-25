﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class VisualStudioSolutionDetector : AbstractRegexSignatureDetector
	{
		public override string Precondition => "txt";
		public override string Extension => "sln";

		protected override Regex Signature => new Regex ( "\r\nMicrosoft Visual Studio Solution File, Format Version [0-9]+.[0-9]+\r\n" );

		public override string ToString () => "Microsoft Visual Studio Solution Detector";
	}
}
