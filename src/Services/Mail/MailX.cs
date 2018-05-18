using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Mail
{
    public class MailX : IMail, ISms
    {
        public string Send()
        {
            return "Mail X";
        }

        public string SmsSend()
        {
            return "Sms Send";
        }
    }
}
