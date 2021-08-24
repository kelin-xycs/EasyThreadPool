using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyThreadPool
{
    public class Task
    {
        private Action action;

        public Task(Action action)
        {
            this.action = action;
        }

        internal void Execute()
        {
            this.action();
        }
    }
}
