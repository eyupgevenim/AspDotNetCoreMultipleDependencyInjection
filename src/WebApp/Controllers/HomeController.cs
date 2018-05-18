using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISms _sms;
        private readonly IEnumerable<IMail> _mails;
        private readonly IFileConverter _fileConverter;//v1
        private readonly Func<string, IFileConverter> _fileConverterDelg;//v2
        public HomeController(ISms sms,
            IEnumerable<IMail> mails, 
            IFileConverter fileConverter,//v1 
            Func<string, IFileConverter> fileConverterDelg//v2
            )
        {
            _sms = sms;
            _mails = mails;
            _fileConverter = fileConverter;//v1
            _fileConverterDelg = fileConverterDelg;//v2
        }

        public IActionResult Index()
        {
            StringBuilder stringBuilder = new StringBuilder();
            
            //sms
            stringBuilder.Append(_sms.SmsSend() + " \n");

            //mails
            ///var mailA = _mails.First(o =>o is MailA);
            ///var mailA = _mails.First(o => o.GetType() == typeof(MailA));
            //or
            ///var mailA = _mails.First(o => o.GetType().Name.Equals("MailA"));
            foreach (var mail in _mails)
            {
                stringBuilder.Append(mail.Send() + "\n");
            }

            //file Converter
            stringBuilder.Append(_fileConverter.Convert());//v1
            ///stringBuilder.Append(_fileConverterDelg("XmlConverter").Convert());//v2


            return Content(stringBuilder.ToString());
        }

        public IActionResult GeteServicesInFunction()
        {
            StringBuilder stringBuilder = new StringBuilder();

            //sms
            ISms smsService = HttpContext.RequestServices.GetService<ISms>();
            stringBuilder.Append(smsService.SmsSend() + " \n");

            //mails
            IEnumerable<IMail> mailServices = HttpContext.RequestServices.GetServices<IMail>();
            foreach (var mail in mailServices)
            {
                stringBuilder.Append(mail.Send() + "\n");
            }

            //file Converter
            IFileConverter fileConverterService = HttpContext.RequestServices.GetService<IFileConverter>();//v1
            Func<string, IFileConverter> fileConverterServiceDelg = HttpContext.RequestServices.GetService<Func<string, IFileConverter>>();//v2
            stringBuilder.Append(fileConverterService.Convert());//v1
            ///stringBuilder.Append(fileConverterServiceDelg("XmlConverter").Convert());//v2

            return Content(stringBuilder.ToString());
        }

        

        public IActionResult Error()
        {
            return View();
        }
    }
}
