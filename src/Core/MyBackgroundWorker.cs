using System;
using System.ComponentModel;

namespace iTunesSyncer.Core
{
    public abstract class MyBackgroundWorker : BackgroundWorker
    {
        public class CancelWorkException : Exception
        {
        }

        public MyBackgroundWorker()
        {
            WorkerReportsProgress = true;
            WorkerSupportsCancellation = true;

            DoWork += DoWorkTaskInternal;
        }

        private void DoWorkTaskInternal(object sender, DoWorkEventArgs e)
        {
            try
            {
                var result = DoWorkTask(sender, e);

                e.Result = result;
            }
            catch (CancelWorkException)
            {
                e.Cancel = true;
            }
        }

        protected abstract object? DoWorkTask(object sender, DoWorkEventArgs e);

        protected void ThrowIfCanceled()
        {
            if (CancellationPending)
                throw new CancelWorkException();
        }
    }
}
