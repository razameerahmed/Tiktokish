using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interface
{
    public enum LogType
    {
        System = 2,
        Activity = 3
    }

    public enum ActionType
    {
        View =1,
        Add = 2,
        Edit = 3,
        Delete = 4,
        Undefined = 5,
        Processing = 6,
        UserServiceCall = 7
    }
    internal interface ILogEvent
    {

    }
}
