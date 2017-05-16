using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketSender
{
    public class MessageSender
    {
        private int THREAD_SLEEP_DELAY = 250;
        private ClientWebSocket webSocket = null;
        private UTF8Encoding encoding = new UTF8Encoding();
        private Queue Queue = new Queue();

        public MessageSender()
        { }

        public MessageSender(string address)
        {
            Connect(address);
        }

        public void SendMessage(Message message)
        {
            lock (Queue.SyncRoot)
            {
                Queue.Enqueue(message);
                Console.Out.WriteLine("WebSocket: inserted message into queue");
            }
        }

        public void run()
        {
            while (true)
            {
                if (webSocket != null)
                {
                    lock (Queue)
                    {
                        if (Queue.Count > 0)
                        {
                            WebSocketState state = webSocket.State;
                            Console.Out.WriteLine(state);
                            if (state == WebSocketState.Closed)
                            {
                                Console.Out.WriteLine("WebSocket: closed");
                                break;
                            }
                            if (state == WebSocketState.Open)
                            {
                                Message Message = (Message)Queue.Dequeue();
                                byte[] array = encoding.GetBytes(Message.JSON());
                                webSocket.SendAsync(new ArraySegment<byte>(array), WebSocketMessageType.Text, true, CancellationToken.None);
                                Console.Out.WriteLine("WebSocket: message sent successfully");
                            }
                        }
                    }
                }
                else
                {
                    Console.Out.WriteLine("WebSocket: not connected, queue size = " + Queue.Count.ToString());
                }
                Thread.Sleep(THREAD_SLEEP_DELAY);
            }
        }

        public async Task Connect(string address)
        {
            try
            {
                string uriString = "ws://" + address + ":1234/";
                webSocket = new ClientWebSocket();
                Console.Out.WriteLine("Socket created");
                await webSocket.ConnectAsync(new System.Uri(uriString), System.Threading.CancellationToken.None);
            }
            catch (Exception)
            {

            }
        }
    }
}
