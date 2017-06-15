using MyData;
using MyData.Models;
using System.Collections.Generic;
using System.Linq;

namespace NancyApi.Modules
{
    public class RequestLogModule : Nancy.NancyModule
    {
        public RequestLogModule() : base("api/requestlogs")
        {
            Get["/"] = _ => Hello(_.SessionId);
            Get["/{amount}/{SessionId}"]=  _ => GetLogs(_.amount, _.SessionId);
        }

        private List<RequestLogEntry> GetLogs(int amount, string SessionId)
        {
            var db = new DataFactory(MyDbType.EtfDb).Db();
            return db.GetRecentRequestLog(amount, SessionId);
        }
        private string Hello(string SessionId)
        {
            return "Helloooo " + SessionId;
        }
    }
}