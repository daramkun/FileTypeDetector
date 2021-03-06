﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Daramee.FileTypeDetector;

namespace FTD.Test
{
	class Program
	{
		static void Main ( string [] args )
		{
			string [] filenames = Directory.GetFiles ( @"..\..\Examples" );
			DetectorService.AddDetectors ( null );

			Stopwatch stopwatch = new Stopwatch ();
			stopwatch.Start ();
			foreach ( var filename in filenames )
			{
				using ( Stream stream = new FileStream ( filename, FileMode.Open, FileAccess.Read ) )
				{
					IDetector detector = DetectorService.DetectDetector ( stream );
					Console.WriteLine ( $"{Path.GetFileName ( filename ).PadRight ( 32 )}: {detector?.Extension.PadRight ( 10 )}({detector})" );
				}
			}
			stopwatch.Stop ();
			Console.WriteLine ( $"Detection Time: {stopwatch.Elapsed}(Average: {TimeSpan.FromMilliseconds ( stopwatch.ElapsedMilliseconds / filenames.Length )})" );
		}
	}
}
