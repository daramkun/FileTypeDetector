using System;

namespace Daramee.FileTypeDetector
{
	[Flags]
	public enum FormatCategories : uint
	{
		Image = 1,
		Video = 2,
		Audio = 4,
		Archive = 8,
		Compression = 16,
		Document = 32,
		System = 64,
		Executable = 128,

		All = 0xffffffff
	}

	[AttributeUsage ( AttributeTargets.Class, AllowMultiple = true )]
	public class FormatCategoryAttribute : Attribute
	{
		public FormatCategories Category { get; private set; }

		public FormatCategoryAttribute ( FormatCategories category )
		{
			Category = category;
		}
	}
}
