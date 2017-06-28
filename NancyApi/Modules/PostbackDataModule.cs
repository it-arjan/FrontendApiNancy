using MyData;
using MyData.Models;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using NLogWrapper;
using System.Collections.Generic;
using System.Linq;


namespace NancyApi.Modules
{
    public class PostbackDataModule : Nancy.NancyModule
    {
        private ILogger _logger = LogManager.CreateLogger(typeof(PostbackDataModule), Helpers.Configsettings.LogLevel());
        IData _db;
        public PostbackDataModule() : base("api/postback")
        {
            Get["/"] = _ => Hello("");
            this.RequiresMSOwinAuthentication();
            _db = new DataFactory(MyDbType.EtfDb).Db(); //nancy handles disposal
            Get["/today"] = _ => GetPostbacksFromToday();
            Get["/{id}"] = _ => FindPostback(_.id);
            Get["/recent/take/{amount}"] = _ => GetPostbacks(_.amount);
            Get["/{SessionId}/recent/take/{amount}"] = _ => GetPostbacks(_.amount, _.SessionId);

            Delete["/{id}"] = _ => RemovePostback(_.id);
            Post["/"] = _ => AddPostback();
        }
        private string Hello(string SessionId)
        {
            return "hello";
        }
        private List<PostbackData> GetPostbacks(int nr, string sessionId)
        {
           return _db.GetRecentPostbacks(nr, sessionId);
        }
        private List<PostbackData> GetPostbacks(int nr)
        {
           return _db.GetRecentPostbacks(nr);
        }
        private PostbackData FindPostback(int id)
        {
            return _db.FindPostback(id);
        }
        private List<PostbackData> GetPostbacksFromToday()
        {
            return _db.GetPostbacksFromToday();
        }
 
        private HttpStatusCode RemovePostback(int id)
        {
            _db.RemovePostback(id);
            _db.Commit();
            return HttpStatusCode.NoContent;
        }
        private HttpStatusCode AddPostback()
        {
            var x = this.Bind<PostbackData>();
            _db.Add(x);
            _db.Commit();
            return HttpStatusCode.OK;
        }

    }
}