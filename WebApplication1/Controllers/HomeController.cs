using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebApplication1.Controllers
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {

            return View();
        }

        
        public ActionResult Test()
        {
            EvPublish.Publish();

            object ret = new { Msg = "signal sent", Success = true };
            return Json(ret);
        }

        public async Task<ActionResult> NoticePoll()
        {
            string s = "";
            s += string.Format("\n before[{0}], poll:{1}, bg:{2}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread, Thread.CurrentThread.IsBackground);

            bool signalGot = await EvPublish.Subscribe(10000); //10s
            
            s += string.Format("\n after[{0}], poll:{1}, bg:{2}", Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread.IsThreadPoolThread, Thread.CurrentThread.IsBackground);

            string msg = null;
            if (signalGot)
            {
                msg = "get new msg!!!" + s;
            }
            else
            {
                msg = "time out return!";
            }
                        
            object ret = new { Msg = msg, Success = true };
            return Json(ret);
        }

        public class EvPublish
        {
            public static AutoResetEvent e = new AutoResetEvent(false);
            
            public static Task<bool> Subscribe(int timeoutMill)
            {                
                return Task.Run<bool>(() => { return e.WaitOne(timeoutMill);  });               
            }

            public static void Publish()
            {
                e.Set();
            }
        }

    }
}