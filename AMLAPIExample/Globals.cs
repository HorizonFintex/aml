using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AMLAPIExample
{
    public static class Globals
    {
        public static string b2cClientId;
        public static string b2cClientSecret;
        public static string b2cRedirectUrl;
        public static string b2cScope;
        public static string b2cRefreshToken;
        public static string b2cGrantType;
        public static string b2cRequestEndpoint;
        public static string amlAPIEndpoint;

        static Globals()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.github.json").Build();
            b2cClientId = configuration["b2c:ClientId"];
            b2cClientSecret = configuration["b2c:ClientSecret"];
            b2cRedirectUrl = configuration["b2c:RedirectUrl"];
            b2cScope = configuration["b2c:Scope"];
            b2cRefreshToken = configuration["b2c:RefreshToken"];
            b2cGrantType = configuration["b2c:GrantType"];
            b2cRequestEndpoint = configuration["b2c:RequestEndpoint"];
            amlAPIEndpoint = configuration["amlAPIEndpoint"];
        }

    }
}
