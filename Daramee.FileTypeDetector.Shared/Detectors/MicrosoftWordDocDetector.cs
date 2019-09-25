﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Document )]
	class MicrosoftWordDocDetector : AbstractCompoundFileDetailDetector
	{
		public override IEnumerable<string> Chunks { get { yield return "WordDocument"; } }

		public override string Extension => "doc";

		protected override bool IsValidChunk ( string chunkName, byte [] chunkData )
		{
			if ( chunkName == "WordDocument" )
				return true;
			return false;
		}

		public override string ToString () => "Microsoft Office Word Document(DOC) Detector";
	}
}
