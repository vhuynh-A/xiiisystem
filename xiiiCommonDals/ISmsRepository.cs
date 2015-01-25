using System;
using xiiiCommonModels;

namespace xiiiCommonDals
{
    public interface ISmsRepository
    {
        void Delete(Guid smsGuid);
        SmsMessage Dequeue();
        SmsMessage Insert(SmsMessage message);
        SmsMessage InsertThenQueue(SmsMessage message);
        SmsMessage Queue(SmsMessage message);
        SmsMessage Read(Guid guid);
        SmsMessage Read(string to, Guid guid);
    }
}
