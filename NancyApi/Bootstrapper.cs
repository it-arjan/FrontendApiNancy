using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Nancy.TinyIoc;
using Nancy.Bootstrapper;
using NancyApi.Helpers;
using Newtonsoft.Json.Linq;
using Nancy.IO;
using System.IO;

namespace NancyApi 
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        //check https://github.com/NancyFx/Nancy/wiki/The-Application-Before,-After-and-OnError-pipelines for a better to do this

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            StaticConfiguration.DisableErrorTraces = false;
            Nancy.Json.JsonSettings.MaxJsonLength = int.MaxValue;
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;
            var contentType = context.Request.Headers["content-type"].FirstOrDefault();
            var msg = string.Empty;
            if (path.ToLower().EndsWith("requestlog") && method.ToUpper()=="POST" && contentType =="application/json")
            {
                var json = ReadBody(context.Request.Body);
                dynamic jsonObj = JObject.Parse(json);
                msg = string.Format("Log request for {0} {1}", jsonObj.Method, jsonObj.Path);
            }
            else
            {
                msg = string.Format("{0} {1}", method, path);
            }
            
            var sockettoken = context.Request.Headers["X-socketToken"].FirstOrDefault() ?? "required Header not set";
            WebNotification.Send(sockettoken, msg);
        }

        private string ReadBody(RequestStream body)
        {
            StreamReader reader = new StreamReader(body);
            return reader.ReadToEnd();
        }
    }
}