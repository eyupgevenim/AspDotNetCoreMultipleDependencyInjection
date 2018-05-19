    Asp.Net Core Multiple Dependency Injection

```csharp
    public interface ISms
    {
        string SmsSend();
    }

    public interface IMail
    {
        string Send();
    } 

    public interface IFileConverter
    {
        string Convert();
    }
```

```csharp

    public class MailA : IMail
    {
        public string Send()
        {
            return "Mail A";
        }
    }

    public class MailB : IMail
    {
        public string Send()
        {
            return "Mail B";
        }
    }

    public class MailC : IMail
    {
        public string Send()
        {
            return "Mail C";
        }
    }

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

    public class JsonConverter : IFileConverter
    {
        public string Convert()
        {
            return "Json Converter";
        }
    }

    public class XmlConverter : IFileConverter
    {
        public string Convert()
        {
            return "Xml Converter";
        }
    }

    public class CsvConverter : IFileConverter
    {
        public string Convert()
        {
            return "Csv Converter";
        }
    }
```

register services extension 
```csharp

    public static class DependencyInjectionExtension
    {
        public static IServiceCollection AddSmsServices(this IServiceCollection services)
        {
            return services.AddTransient<ISms, MailX>();
        }

        public static IServiceCollection AddMailServices(this IServiceCollection services)
        {
            services.AddTransient<IMail, MailA>();
            services.AddTransient<IMail, MailB>();
            services.AddTransient<IMail, MailC>();
            services.AddTransient<IMail, MailX>();
            return services;
        }

        //v1
        public static IServiceCollection AddFileConverterServices(this IServiceCollection services, string key)
        {
            switch (key)
            {
                case "JsonConverter":
                    services.AddScoped<IFileConverter, JsonConverter>();
                    break;
                case "XmlConverter":
                    services.AddScoped<IFileConverter, XmlConverter>();
                    break;
                case "CsvConverter":
                    services.AddScoped<IFileConverter, CsvConverter>();
                    break;
                default:
                    break;
                    ///return null;
                    ///throw new KeyNotFoundException();
            }
            return services;
        }

        //v2
        public static IServiceCollection AddFileConverterDelgServices(this IServiceCollection services)
        {
            services.AddScoped<Func<string, IFileConverter>>(serviceProvider => key =>
            {
                switch (key)
                {
                    case "JsonConverter":
                        return new JsonConverter();
                    case "XmlConverter":
                        return new XmlConverter();
                    case "CsvConverter":
                        return new CsvConverter();
                    default:
                        return null;
                        ///throw new KeyNotFoundException();
                }
            });
            return services;
        }

    }
```

in <code>appsettings.json</code> json file
```csharp
    "ActiveFileConverter": "XmlConverter"
```

in <code>Startup.cs </code> class
```csharp
    public void ConfigureServices(IServiceCollection services)
    {
            services.AddSmsServices();
            services.AddMailServices();
            services.AddFileConverterServices(Configuration["ActiveFileConverter"]);//v1
            services.AddFileConverterDelgServices();//v2

            services.AddMvc();
    }
```

call DI services in <code>HomeController.cs</code>
```csharp
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

        /// <summary>
        /// using Microsoft.Extensions.DependencyInjection;
        /// </summary>
        /// <returns></returns>
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
    }
```
url <code>http://localhost:8359</code> output
```csharp
Sms Send 
Mail A
Mail B
Mail C
Mail X
Xml Converter
```

url <code>http://localhost:8359/Home/GeteServicesInFunction</code> output
```csharp
Sms Send 
Mail A
Mail B
Mail C
Mail X
Xml Converter
```

