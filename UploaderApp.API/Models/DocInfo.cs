using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UploaderApp.API.Models
{
    public class DocInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string FileId {get; set;}

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string Title { get; set; }

        public string Company { get; set; }

        public string DocumentFullName { get; set; }

        public string Description { get; set; }

        public string UniqueLinkId { get; set; }

        public int SalesforceId { get; set; }

        public string FilePath { get; set; }

        public DateTime dateSent { get; set; }
        public DateTime dateViewed { get; set; }
        public DateTime dateAgreed { get; set; }
        public DateTime dateResent { get; set; }

        public string Status { get; set; }

        public string[] Tags {get; set;}
        public string[] Owners {get; set;}
    }
}