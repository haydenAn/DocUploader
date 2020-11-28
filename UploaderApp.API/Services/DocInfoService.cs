using UploaderApp.API.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

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

        public List<DocInfo> Get() =>
            _docInfos.Find(docInfo => true).ToList();
        
        
        public DocInfo Get(string id) =>
            _docInfos.Find<DocInfo>(docInfo => docInfo.Id == id).FirstOrDefault();

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