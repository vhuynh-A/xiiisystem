using System;
using System.Collections.Generic;
using xiiiCommonDals;
using xiiiCommonModels;

namespace xiiiWebApi.Tests.Dal
{
    class MockSmsRepository : ISmsRepository
    {
        private Queue<SmsMessage> _queue;
        private Dictionary<Guid, SmsMessage> _table;

        public MockSmsRepository()
        {
            _queue = new Queue<SmsMessage>();
            _table = new Dictionary<Guid, SmsMessage>();
        }

        public void Delete(Guid smsGuid)
        {
            _queue.Dequeue();
        }

        public SmsMessage Dequeue()
        {
            return _queue.Peek();
        }

        public SmsMessage Insert(SmsMessage message)
        {
            if (message.Guid == Guid.Empty)
            {
                message.Guid = Guid.NewGuid();
            }
            _table.Add(message.Guid, message);
            return message;
        }

        public SmsMessage InsertThenQueue(SmsMessage message)
        {
            var insertResult = Insert(message);
            var queueResult = Queue(insertResult);
            return queueResult;
        }

        public SmsMessage Queue(SmsMessage message)
        {
            if (!_table.ContainsKey(message.Guid))
            {
                throw new InvalidOperationException();
            }
            if (!SmsMessage.ReferenceEquals(_table[message.Guid], message))
            {
                throw new ArgumentException();
            }
            _queue.Enqueue(message);
            return message;
        }

        public SmsMessage Read(Guid guid)
        {
            if (!_table.ContainsKey(guid))
            {
                return null;
            }
            return _table[guid];
        }

        public SmsMessage Read(string to, Guid guid)
        {
            if (!_table.ContainsKey(guid))
            {
                return null;
            }
            return _table[guid];
        }

        public int QueueLength
        {
            get { return _queue.Count; }
        }

        public int TableSize
        {
            get { return _table.Count; }
        }
    }
}
