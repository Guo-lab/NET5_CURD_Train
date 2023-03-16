using System;
using System.Collections.Generic;

namespace ESC5.Email.Message
{
    [Serializable]
    public class EmailMessage
    {
        public string CustomerCode { get; set; }
        public string ReceiverE2bizCode { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Format { get; set; }
        public string Body { get; set; }

        public string Priority { get; set; }

        public string ReplyTo { get; set; }
        public IList<string> Attachment{ get; set; }
        public string IsGenerateZip { get; set; }


    }
}
