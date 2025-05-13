using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hybriona
{
    internal class HttpServerMainThreadDispatcher : HybSingleton<HttpServerMainThreadDispatcher>
    {

        private static Queue<HttpServerMainThreadAction> actions = new Queue<HttpServerMainThreadAction>();
        private static object thelock = new object();
        public void Init() { }

        internal static Task Dispatch(System.Action action)
        {
            lock (thelock)
            {
                var tcs = new TaskCompletionSource<bool>();
                actions.Enqueue(new HttpServerMainThreadAction(action, tcs));
                return tcs.Task;
            }
        }

        private void Update()
        {
            while (actions.Count > 0)
            {
                lock (thelock)
                {
                    var actionGroup = actions.Dequeue();
                    try
                    {
                        actionGroup.action();
                        actionGroup.taskCompletionSource.SetResult(true);
                    }
                    catch(System.Exception ex)
                    {
                        actionGroup.taskCompletionSource.SetException(ex);
                    }
                    
                }
            }
        }

    }

    internal struct HttpServerMainThreadAction
    {
        public System.Action action;
      
        public TaskCompletionSource<bool> taskCompletionSource;
        public HttpServerMainThreadAction(System.Action action,TaskCompletionSource<bool> taskCompletionSource)
        {
            this.action = action;
          
            this.taskCompletionSource = taskCompletionSource;
        }


    }

}