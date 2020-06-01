using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VLCTest.VLC
{
	static class VLCHandler
	{
		public static LibVLC LibVLC => libVLC;

		public static void Init()
		{
			Core.Initialize();
			libVLC = new LibVLC();
			libVLC.SetLogFile("d:/libvlclog.txt");
		}

		static LibVLC libVLC;

	}
}
