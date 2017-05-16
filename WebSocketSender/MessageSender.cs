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
        private string address = "", port = "";

        public MessageSender()
        { }
        
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
                Console.Out.WriteLine("WebSocket: running");
                if (webSocket != null)
                {
                    WebSocketState state = webSocket.State;
                    Console.Out.WriteLine("WebSocket: state = " + state);
                    Console.Out.WriteLine("WebSocket: is created");
                    if (state == WebSocketState.Closed || state == WebSocketState.Aborted)
                    {
                        string newAddress, newPort;
                        lock (address)
                        {
                            lock (port)
                            {
                                newAddress = address;
                                newPort = port;
                            }
                        }
                        if (newAddress.Length > 0 && newPort.Length > 0)
                        {
                            Connect(newAddress, newPort);
                        }
                    }
                    lock (Queue)
                    {
                        if (Queue.Count > 0)
                        {
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
                    lock (address)
                    {
                        lock (port)
                        {
                            if (address.Length > 0 && port.Length > 0)
                            {
                                Connect(address, port);
                            }
                        }
                    }
                }
                Thread.Sleep(THREAD_SLEEP_DELAY);
            }
        }

        private async Task Connect(string address, string port)
        {
            try
            {
                string uriString = "ws://" + address + ":" + port.ToString() + "/";
                Console.Out.WriteLine("WebSocket: connecting to " + uriString);
                if (webSocket != null)
                {
                    webSocket.Dispose();
                }
                webSocket = null;
                webSocket = new ClientWebSocket();
                Console.Out.WriteLine("WebSocket: socket created: " + webSocket.ToString());
                Thread.Sleep(THREAD_SLEEP_DELAY);
                await webSocket.ConnectAsync(new System.Uri(uriString), System.Threading.CancellationToken.None);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                webSocket.Dispose();
                webSocket = null;
            }
        }

        public void SetConnectionParameters(string address, string port)
        {
            lock(this.address)
            {
                this.address = address;
            }
            lock(this.port)
            {
                this.port = port;
            }
        }

        public int QueueSize()
        {
            int count = 0;
            lock(Queue)
            {
                count = Queue.Count;
            }
            return count;
        }
    }
}
