namespace EmailProcessor
{
    public class SMTPSetting
    {
        public string CustomerCode { get; set; }
        public string SMTPServer { get; set; }
        public string SMTPUID { get; set; }
        public string SMTPPWD { get; set; }
        public string Sender { get; set; }
        public bool EnableSsl { get; set; }
        public int Port { get; set; }
    }
}
