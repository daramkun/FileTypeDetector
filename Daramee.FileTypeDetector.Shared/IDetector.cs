using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramee.FileTypeDetector
{
    public interface IDetector
	{
		string Precondition { get; }
		string Extension { get; }

		bool Detect ( Stream stream );
    }
}
