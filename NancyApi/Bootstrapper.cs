﻿using System;
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
using MyData;
using NLogWrapper;

namespace NancyApi 
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        //check https://github.com/NancyFx/Nancy/wiki/The-Application-Before,-After-and-OnError-pipelines for a better to do this

        private ILogger _logger = LogManager.CreateLogger(typeof(Bootstrapper), Helpers.Configsettings.LogLevel());
        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            _logger.Debug("Nancy Api: executing startup");
            StaticConfiguration.DisableErrorTraces = false;
            Nancy.Json.JsonSettings.MaxJsonLength = int.MaxValue;
            new MyData.DataFactory(MyDbType.EtfDb).DbSetup().InitDB();
            CheckHealth();
        }

        private void CheckHealth()
        {
            _logger.Info("Checking config settings..");
            _logger.Info("Running under: Environment.UserName= {0}, Environment.UserDomainName= {1}", Environment.UserName, Environment.UserDomainName);
            SettingsChecker.CheckPresenceAllPlainSettings(typeof(Configsettings));

            _logger.Info("all requried config settings seem present..");
            _logger.Info("Url = {0}", Configsettings.HostUrl());
            _logger.Info("Auth server Url= {0}", Configsettings.AuthUrl());
            _logger.Info("..done with config checks.");
        }

        protected override void RequestStartup(TinyIoCContainer requestContainer, IPipelines pipelines, NancyContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;
            var contentType = context.Request.Headers["content-type"].FirstOrDefault();
            var msg = string.Empty;
            if (path.ToLower().Contains("/requestlog") && method.ToUpper()=="POST")
            {
                var json = ReadBody(context.Request.Body);
                dynamic jsonObj = JObject.Parse(json);
                msg = string.Format("Log request for {0} {1}", jsonObj.Method, jsonObj.Path);
            }
            else
            {
                msg = string.Format("{0} {1}", method, path);
            }
            
            var feedId = context.Request.Headers["X-socketFeedId"].FirstOrDefault() ?? "required Header not set";
            if (feedId.Contains("required Header"))
                _logger.Error("X-socketFeedId not set");

            var socketServerAccesstoken = context.Request.Headers["X-socketServerAccessToken"].FirstOrDefault() ?? "required Header not set";
            if (socketServerAccesstoken.Contains("required Header"))
                _logger.Error("X-socketFeedId not set");

            WebNotification.Send(socketServerAccesstoken, feedId, msg);
        }

        private string ReadBody(RequestStream body)
        {
            StreamReader reader = new StreamReader(body);
            return reader.ReadToEnd();
        }
    }
}