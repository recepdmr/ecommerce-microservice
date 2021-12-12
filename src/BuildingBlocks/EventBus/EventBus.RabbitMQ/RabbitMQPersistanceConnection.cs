using System.Net.Sockets;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistanceConnection : IDisposable
    {

        private object LockObject = new object();
        public RabbitMQPersistanceConnection(IConnectionFactory connectionFactory, int reTryCount = 5)
        {
            ConnectionFactory = connectionFactory;
            ReTryCount = reTryCount;
        }

        private IConnection Connection { get; set; }

        public bool IsConnection => Connection != null && Connection.IsOpen;
        private bool IsDisposed { get; set; }

        public IConnectionFactory ConnectionFactory { get; }
        public int ReTryCount { get; }

        public IModel CreateModel()
        {
            return Connection.CreateModel();
        }

        public void Dispose()
        {
            Connection.Dispose();
            IsDisposed = true;
        }

        public bool TryConnect()
        {
            lock (LockObject)
            {
                var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(ReTryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                {

                });

                policy.Execute(() =>
                {
                    Connection = ConnectionFactory.CreateConnection();
                });

                if (IsConnection)
                {
                    Connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    Connection.CallbackException += Connection_CallbackException;
                    Connection.ConnectionBlocked += Connection_ConnectionBlocked;
                    return true;
                }

                return false;
            }
        }

        private void Connection_ConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
        {
            if (!IsDisposed)
            {
                TryConnect();
            }
        }

        private void Connection_CallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            if (!IsDisposed)
            {
                TryConnect();
            }
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            if (!IsDisposed)
            {
                TryConnect();
            }
        }
    }
}