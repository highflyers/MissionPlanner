using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebSocketSender
{
    public interface Message
    {
        string JSON();
    }
}
