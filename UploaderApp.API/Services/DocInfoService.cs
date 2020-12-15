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
        private readonly IMongoDatabase _db;
        private string _dbName;

        public DocInfoService(IDocInfoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _dbName = settings.DatabaseName;
            _db = client.GetDatabase(_dbName);
            _docInfos = _db.GetCollection<DocInfo>(settings.CollectionName);
        }

        public async Task<PagedList<DocInfo>> GetDocInfo([FromQuery] ReportParams rptParams)
        {

            var docs = _docInfos.Find(docInfo => true).ToList();

            var result = await PagedList<DocInfo>.CreateAsyncMongo(docs, rptParams.PageNumber, rptParams.PageSize);
            return result;
        }

        public async Task<PagedList<DocInfo>> GetDocsFilteredResult([FromQuery] ReportParams rptParams)
        {
            var keyword = rptParams.Keyword;
            var builder = Builders<DocInfo>.Filter;
            PagedList<DocInfo> result = null;
            ///if keys is null
            if (rptParams.Keys == null)
            {
                if (keyword != null & !String.IsNullOrEmpty(keyword))
                {
                    var value = new BsonRegularExpression("/^" + keyword + "$/i");
                    var filter = builder.Regex("FirstName", value);
                    filter = filter
                    | builder.Regex("LastName", value)
                    | builder.Regex("EmailAddress", value)
                    | builder.Regex("Title", value)
                    | builder.Regex("Company", value)
                    | builder.Regex("Description", value);

                    //this is not working properly need to think about the way to 
                    var filteredList = _docInfos.Find(filter)
                                           .Sort(Builders<DocInfo>.Sort.Descending(r => r.dateSent))
                                           .ToList();
                    result = await PagedList<DocInfo>.CreateAsyncMongo(filteredList, rptParams.PageNumber, rptParams.PageSize);
                }
            }
            else
            {
                //apply activated filters from UI  //initialize filter
                var filter = builder.Eq(rptParams.Keys[0], rptParams.GetType().GetProperty(rptParams.Keys[0]).GetValue(rptParams, null));
                foreach (string key in rptParams.Keys)
                {
                    if (key != rptParams.Keys[0])
                    {
                        var value = rptParams.GetType().GetProperty(key).GetValue(rptParams, null);
                        filter = filter & builder.Eq(key, value);
                    }
                }
                if (keyword != null & !String.IsNullOrEmpty(keyword))
                {
                    var value = new BsonRegularExpression("/^" + keyword + "$/i");
                    filter = filter
                    & builder.Regex("FirstName", value)
                    | builder.Regex("LastName", value)
                    | builder.Regex("EmailAddress", value)
                    | builder.Regex("Title", value)
                    | builder.Regex("Company", value)
                    | builder.Regex("Description", value);
                }
                //this is not working properly need to think about the way to 
                var filteredList = _docInfos.Find(filter)
                                       .Sort(Builders<DocInfo>.Sort.Descending(r => r.dateSent))
                                       .ToList();
                result = await PagedList<DocInfo>.CreateAsyncMongo(filteredList, rptParams.PageNumber, rptParams.PageSize);
            }
            return result;
        }

        public DocInfo CreatDocInfo(DocInfo docInfo)
        {
            _docInfos.InsertOne(docInfo);
            if (docInfo!=null){
                var fileId = this.UploadFile(docInfo.FilePath, docInfo.Id);
                docInfo.FileId = fileId.ToString();
                this.UpdateDocInfo(docInfo.Id, docInfo );
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

        public ObjectId UploadFromStreamAsync(string filename, System.IO.Stream stream,GridFSUploadOptions options )
        {
            GridFSBucket gridFSBucket = new GridFSBucket(_db);
            var gridFsBucket2 = new GridFSBucket(_db);
            // var task = Task.Run(() => {
            //   gridFsBucket.UploadFromStreamAsync(filename, stream);
            // });
            
            ObjectId fid = gridFsBucket2.UploadFromStream(filename, stream,options);

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
            
                    string filename2 = "docup" + DateTime.Now.ToString("yyyyMMdd-hhmmss") + ".txt";
                    return fs.UploadFromStreamAsync(filename2, s,options);
                });

                return t.Result;
            }
        }

        public async Task<bool> FileExistsAsync(ObjectId oid) // string gfsname, string bucketName)
        {
            var bucket = new GridFSBucket(_db);
            //, new GridFSBucketOptions
            //{
            //    BucketName = bucketName
            //}); ;

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

                //var t = fs.DownloadAsBytesByNameAsync("docup20201208-023109.txt");

                var filter = Builders<GridFSFileInfo>.Filter.Eq("_id", oid);
                // var test = await dbfiles.Find(filter).FirstOrDefaultAsync();
                var test = await fsBucket.FindAsync(filter);
                var firstData = test.FirstOrDefault();

                var dataStream = await fsBucket.OpenDownloadStreamAsync(oid);
                return dataStream;

                // var x = fs.DownloadAsBytesAsync(oid);
                // Task.WaitAll(x);
                // var bytes = x.Result;

                // using (var newFs = new FileStream(test.Filename, FileMode.Create))
                // {
                //     newFs.Write(bytes, 0, bytes.Length);
                // }

                // return bytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //using (var stream = fileObj.OpenRead())
            //{
            //    var bytes = new byte[stream.Length];
            //    stream.Read(bytes, 0, (int)stream.Length);
            //    using (var newFs = new FileStream(fileObj.FirstOrDefault().filename, FileMode.Create))
            //    {
            //        newFs.Write(bytes, 0, bytes.Length);
            //    }
            //}

            //This blows chunks (I think it's a driver bug, I'm using 2.1 RC-0)
            //var x = fs.DownloadAsBytesAsync(oid);
            //Task.WaitAll(x);
            //var bytes2 = x.Result;
            return null;
        }
    }
}