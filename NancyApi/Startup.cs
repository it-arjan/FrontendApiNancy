using System;
using Microsoft.Owin;
using Owin;
using NLogWrapper;
using IdentityServer3.AccessTokenValidation;
using System.Net;
using NancyApi.Helpers;
using Nancy.Owin;

[assembly: OwinStartup(typeof(NancyApi.Startup))]

namespace NancyApi
{
    public class Startup
    {
        private ILogger _logger = LogManager.CreateLogger(typeof(Startup), Configsettings.LogLevel());
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


        public void Configuration(IAppBuilder app)
        {
            if (Configsettings.OnAzure())
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;
            }

            _logger.Info("startup starting");

            CheckHealth();

            // silicon client authorization
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = Configsettings.AuthUrl(),
                ValidationMode = ValidationMode.Local, 
                RequiredScopes = new[] { IdSrv3.ScopeNancyApi }
            });

            // Later figure out auth config for nancy
            app.UseNancy();

            _logger.Info("startup executed");
        }



        private static bool IsAjaxRequest(IOwinRequest request)
        {
            IReadableStringCollection query = request.Query;
            if ((query != null) && (query["X-Requested-With"] == "XMLHttpRequest"))
            {
                return true;
            }
            IHeaderDictionary headers = request.Headers;
            return ((headers != null) && (headers["X-Requested-With"] == "XMLHttpRequest"));
        }
    }
}
