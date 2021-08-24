using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyThreadPool
{
    enum PoolStatus
    {
        End,
        First,
        Running,
        //Edle,
        Empty,
        FirstFromEmpty
    }
}
