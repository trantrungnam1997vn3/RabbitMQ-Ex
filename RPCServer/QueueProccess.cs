using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace RPCServer
{
    public class QueueProccess
    {
        const int ConsumerSize = 5;
        private static int _totalActive = 0;
        public static void ConsumerActive()
        {
            _totalActive++;
        }
        public static void ConsumerDeactive()
        {
            _totalActive--;
        }

        private static ConcurrentQueue<Action> _lstQueue = new ConcurrentQueue<Action>();
        public static void Add(Action act)
        {
            lock (_lstQueue)
            {
                _lstQueue.Enqueue(act);
            }
        }

        private static void Init()
        {
            if (_totalActive < ConsumerSize)
            {
                Action action = () =>
                {
                    try
                    {
                        QueueProccess.ConsumerActive();
                        while (true)
                        {
                            Action item = null;
                            while (_lstQueue.TryDequeue(out item))
                            {
                                if (item != null)
                                    item();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        QueueProccess.ConsumerDeactive();
                    }
                };
                Action[] pars = new Action[ConsumerSize - _totalActive];
                for (int i = 0; i < ConsumerSize - _totalActive; i++)
                {
                    pars[i] = action;
                }
                Task.Run(() =>
                {
                    Parallel.Invoke(pars);
                });
            }
        }

        public static void Sync(params Action[] pars)
        {
            if (_totalActive < ConsumerSize) Init();

            if (pars != null)
            {
                var lstTask = new List<Task>();
                foreach (var item in pars)
                {
                    var tsk = Task.Run(() =>
                    {
                        bool running = true;
                        QueueProccess.Add(() =>
                        {
                            try
                            {
                                item();
                            }
                            catch (Exception ex)
                            {

                            }
                            running = false;
                        });
                        while (running)
                        { }
                    });
                    lstTask.Add(tsk);
                }
                Task.WaitAll(lstTask.ToArray());
            }
        }

        public static void Async(params Action[] pars)
        {
            if (_totalActive < ConsumerSize) Init();

            if (pars != null)
            {
                foreach (var item in pars)
                {
                    var tsk = Task.Run(() =>
                    {
                        QueueProccess.Add(() =>
                        {
                            try
                            {
                                item();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                        });
                    });
                }
            }
        }
    }
}
