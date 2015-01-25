using System;

namespace xiiiCommonModels
{
    public class SmsMessage
    {
        public Guid Guid;
        public string To;
        public string From;
        public string Message;
        public DateTime Created;
    }
}
