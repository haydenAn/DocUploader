using UploaderApp.API.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using UploaderApp.API.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace UploaderApp.API.Services
{
    public class DocInfoService
    {
        private readonly IMongoCollection<DocInfo> _docInfos;

        public DocInfoService(IDocInfoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _docInfos = database.GetCollection<DocInfo>(settings.CollectionName);
        }

        // public List<DocInfo> Get() =>
        //     _docInfos.Find(docInfo => true).ToList();

        public async Task<PagedList<DocInfo>> Get([FromQuery] ReportParams rptParams){

        //    var docsQuery = _docInfos.AsQueryable().Where(x=> x != null);
           var docsQuery = _docInfos.Find(docInfo => true).ToList();

           var result = await PagedList<DocInfo>.CreateAsyncMongo(docsQuery, rptParams.PageNumber, rptParams.PageSize);
           return result;
        }

          public DocInfo Get(string email) =>
            _docInfos.Find<DocInfo>(docInfo => docInfo.EmailAddress == email).FirstOrDefault();

        public DocInfo Create(DocInfo docInfo)
        {
            _docInfos.InsertOne(docInfo);
            return docInfo;
        }
        
        public void Update(string id, DocInfo docInfoIn) =>
            _docInfos.ReplaceOne(docInfo => docInfo.Id == id, docInfoIn);

        public void Remove(DocInfo docInfoIn) =>
            _docInfos.DeleteOne(docInfo => docInfo.Id == docInfoIn.Id);

        public void Remove(string id) => 
            _docInfos.DeleteOne(docInfo => docInfo.Id == id);
    }
}