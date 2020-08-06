using System.Collections.Generic;
using System.IO;
using System.Text;

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
					foreach ( var ns in NextSignature )
					{
						stream.Position = 8;
						string readed = Encoding.GetEncoding ( "ascii" ).GetString ( reader.ReadBytes ( ns.Length ), 0, ns.Length );
						stream.Position = offset;
						if ( ns == readed )
							return true;
					}
				}
			}

			return false;
		}
	}
}
