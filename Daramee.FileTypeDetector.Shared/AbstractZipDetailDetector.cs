using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Daramee.FileTypeDetector
{
	public abstract class AbstractZipDetailDetector : IDetector
	{
		public abstract IEnumerable<string> Files { get; }

		public abstract string Extension { get; }
		public virtual string Precondition { get { return null; } }

		protected virtual bool IsValid ( string filename, ZipArchiveEntry entry ) { return true; }

		public bool Detect ( Stream stream )
		{
			try
			{
				using ( ZipArchive archive = new ZipArchive ( stream, ZipArchiveMode.Read, true ) )
				{
					foreach ( string filename in Files )
					{
						bool succeed = false;
						foreach ( var entry in archive.Entries )
						{
							if ( entry.FullName == filename && IsValid ( filename, entry ) )
							{
								succeed = true;
								break;
							}
						}

						if ( !succeed )
							return false;
					}
				}

				return true;
			}
			catch { return false; }
		}
	}
}
