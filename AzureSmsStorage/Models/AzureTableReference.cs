using System;

namespace AzureSmsStorage.Models
{
    internal class AzureTableReference
    {
        public string To;
        public Guid Guid;

        public string Reference
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.To))
                {
                    throw new ArgumentException("To");
                }

                if (Guid == Guid.Empty)
                {
                    throw new ArgumentException("Guid");
                }

                return string.Format("{0}:{1}", To, Guid.ToString());
            }
            set
            {
                var words = value.Split(':');

                if (words.Length != 2)
                {
                    throw new ArgumentException("reference");
                }

                this.To = words[0];
                this.Guid = Guid.Parse(words[1]);
            }
        }
    }
}
