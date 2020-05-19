using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Wyklad3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly static string filePath = "requestsLog.txt";

        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();

            string body;
            using (var streamReader = new System.IO.StreamReader(httpContext.Request.Body, System.Text.Encoding.UTF8))
            {
                body = await streamReader.ReadToEndAsync();
            }

            string logEntry = String.Format(
                "method='{0}' path='{1}' body='{2}' query='{3}'",
                httpContext.Request.Method, httpContext.Request.Path, body, httpContext.Request.QueryString.ToString()
            );

            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.WriteLine(logEntry);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(logEntry);
                }
            }
            await _next(httpContext);
        }
    }

}
