using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UploaderApp.API.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EmailAddress { get; set; }

    }
}