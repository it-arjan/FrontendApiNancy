using MyData;
using MyData.Models;
using System.Collections.Generic;
using System.Linq;


namespace NancyApi.Modules
{
    public class PostbackDataModule : Nancy.NancyModule
    {
        public PostbackDataModule() : base("api/postbacks")
        {
            Get["/"] = _ => Hello(_.SessionId);
            Get["/{SessionId}"]=  _ => GetPostbacks(_.SessionId);
        }
        private List<PostbackData> GetPostbacks(string SessionId)
        {
            var db = new DataFactory(MyDbType.EtfDb).Db();
            return db.GetPostbacksFromToday();
        }
        private string Hello(string SessionId)
        {
            return "Helloooo :)";
        }
    }
}