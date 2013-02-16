using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace DerpScrapper
{
    class WorkThreadManager
    {
        private int maxTasks = 4;
        private static WorkThreadManager _instance;

        private Queue<WorkThread> queue;

        private int _currentRunning;
        private int currentRunningTasks
        {
            get
            {
                return _currentRunning;
            }
            set
            {
                _currentRunning = value;
            }
        }

        private long currentIndex = 0;

        protected WorkThreadManager()
        {
            // Singleton
            queue = new Queue<WorkThread>();
            currentRunningTasks = 0;
        }

        public static WorkThreadManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WorkThreadManager();
                }
                return _instance;
            }
        }

        public WorkThreadControl AddNewTask(Func<object, object> task, object argument = null, bool usesDBO = false, Action<object> callback = null)
        {
            WorkThread t = new WorkThread(++currentIndex, task, argument);
            t.callback = callback;
            t.SetDboPointer = (SQLiteConnection) BaseDB.connection.Clone();

            // Loop around if for some reason this application would be running for... a few million years?
            if (currentIndex == long.MaxValue)
            {
                currentIndex = long.MinValue;
            }

            t.ReportProgress += new WorkThread.ReportProgressEventHandler(t_ReportProgress);
            t.WorkDone += new WorkThread.WorkDoneEventHandler(t_WorkDone);

            t.usesDBO = usesDBO;

            if (currentRunningTasks >= maxTasks)
            {
                queue.Enqueue(t);
            }
            else
            {
                t.Start();
                currentRunningTasks++;
            }

            return new WorkThreadControl(t);
        }

        void t_WorkDone(WorkThread sender, object result, bool cancelled, Exception exception)
        {
            currentRunningTasks--;
            if (currentRunningTasks < maxTasks && queue.Count > 0)
            {
                var task = queue.Dequeue();
                task.Start();
                currentRunningTasks++;
            }

            if (sender.callback != null)
            {
                sender.callback(result);
            }
        }

        void t_ReportProgress(WorkThread sender, int pct, object userData)
        {
            Console.WriteLine("A func did report progress");
        }
    }
}
