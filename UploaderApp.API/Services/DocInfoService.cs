using UploaderApp.API.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using UploaderApp.API.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;


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

        
        public async Task<PagedList<DocInfo>> GetDocInfo([FromQuery] ReportParams rptParams){

           var docs = _docInfos.Find(docInfo => true).ToList();

           var result = await PagedList<DocInfo>.CreateAsyncMongo(docs, rptParams.PageNumber, rptParams.PageSize);
           return result;
        }

        public async Task<PagedList<DocInfo>> GetDocsFilteredResult([FromQuery] ReportParams rptParams){
          var status = rptParams.Status;
          var builder = Builders<DocInfo>.Filter;
          //initialize filter
          var filter = builder.Eq(rptParams.Keys[0],rptParams.GetType().GetProperty(rptParams.Keys[0]).GetValue(rptParams, null) );
           
           //apply activated filters from UI 
           foreach (string key in rptParams.Keys)
           {
              if(key != rptParams.Keys[0]){
                var value = rptParams.GetType().GetProperty(key).GetValue(rptParams, null);
                filter = filter & builder.Eq( key , value );
              }
           }

           if(rptParams.Keyword!=null)
           {
                var value = rptParams.Keyword;
                filter = filter 
                & builder.Eq( "FirstName" , value )
                & builder.Eq( "LastName" , value )
                & builder.Eq( "EmailAddress" , value )
                & builder.Eq( "Title" , value )
                & builder.Eq( "Company" , value )
                & builder.Eq( "DocumentFullName" , value )
                & builder.Eq( "Description" , value );
           }
           //sort by date descending
           var filteredList = _docInfos.Find(filter)
                         .SortByDescending(r =>(status == "Agreed" ? r.dateAgreed : status == "Resent" ? r.dateResent : status == "Viewed" ? r.dateViewed : r.dateSent))
                         .ToList();
           var result = await PagedList<DocInfo>.CreateAsyncMongo(filteredList, rptParams.PageNumber, rptParams.PageSize);
           return result;
        }

        public DocInfo CreatDocInfo(DocInfo docInfo)
        {
            _docInfos.InsertOne(docInfo);
            return docInfo;
        }
        
        // public void Update(string id, DocInfo docInfoIn) =>
        //     _docInfos.ReplaceOne(docInfo => docInfo.Id == id, docInfoIn);

        // public void Remove(DocInfo docInfoIn) =>
        //     _docInfos.DeleteOne(docInfo => docInfo.Id == docInfoIn.Id);

        // public void Remove(string id) => 
        //     _docInfos.DeleteOne(docInfo => docInfo.Id == id);
    }
}