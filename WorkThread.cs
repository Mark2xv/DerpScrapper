using System;
using System.ComponentModel;
using System.Data.SQLite;

namespace DerpScrapper
{
    class WorkThread
    {
        private BackgroundWorker workingThread;

        public delegate void ReportProgressEventHandler(WorkThread sender, int pct, object userData);
        public event ReportProgressEventHandler ReportProgress;

        public delegate void WorkDoneEventHandler(WorkThread sender, object result, bool cancelled, Exception exception);
        public event WorkDoneEventHandler WorkDone;

        public bool usesDBO = false;

        Func<object, object> task;
        object argument;

        public Action<object> callback;

        public long identifier;

        public SQLiteConnection SetDboPointer;

        public WorkThread(long identifier, Func<object, object> func, object argument = null, Action<object> callback = null)
        {
            workingThread = new BackgroundWorker();
            workingThread.WorkerSupportsCancellation = true;
            workingThread.WorkerReportsProgress = true;

            workingThread.DoWork += new DoWorkEventHandler(workingThread_DoWork);
            workingThread.ProgressChanged += new ProgressChangedEventHandler(workingThread_ProgressChanged);
            workingThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workingThread_RunWorkerCompleted);

            this.task = func;
            this.argument = argument;

            this.identifier = identifier;
        }

        void workingThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.WorkDone != null)
                this.WorkDone(this, e.Result, e.Cancelled, e.Error); // TODO fix
            this.Done();
        }

        void workingThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.ReportProgress != null)
                this.ReportProgress(this, e.ProgressPercentage, e.UserState); // TODO fix
        }

        void workingThread_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.task != null)
            {
                if (this.usesDBO && this.SetDboPointer != null)
                {
                    // Overwrite the BaseDB.connection. It's a [ThreadStatic] variable, so nothing on the mainthread (or other threads, for that matter) will be changed
                    BaseDB.connection = this.SetDboPointer;
                }
                e.Result = task(argument);
            }
            // why did you create this workthread without a task
        }

        public void Start()
        {
            if(!this.workingThread.IsBusy)
                this.workingThread.RunWorkerAsync();
        }

        public void Cancel()
        {
            this.workingThread.CancelAsync();
        }

        private void Done()
        {
            workingThread.Dispose();
            workingThread = null;
        }
    }

    class WorkThreadControl
    {
        public WorkThreadControl(WorkThread work)
        {
            this.work = work;
        }

        private WorkThread work;
        public void Cancel()
        {
            work.Cancel();
        }
    }
}
