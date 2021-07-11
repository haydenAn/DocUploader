using UploaderApp.API.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using UploaderApp.API.Helpers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using MongoDB.Bson;
using MongoDB.Driver.GridFS;
using System.IO;

namespace UploaderApp.API.Services
{
    public class DocInfoService
    {
        private readonly IMongoCollection<DocInfo> _docInfos;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoDatabase _db;
        private string _dbName;

        public DocInfoService(IDocumentInfoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _dbName = settings.DatabaseName;
            _db = client.GetDatabase(_dbName);
            _docInfos = _db.GetCollection<DocInfo>(settings.DocInfoCollectionName);
            _users = _db.GetCollection<User>(settings.UserCollectionName);
        }

        public async Task<PagedList<DocInfo>> GetDocInfo([FromQuery] ReportParams rptParams)
        {
            var builder = Builders<DocInfo>.Filter;
            FilterDefinition<DocInfo> filter;
            bool isAdmin = false;
            //TODO check user role here
            if (isAdmin)
            {
                var docs = _docInfos.Find(DocInfo=>true).ToList();
                var result = await PagedList<DocInfo>.CreateAsyncMongo(docs, rptParams.PageNumber, rptParams.PageSize);
                return result;
            }
            else
            {
                filter = builder.AnyEq( "Owners", rptParams.UserId);
                var docs = _docInfos.Find(filter).ToList();
                var result = await PagedList<DocInfo>.CreateAsyncMongo(docs, rptParams.PageNumber, rptParams.PageSize);
                return result;
            }
        }

        private FilterDefinition<DocInfo> buildOwnedDocumentFilter(ReportParams rptParams)
        {
            string userId = rptParams.UserId;
            string keyword = rptParams.Keyword;
            string[] keys = rptParams.Keys;

            var user = _users.Find(user => user.Id == userId).FirstOrDefault();
            var builder = Builders<DocInfo>.Filter;
            FilterDefinition<DocInfo> filter;

            //TODO check user role here
            bool isAdmin = false;
            filter = builder.AnyEq("Owners", userId);

            if (keyword != null & !String.IsNullOrEmpty(keyword))
            {
                var keywordFilter = this.buildKeywordFilter(keyword);
                filter = filter & keywordFilter;
            }
            if (keys != null)
            {
                foreach (string key in keys)
                {
                    var value = rptParams.GetType().GetProperty(key).GetValue(rptParams, null);
                    filter = filter & builder.Eq(key, value);
                }
            }
            return filter;
        }

        private FilterDefinition<DocInfo> buildKeywordFilter(string keyword)
        {
            var builder = Builders<DocInfo>.Filter;
            var value = new BsonRegularExpression("/^" + keyword + "$/i");

            var filter = builder.Regex("FirstName", value)
               | builder.Regex("LastName", value)
               | builder.Regex("EmailAddress", value)
               | builder.Regex("Title", value)
               | builder.Regex("Company", value)
               | builder.Regex("Description", value);

            return filter;
        }

        public async Task<PagedList<DocInfo>> GetDocsFilteredResult([FromQuery] ReportParams rptParams)
        {
            var builder = Builders<DocInfo>.Filter;
            PagedList<DocInfo> result = null;

            var filter = this.buildOwnedDocumentFilter(rptParams);

            //this is not working properly need to think about the way to 
            var filteredList = _docInfos.Find(filter)
                                   .Sort(Builders<DocInfo>.Sort.Descending(r => r.dateSent))
                                   .ToList();
            result = await PagedList<DocInfo>.CreateAsyncMongo(filteredList, rptParams.PageNumber, rptParams.PageSize);

            return result;
        }

        public DocInfo CreatDocInfo(DocInfo docInfo)
        {
            _docInfos.InsertOne(docInfo);
            if (docInfo != null)
            {
                var fileId = this.UploadFile(docInfo.FilePath, docInfo.Id);
                docInfo.FileId = fileId.ToString();
                this.UpdateDocInfo(docInfo.Id, docInfo);
            }
            return docInfo;
        }

        public List<DocInfo> GetSingleDoc(string id)
        {
            return _docInfos.Find(x => x.Id == id).ToList();
        }

        public void UpdateDocInfo(string id, DocInfo docInfo) =>
            _docInfos.ReplaceOne(x => x.Id == id, docInfo);

        // public void Remove(DocInfo docInfoIn) =>
        //     _docInfos.DeleteOne(docInfo => docInfo.Id == docInfoIn.Id);

        // public void Remove(string id) => 
        //     _docInfos.DeleteOne(docInfo => docInfo.Id == id);

        public ObjectId UploadFromStreamAsync(string filename, System.IO.Stream stream, GridFSUploadOptions options)
        {
            GridFSBucket gridFSBucket = new GridFSBucket(_db);
            var gridFsBucket2 = new GridFSBucket(_db);
            // var task = Task.Run(() => {
            //   gridFsBucket.UploadFromStreamAsync(filename, stream);
            // });

            ObjectId fid = gridFsBucket2.UploadFromStream(filename, stream, options);

            return fid; //  task.Result;
        }

        public ObjectId UploadFile(string filename, string docinfoid)
        {
            GridFSBucket fs = new GridFSBucket(_db);

            using (var s = File.OpenRead(filename))
            {
                var t = Task.Run<ObjectId>(() =>
                {
                    var options = new GridFSUploadOptions
                    {
                        Metadata = new BsonDocument
                      {
                       { "docinfo_id", docinfoid }
                      }
                    };

                    return fs.UploadFromStreamAsync(filename, s, options);
                });

                return t.Result;
            }
        }

        public async Task<bool> FileExistsAsync(ObjectId oid) // string gfsname, string bucketName)
        {
            var bucket = new GridFSBucket(_db);

            var filter = Builders<GridFSFileInfo>.Filter.Eq(x => x.Id, oid); // .Filename, gfsname);
            var fileInfo = await bucket.FindAsync(filter);

            return fileInfo.Any();
        }

        public async Task<GridFSDownloadStream> DownloadFile(string id)
        {
            ObjectId oid = new ObjectId(id);
            GridFSBucket fsBucket = new GridFSBucket(_db);
            //This works

            try
            {
                IMongoCollection<GridFSFileInfo> dbfiles = _db.GetCollection<GridFSFileInfo>("fs.files");

                var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", oid);
                var test = await fsBucket.FindAsync(filter);
                var firstData = test.FirstOrDefault();

                var dataStream = await fsBucket.OpenDownloadStreamAsync(oid);
                return dataStream;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }
    }
}