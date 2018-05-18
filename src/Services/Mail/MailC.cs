using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Mail
{
    public class MailC : IMail
    {
        public string Send()
        {
            return "Mail C";
        }
    }
}
