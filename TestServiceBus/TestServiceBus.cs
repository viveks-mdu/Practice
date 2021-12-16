using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TestServiceBus
{
    public class TestServiceBus
    {
        int i = 0;
        private NamespaceManager namespaceManager;
        private IEnumerable<QueueDescription> queues;
        private QueueClient queueReadClient;
        private QueueClient queueWriteClient;

        public static void Main(string[] args)
        {
            Console.WriteLine("Program started ...");

            TestServiceBus tsb = new TestServiceBus();
            tsb.ConnectToServiceBusAsync();

            Console.WriteLine("Program completed.");
        }

        public async Task ConnectToServiceBusAsync()
        {
            string connectionString = "Endpoint=sb://metatestautomation.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=hejZJfk+u7zvfp7tbUnK8sAP5vDgLFYhSiXzydzKI14=";

            // establish connection to namespace
            namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            // get queues
            queues = namespaceManager.GetQueues();            

            await CreateQueueClientAsync(Operation.Read);
            await CreateQueueClientAsync(Operation.Write);

            WriteToQueue();
            //ReadQueue();

            while (true)
            {
                Console.WriteLine($"Choose from below options: \n\n"
                    + $"1. Stop read\n"
                    + $"2. Stop write\n"
                    + $"3. Start read\n"
                    + $"4. Start write\n"
                    + $"other. EXIT\n");
                string input = Console.ReadLine();
            
                switch (input)
                {
                    case "1":
                        CloseQueue(Operation.Read);
                        break;
                    case "2":
                        CloseQueue(Operation.Write);
                        break;
                    case "3":
                        await CreateQueueClientAsync(Operation.Read);
                        break;
                    case "4":
                        await CreateQueueClientAsync(Operation.Write);
                        break;
                    default:
                        CloseQueue(Operation.Read);
                        CloseQueue(Operation.Write);
                        return;
                }
            }
        }

        public async Task CreateQueueClientAsync(Operation o)
        {
            var mfs = new MessagingFactorySettings
            {
                TokenProvider = this.namespaceManager.Settings.TokenProvider,
                NetMessagingTransportSettings = { BatchFlushInterval = TimeSpan.FromSeconds(0.1) }
            };

            MessagingFactory mf = await MessagingFactory.CreateAsync(namespaceManager.Address, mfs);

            foreach (var q in queues)
            {
                if (o == Operation.Read)
                {
                    if (queueReadClient == null || queueReadClient.IsClosed)
                    {
                        var qr = mf.CreateQueueClient(q.Path);
                        queueReadClient = qr;
                        qr.OnMessageAsync(ProcessMessageAsync);
                    }
                }
                else
                {
                    if (queueWriteClient == null || queueWriteClient.IsClosed)
                    {
                        var qw = mf.CreateQueueClient(q.Path);
                        queueWriteClient = qw;
                    }
                }
            }
        }

        public void WriteToQueue()
        {
            Task write = Task.Run(async () => 
            {
                while (true)
                {
                    try
                    {
                        string msgId = GetNextI().ToString();
                        if (!queueWriteClient.IsClosed)
                        {
                            await queueWriteClient.SendAsync(new BrokeredMessage() { MessageId = msgId });
                            Console.WriteLine($"Write: {msgId}");

                            await Task.Delay(TimeSpan.FromSeconds(2));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            });
        }

        private int GetNextI()
        {
            return Interlocked.Increment(ref i);
        }

        public void ReadQueue()
        {
            queueReadClient.OnMessageAsync(ProcessMessageAsync);
        }

        public async Task ProcessMessageAsync(BrokeredMessage message)
        {
            Console.WriteLine($"Read: {message.MessageId}");
            return;
        }

        public void CloseQueue(Operation o)
        {
            var queue = (o == Operation.Read) ? queueReadClient : queueWriteClient;
            
            if (!queue.IsClosed)
            {
                queue.Close();
            }
        }
    }

    public enum Operation
    {
        Read,
        Write
    }
}
