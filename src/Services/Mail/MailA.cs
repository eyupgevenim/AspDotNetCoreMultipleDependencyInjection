using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Mail
{
    public class MailA : IMail
    {
        public string Send()
        {
            return "Mail A";
        }
    }
}
