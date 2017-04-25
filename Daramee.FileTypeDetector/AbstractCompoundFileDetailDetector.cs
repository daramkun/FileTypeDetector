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
			CompoundFile cf = null;

			try
			{
				cf = new CompoundFile ( stream, CFSUpdateMode.ReadOnly, CFSConfiguration.LeaveOpen | CFSConfiguration.Default );

				foreach ( var chunk in Chunks )
				{
					var compoundFileStream = cf.RootStorage.GetStream ( chunk );
					if ( compoundFileStream == null || !IsValidChunk ( chunk, compoundFileStream.GetData () ) )
					{
						//cf.Close ();
						return false;
					}
				}

				cf.Close ();

				return true;
			}
			catch
			{
				//if ( cf != null )
					//cf.Close ();
				return false;
			}
		}
	}
}
