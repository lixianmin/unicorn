/********************************************************************
created:    2024-03-08
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Unicorn.Collections
{
    public class ThreadSwapper<T>
    {
        public ThreadSwapper(Slice<T> producer = null, Slice<T> consumer = null)
        {
            _producer = producer ?? new Slice<T>();
            _consumer = consumer ?? new Slice<T>();
        }
        
        /// <summary>
        /// 因为2个线程的执行速度不一样, 由producer调用Clear()的好处是:
        /// 1. 不会因为producer线程调用速度过快, 导致_sharedList无限变长
        /// 2. consumer线程每次都能读到有数据的_sharedList, 而if由consumer调用Clear(), 则可能在下一帧读不到有数据的_sharedList
        /// </summary>
        /// <param name="clearShared"></param>
        public void Put(bool clearShared)
        {
            lock (_lock)
            {
                if (clearShared)
                {
                    _sharedList.Clear();
                }

                _sharedList.AddRange(_producer);
            }
        }

        public void Take(bool clearShared)
        {
            lock (_lock)
            {
                _consumer.AddRange(_sharedList);

                if (clearShared)
                {
                    _sharedList.Clear();
                }
            }
        }

        public Slice<T> GetProducer()
        {
            return _producer;
        }

        public Slice<T> GetConsumer()
        {
            return _consumer;
        }

        private readonly Slice<T> _producer;
        private readonly Slice<T> _consumer;

        private readonly Slice<T> _sharedList = new();
        private readonly object _lock = new();
    }
}