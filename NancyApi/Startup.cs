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

        public void Configuration(IAppBuilder app)
        {
            if (Configsettings.OnAzure())
            {
                // Azure has no way of trusting a self signed certificate, 
                // so only option left is to disable the certificate check
                ServicePointManager.ServerCertificateValidationCallback +=
                            (sender, cert, chain, sslPolicyErrors) => true;
            }

            // silicon client authorization
            app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = Configsettings.AuthUrl(),
                ValidationMode = ValidationMode.Local, 
                RequiredScopes = new[] { IdSrv3.ScopeFrontendDataApi }
            });

            app.UseNancy();

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
