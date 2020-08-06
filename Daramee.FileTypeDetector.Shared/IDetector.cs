using System.IO;

namespace Daramee.FileTypeDetector
{
    public interface IDetector
	{
		string Precondition { get; }
		string Extension { get; }

		bool Detect ( Stream stream );
    }
}
