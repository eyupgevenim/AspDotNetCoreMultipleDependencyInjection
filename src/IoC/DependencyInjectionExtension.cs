using Microsoft.Extensions.DependencyInjection;
using Services;
using Services.File;
using Services.Mail;
using System;
using System.Collections.Generic;
using System.Text;

namespace IoC
{
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
}
