using System;
using System.Collections.Generic;
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
			string [] filenames = new string [] {
				@"E:\Multimedia\Images\XWaIsbDCta.png",
				@"E:\Development\예제\Unity\Unity_Intro_01_Introduction.rar",
				@"D:\OneDrive\문서\교육\GROW Game Team DirectX Team 시험 - 1.docx",
				@"E:\Multimedia\Images\tumblr_mhpgrnduqE1s0rlcfo1_500.gif",
				@"E:\Development\백업\1. VisualStudio\Developing\Projects\GitHub\DaramRenamer\Daramkun.DaramRenamer\Resources\ProgramIcon.ico",
				@"E:\Multimedia\Images\RE7_Q3d_F.jpg",
				@"E:\Development\백업\6. Web\sh.apk",
				@"E:\Development\백업\6. Web\cert\daram_pe_kr.crt",
				@"D:\OneDrive\문서\교육\객체 지향 프로그래밍.docx",
				@"D:\OneDrive\문서\교육\DirectX 11 PPT\DirectX 11 - 16 - Deferred Lighting.pptx",
				@"D:\OneDrive\문서\시간표.xlsx",
				@"E:\Multimedia\E-books\unmaintainablecode.epub",
				@"E:\Multimedia\Games\DJMAX Trilogy Resolution.reg",
				@".\FTD.Test.exe",
				@"..\..\..\Daramee.FileTypeDetector.sln",
				@"C:\Windows\System32\kernel32.dll",
				@"E:\Utilities\Drivers\ke2200_2400_Inf\Windows10-x64\ke2200_2400w10\e2xw10x64.sys",
				@"E:\Multimedia\E-books\게임독립만세.pdf",
				@"E:\User Resources\test.bmp",
				@"E:\Multimedia\Animations\ALDNOAH. ZERO\ALDNOAH. ZERO - 01.mp4",
				@"E:\Development\백업\1. VisualStudio\Developing\Projects\GitHub\Misty\Daramkun.Misty.Core\Resources\Spirit\SpriteEffect.xml",
				@"E:\User Resources\새 폴더\데이터베이스연구실자료\02_데이터베이스연구실관련\기타제출파일\잔류자신고서 양식@2013.hwp"
			};
			DetectorService.AddDetectors ( null );

			foreach ( var filename in filenames )
			{
				using ( Stream stream = new FileStream ( filename, FileMode.Open, FileAccess.Read ) )
				{
					IDetector detector = DetectorService.DetectDetector ( stream );
					Console.WriteLine ( $"{Path.GetFileName ( filename ).PadRight ( 54 )}: {detector?.Extension}" );
				}
			}
		}
	}
}
