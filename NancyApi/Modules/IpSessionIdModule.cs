using MyData;
using MyData.Models;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;

namespace NancyApi.Modules
{
    public class IpSessionIdModule : Nancy.NancyModule
    {
        IData _db;
        public IpSessionIdModule() : base("api/ipsessionid")
        {
            this.RequiresMSOwinAuthentication();

            _db = new DataFactory(MyDbType.EtfDb).Db(); //nancy handles disposal
            Get["/{id}"] = _ => FindIpSessionId(_.id);
            Get["/exists/{SessionId}"] = _ => SessionIdExists(_.SessionId);
            Get["/exists/{SessionId}/{ip}"] =  _ => IpSessionIdExists(_.SessionId, _.ip );
            Delete["/{id}"] = _ =>RemoveIpSessionId(_.id);
            Post["/"] = _ => AddIpSessionId();
        }

        private bool SessionIdExists(string session)
        {
            return _db.SessionIdExists(session);
        }

        private bool IpSessionIdExists(string session, string ip)
        {
            var result = _db.IpSessionIdExists(session, ip);
            return result;
        }
        private IpSessionId FindIpSessionId(int id)
        {
            return _db.FindIpSessionId(id);
        }

        private HttpStatusCode RemoveIpSessionId(int id)
        {
            _db.RemoveIpSessionid(id);
            _db.Commit();
            return HttpStatusCode.NoContent;
        }

        private HttpStatusCode AddIpSessionId()
        {
            var x = this.Bind<IpSessionId>();
            _db.Add(x);
            _db.Commit();
            return HttpStatusCode.OK;
        }
 
    }
}