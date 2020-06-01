using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VLCTest.VLC;

namespace VLCTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			VideoFileRenderer vfr;
			vfr = new VideoFileRenderer();
			string dir = Directory.GetCurrentDirectory();
			string filename = dir + @"\..\..\..\SampleFile\Tunel.mov";
			if (vfr.Open(filename))
				videos.Add(vfr);
			
			var dirFiles = Directory.GetFiles(@"d:\LED Video FullHD\");
			List<string> files = new List<string>();
			files.AddRange(dirFiles);
			files.Sort();
			//int startFrom = 0;
			int index = 0;
			//to read whole directory:
			//foreach (var filename in files)
			//{
			//	if (index < startFrom)
			//	{
			//		index++;
			//		continue;
			//	}
			//	if (index - startFrom >= 1)
			//		break;
				
			//	vfr = new VideoFileRenderer();
			//	if (vfr.Open(filename))
			//		videos.Add(vfr);
			//	index++;
			//}

			index = 0;

			foreach (var video in videos)
			{
				if (index > 10)
					break;

				video.Play();

				var ni = new Image();
				images.Add(ni);
				spImages.Children.Add(ni);

				index++;
			}
			
			bws.GuiUpdate = GuiUpdate;
			bws.Start();
		}

		private void VsPreview_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			videos.ForEach(v => v.PauseAt((float)vsPreview.Value / 100.0f));
		}

		private void BPlay_Click(object sender, RoutedEventArgs e)
		{
			videos.ForEach(v => v.Play());
		}

		void GuiUpdate()
		{
			for (int i = 0; i < videos.Count && i < images.Count; i++)
			{
				var src = videos[i].CurrenBitmap;
				if (src == null)
					continue;

				images[i].Source = videos[i].CurrenBitmap;
			}

			//videos.ForEach(v => v.Seek());
		}

		BackgroundWorkerSimple bws = new BackgroundWorkerSimple();
		List<VideoFileRenderer> videos = new List<VideoFileRenderer>();
		List<Image> images = new List<Image>();


	}
}
