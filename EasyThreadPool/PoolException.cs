using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyThreadPool
{
    public class PoolException : Exception
    {
        internal PoolException(string message) : base(message)
        {

        }
    }
}
