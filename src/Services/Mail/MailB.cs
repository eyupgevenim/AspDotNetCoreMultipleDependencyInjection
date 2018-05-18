using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Mail
{
    public class MailB : IMail
    {
        public string Send()
        {
            return "Mail B";
        }
    }
}
