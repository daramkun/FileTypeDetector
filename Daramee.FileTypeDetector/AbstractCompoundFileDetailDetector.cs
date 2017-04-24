using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMcdf;

namespace Daramee.FileTypeDetector
{
	public abstract class AbstractCompoundFileDetailDetector : IDetector
	{
		public abstract IEnumerable<string> Chunks { get; }

		public abstract string Extension { get; }
		public virtual string Precondition { get { return null; } }

		protected abstract bool IsValidChunk ( string chunkName, byte [] chunkData );

		public bool Detect ( Stream stream )
		{
			try
			{
				CompoundFile cf = new CompoundFile ( stream, CFSUpdateMode.ReadOnly, CFSConfiguration.LeaveOpen );

				foreach ( var chunk in Chunks )
				{
					if ( !IsValidChunk ( chunk, cf.RootStorage.GetStream ( chunk ).GetData () ) )
					{
						cf.Close ();
						return false;
					}
				}

				cf.Close ();

				return true;
			}
			catch { return false; }
		}
	}
}
