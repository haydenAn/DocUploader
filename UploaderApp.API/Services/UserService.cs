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
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoDatabase _db;
        private string _dbName;

        public UserService(IDocumentInfoDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _dbName = settings.DatabaseName;
            _db = client.GetDatabase(_dbName);
            _users = _db.GetCollection<User>(settings.UserCollectionName);
        }

        public async Task<User> GetUserByEmail([FromQuery] string email)
        {
            var user = await _users.FindAsync(user => user.EmailAddress == email);
            var firstData = user.FirstOrDefault();
            return firstData;
        }

        
        public async Task<User> CreateUser(User user)
        {
            //TODO EXCEPTION HANDLING think about the better structure for error msg class
          string errorMessage = "";
          var existingUser = await this.GetUserByEmail(user.EmailAddress);

          if(existingUser!= null){
              errorMessage += $"User with same email address {user.EmailAddress} exists";
            //   Debug.WriteLine(errorMessages);
              return null;
          }
           _users.InsertOne(user);
           var userCreated = this.GetUser(user.Id);
            return userCreated;
        }

        public User GetUser(string id){
           var user = _users.Find(user => user.Id == id).FirstOrDefault();
           return user;
        }

    }
}