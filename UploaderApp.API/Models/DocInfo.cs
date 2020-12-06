using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UploaderApp.API.Models
{
    public class DocInfoDbSettings : IDocInfoDbSettings
    {
        public string CollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDocInfoDbSettings
    {
        string CollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
    public class DocInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string Title { get; set; }

        public string Company { get; set; }

        public int SalesforceId { get; set; }

        public string DocumentFullName { get; set; }

        public string Description { get; set; }

        public string UniqueLinkId { get; set; }

        public DateTime dateSent { get; set; }
        public DateTime dateViewed { get; set; }
        public DateTime dateAgreed { get; set; }
        public DateTime dateResent { get; set; }

        public string Status { get; set; }

        public string[] Tags {get; set;}
    }
}