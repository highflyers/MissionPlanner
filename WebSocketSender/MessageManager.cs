using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketSender
{
    public class MessageManager
    {
        private static MessageManager instance;

        public WebSocketSender.MessageSender wsSender;
        Thread senderThread;

        private MessageManager()
        {
            wsSender = new WebSocketSender.MessageSender();
            senderThread = new Thread(new ThreadStart(wsSender.run));
            senderThread.Start();
        }

        public static MessageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageManager();
                }
                return instance;
            }
        }

        public void HandleMessage(WebSocketSender.Message message)
        {
            wsSender.SendMessage(message);
        }
    }
}
