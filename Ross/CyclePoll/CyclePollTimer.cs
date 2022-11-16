using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ross.CyclePoll
{
    public class CyclePollTimer : IDisposable
    {
        private Timer _timer;
        private Action callback;

        public CyclePollTimer(int periodInSeconds, Action callBack)
        {
            _timer = new Timer(PollAction, null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(periodInSeconds));
            this.callback = callBack;
        }

        private void PollAction(object state)
        {
            callback();
        }

        public void Dispose()
        {
            if (_timer == null)
                return;

            _timer.Dispose();
            _timer = null;
        }
    }
}
