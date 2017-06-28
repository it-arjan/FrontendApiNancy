using MyData;
using MyData.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Nancy.ModelBinding;
using Nancy;
using Nancy.Security;

namespace NancyApi.Modules
{
    public class RequestLogModule : Nancy.NancyModule
    {
        IData _db;
        public RequestLogModule() : base("api/requestlog")
        {
            Get["/"] = _ => Hello(_.SessionId);
            this.RequiresMSOwinAuthentication();
            _db = new DataFactory(MyDbType.EtfDb).Db(); //nancy handles disposal
            Get["/{id}"] = _ => FindLog(_.id);
            Get["/recent/take/{amount}"] = _ => GetLogs(_.amount);
            Get["/{SessionId}/recent/take/{amount}"] =  _ => GetLogs(_.amount, _.SessionId);

            Delete["/{id}"] = _ => RemoveLog(_.id);
            Post["/"] = _ => AddLog();
        }

        private List<RequestLogEntry> GetLogs(int amount)
        {
            return _db.GetRecentRequestLogs(amount);
        }
        private List<RequestLogEntry> GetLogs(int amount, string SessionId)
        {
            return _db.GetRecentRequestLogs(amount, SessionId);
        }
        private RequestLogEntry FindLog(int id)
        {
            return _db.FindRequestLog(id);
        }
        private HttpStatusCode AddLog()
        {
            var x = this.Bind<RequestLogEntry>();
            _db.Add(x);
            _db.Commit();
            return HttpStatusCode.OK;
        }

        private HttpStatusCode RemoveLog(int id)
        {
            _db.RemoveRequestlog(id);
            _db.Commit();
            return HttpStatusCode.NoContent;
        }

        private string Hello(string SessionId)
        {
            return "Hello requestlogs " + SessionId;
        }
    }
}