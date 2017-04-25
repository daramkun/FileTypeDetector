using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector
{
	public abstract class AbstractISOBaseMediaFileDetailDetector : IDetector
	{
		protected abstract IEnumerable<string> NextSignature { get; }

		public abstract string Extension { get; }
		public virtual string Precondition { get { return null; } }

		public virtual bool Detect ( Stream stream )
		{
			using ( BinaryReader reader = new BinaryReader ( stream, Encoding.UTF8, true ) )
			{
				int offset = reader.ReadInt32 ();
				// ftyp
				if ( reader.ReadByte () == 0x66 && reader.ReadByte () == 0x74 && reader.ReadByte () == 0x79 && reader.ReadByte () == 0x70 )
				{
					string readed = Encoding.UTF8.GetString ( reader.ReadBytes ( 4 ), 0, 4 );
					stream.Position = offset;
					foreach ( var ns in NextSignature)
					if ( ns == readed )
						return true;
				}
			}

			return false;
		}
	}
}
