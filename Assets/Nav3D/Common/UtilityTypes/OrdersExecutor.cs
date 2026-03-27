using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Nav3D.Pathfinding;

namespace Nav3D.Common
{
    public class OrdersExecutor<R, O> : IDisposable where O : IExecutable
    {
        #region Constants

        //ms
        const int TASK_LIFE_TIME = 500;

        #endregion

        #region Attributes

        volatile bool m_Alive;
        int m_MaxAliveOrders;

        volatile Task m_QueueUpdateTask;

        volatile int m_CurrentAliveOrders;

        ConcurrentQueue<R> m_Requesters = new ConcurrentQueue<R>();
        ConcurrentDictionary<R, O> m_Orders = new ConcurrentDictionary<R, O>();

        object m_LockObject = new object();

        #endregion

        #region Properties

        public int MaxAliveOrders => m_MaxAliveOrders;
        public int CurrentAliveOrders => m_CurrentAliveOrders;

        #endregion

        #region Constructors

        public OrdersExecutor(int _MaxAliveOrders)
        {
            m_MaxAliveOrders = _MaxAliveOrders;
            m_Alive = true;
        }

        #endregion

        #region Public methods

        public void EnqueueOrder(R _Requester, O _Order, Log _Log = null)
        {
            _Log?.Write($"{nameof(PathfindingOrder)} instance ({_Order.GetHashCode()})has enqueued");

            lock (m_LockObject)
            {
                if (m_Orders.ContainsKey(_Requester))
                {
                    m_Orders[_Requester] = _Order;
                }
                else
                {
                    m_Requesters.Enqueue(_Requester);
                    m_Orders.TryAdd(_Requester, _Order);
                }
            }

            CheckQueueUpdateTask();
        }

        public bool TryRemoveOrder(R _Requester)
        {
            lock (m_LockObject)
            {
                 return m_Orders.TryRemove(_Requester, out O order);
            }
        }

        void CheckQueueUpdateTask()
        {
            if (m_QueueUpdateTask == null || m_QueueUpdateTask.IsCompleted)
            {
                DateTime taskFireTime = DateTime.Now;

                m_QueueUpdateTask?.Dispose();

                m_QueueUpdateTask = Task.Factory.StartNew(() =>
                    {
                        while (m_Alive && (m_Requesters.Count > 0 || (DateTime.Now - taskFireTime).TotalMilliseconds > TASK_LIFE_TIME))
                        {
                            if (m_Requesters.Count < 0 || m_CurrentAliveOrders >= m_MaxAliveOrders)
                            {
                                Thread.Sleep(100);
                                continue;
                            }

                            UpdateQueue();
                        }
                    },
                    TaskCreationOptions.LongRunning);
            }
        }

        void UpdateQueue()
        {
            //get requester
            if (!m_Requesters.TryPeek(out R requester))
                return;

            lock (m_LockObject)
            {
                //remove order
                if (!m_Orders.TryRemove(requester, out O order))
                {
                    //if there is no order, then remove corresponding requester
                    m_Requesters.TryDequeue(out _);
                    return;
                }

                Interlocked.Increment(ref m_CurrentAliveOrders);
                //execute removed order
                order.Execute(() => Interlocked.Decrement(ref m_CurrentAliveOrders));
            }

            Thread.Sleep(2);

            //if corresponding order is not exist then delete requester
            if (!m_Orders.ContainsKey(requester))
                m_Requesters.TryDequeue(out _);
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            m_Alive = false;
        }

        #endregion
    }
}