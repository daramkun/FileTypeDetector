using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramee.FileTypeDetector.Detectors
{
	[FormatCategory ( FormatCategories.Audio )]
	class MidiDetector : AbstractSignatureDetector
	{
		static SignatureInformation [] MIDI_SignatureInfo = new []
		{
			new SignatureInformation () { Position = 0, Signature = new byte [] { 0x4D, 0x54, 0x68, 0x64 } },
		};

		public override string Extension => "mid";

		protected override SignatureInformation [] SignatureInformations => MIDI_SignatureInfo;

		public override string ToString () => "MIDI Detector";
	}
}
