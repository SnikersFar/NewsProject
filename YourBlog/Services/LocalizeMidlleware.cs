using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;


namespace YourBlog.Services
{
    public class LocalizeMidlleware
    {
        private readonly RequestDelegate _next;

        public LocalizeMidlleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {

            LanguageFromCookie(context);


            await _next(context);
        }


        private void LanguageFromCookie(HttpContext context)
        {
            int myLang;

            if (context.Request.Cookies.ContainsKey("Language"))
            {
                var lang = context.Request.Cookies["Language"];
                if (!Int32.TryParse(lang, out myLang))
                {
                    context.Response.Cookies.Append("Language", "2");
                }
            }
            else
            {
                context.Response.Cookies.Append("Language", "2");
                myLang = 2;
            }

            switch ((Language)myLang)
            {
                case Language.Ru:
                    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");
                    break;
                case Language.En:
                    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-EN");
                    break;
                default:
                    CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-EN");
                    break;
            }

        }
    }

}
