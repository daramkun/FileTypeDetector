﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	class PPSXDetector : AbstractZipDetailDetector
	{
		public override IEnumerable<string> Files { get { yield return "[Content_Types].xml"; yield return "_rels/.rels"; yield return "ppt/_rels/presentation.xml.rels"; } }

		public override string Precondition => "zip";
		public override string Extension => "ppsx";

		protected override bool IsValid ( string filename, ZipArchiveEntry entry )
		{
			if ( filename == "[Content_Types].xml" )
			{
				using ( StreamReader reader = new StreamReader ( entry.Open () ) )
				{
					var text = reader.ReadToEnd ();
					if ( text.IndexOf ( "<Override PartName=\"/ppt/presentation.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.presentationml.slideshow.main+xml\"/>" ) >= 0 )
						return true;
					return false;
				}
			}
			return base.IsValid ( filename, entry );
		}

		public override string ToString () => "Microsoft PowerPoint Open XML Document for Slideshow(PPSX) Detector";
	}
}
