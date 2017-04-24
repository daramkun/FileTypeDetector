using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector
{
	public static class DetectorService
	{
		private static List<IDetector> Detectors { get; set; } = new List<IDetector> ();

		public static void AddDetector<T> () where T : IDetector
		{
			var instance = Activator.CreateInstance<T> ();
			AddDetector ( instance );
		}

		public static void AddDetector ( IDetector instance )
		{
			if ( !( Detectors as List<IDetector> ).Contains ( instance ) )
				( Detectors as List<IDetector> ).Add ( instance );
		}

		public static void AddDetectors ( Assembly asm = null )
		{
			if ( asm == null ) asm = Assembly.Load ( new AssemblyName ( "Daramee.FileTypeDetector" ) );
			TypeInfo detectorTypeInfo = typeof ( IDetector ).GetTypeInfo ();
			foreach ( var type in asm.DefinedTypes )
			{
				if ( detectorTypeInfo.IsAssignableFrom ( type ) && !type.IsAbstract && type.DeclaredConstructors.First ().GetParameters ().Length == 0 )
					AddDetector ( Activator.CreateInstance ( type.AsType () ) as IDetector );
			}
		}

		public static IDetector DetectDetector ( Stream stream )
		{
			string pre = null;
			IDetector foundDetector = null;
			while ( true )
			{
				bool found = false;
				foreach ( var detector in from d in Detectors where d.Precondition == pre select d )
				{
					stream.Position = 0;
					if ( detector.Detect ( stream ) )
					{
						found = true;
						foundDetector = detector;
						pre = detector.Extension;
						break;
					}
				}
				if ( !found )
					break;
			}
			return foundDetector;
		}
	}
}
