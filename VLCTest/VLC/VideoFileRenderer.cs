using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VLCTest.VLC
{
	unsafe class VideoFileRenderer
	{
		public BitmapSource CurrenBitmap => bsg.RenderBitmapSource();

		public bool Open(string filename)
		{
			mediaPlayer = new MediaPlayer(VLCHandler.LibVLC);

			//videoView.MediaPlayer = mediaPlayer;
			media = new Media(VLCHandler.LibVLC, filename, FromType.FromPath,
				"input-repeat=1000000");
			media.Parse().Wait();

			UInt32 width = 0;
			UInt32 height = 0;
			UInt32 pitch = 0;
			foreach (var track in media.Tracks)
			{
				if (track.TrackType != TrackType.Video)
					continue;

				width = track.Data.Video.Width;
				height = track.Data.Video.Height;

				//TODO: align to 4 bytes
				pitch = width * 4;

				originalWidth = (int)width;
				originalHeight = (int)height;
				originalStride = (int)pitch;

				bsg.Init(originalWidth, originalHeight);


				memory = Marshal.AllocHGlobal((int)(height * pitch));
			}

			mediaPlayer.SetVideoFormat("RV32", width, height, pitch);
			mediaPlayer.SetVideoCallbacks(LockCB, UnlockCb, DisplayCb);
			//mediaPlayer.EnableHardwareDecoding = true;
			mediaPlayer.PositionChanged += OnPositionChanged;
			mediaPlayer.SeekableChanged += OnSeekableChanged;
			mediaPlayer.EndReached += OnEndReached;

			mediaPlayer.Pause();
			mediaPlayer.Position = -0.05f;

			Trace.WriteLine("Opened: " + filename);
			//Thread.Sleep(1000);

			return true;
		}

		private void OnEndReached(object sender, EventArgs e)
		{
			Trace.WriteLine("OnEndReached");
			ThreadPool.QueueUserWorkItem(_ =>
			{
				mediaPlayer.Stop();
				var ret = mediaPlayer.Play();
			});
		}


		public void PauseAt(float perc)
		{
			Trace.WriteLine("Seek: " + perc.ToString());
			mediaPlayer.Position = perc;
			if (mediaPlayer.IsPlaying)
				mediaPlayer.SetPause(true);
			//Thread.Sleep(100);
		}

		private void OnSeekableChanged(object sender, MediaPlayerSeekableChangedEventArgs e)
		{
			Trace.WriteLine("OnSeekableChanged:" + e.Seekable.ToString());
		}

		private void OnPositionChanged(object sender, 
			MediaPlayerPositionChangedEventArgs e)
		{
			//Trace.WriteLine("New pos:" + e.Position.ToString());
		}

		public bool Play()
		{
			if (media == null)
				return false;

			mediaPlayer.SetRate(1.0f);
			if (!mediaPlayer.Play(media))
				return false;

			mediaPlayer.Position = 0.5f;
			Trace.WriteLine("Play triggered");
			return true;
		}

		public IntPtr LockCB(IntPtr opaque, IntPtr planes)
		{
			//Trace.Write("L");
			Marshal.WriteIntPtr(planes, memory);
			return IntPtr.Zero;
		}

		public void UnlockCb(IntPtr opaque, IntPtr picture, IntPtr planes)
		{
			//Trace.Write("U");
		}

		public void DisplayCb(IntPtr opaque, IntPtr picture)
		{
			//Trace.Write("D");
			byte* pMem = (byte*)memory.ToPointer();

			for (int y = 0; y < bsg.Height; y++)
			{
				int fromY = originalHeight * y / bsg.Height;
				for (int x = 0; x < bsg.Width; x++)
				{
					int fromX = originalWidth * x / bsg.Width;
					int originalOffset = fromY * originalStride + 4 * fromX;
					int offset = y * bsg.RawStride + 4 * x;
					bsg.RawImage[offset + 0] = pMem[originalOffset + 0];
					bsg.RawImage[offset + 1] = pMem[originalOffset + 1];
					bsg.RawImage[offset + 2] = pMem[originalOffset + 2];
					bsg.RawImage[offset + 3] = pMem[originalOffset + 3];
				}
			}

			//currenBitmap = bsg.RenderBitmapSource();
		}

		Media media;
		MediaPlayer mediaPlayer;
		IntPtr memory;
		int originalWidth = 0;
		int originalHeight = 0;
		int originalStride = 0;
		BitmapSourceGenerator bsg = new BitmapSourceGenerator();
		//BitmapSource currenBitmap;

	}
}
