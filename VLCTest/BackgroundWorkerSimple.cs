using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VLCTest
{
	public class BackgroundWorkerSimple
	{
		public int TickDuration { get; set; } = 30;
		public delegate void DVoid();
		public DVoid DoWorkTick;
		public DVoid GuiUpdate;

		public void Start()
		{

			bw.DoWork += BWDoWork;
			bw.ProgressChanged += BWProgressChanged;
			bw.WorkerReportsProgress = true;
			bw.WorkerSupportsCancellation = true;
			bw.RunWorkerAsync();
		}

		public void Stop()
		{
			if (bw == null)
				return;

			bw.CancelAsync();
			while (true)
			{
				if (!bw.IsBusy)
					break;
				System.Windows.Forms.Application.DoEvents();
				Thread.Sleep(100);
			}
		}

		void BWProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			if (GuiUpdate != null)
				GuiUpdate();
		}

		void BWDoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker bwLocal = (BackgroundWorker)sender;
			while (true)
			{
				if (bwLocal.CancellationPending)
				{
					e.Cancel = true;
					break;
				}
				if (DoWorkTick != null)
					DoWorkTick();

				bwLocal.ReportProgress(0);
				Thread.Sleep(TickDuration);
			}
		}

		public bool shouldFinish = false;
		BackgroundWorker bw = new BackgroundWorker();
	}
}
