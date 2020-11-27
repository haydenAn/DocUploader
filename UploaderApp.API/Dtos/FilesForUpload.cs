using System;
using Microsoft.AspNetCore.Http;

namespace UploaderApp.API.Dtos
{
    public class FilesForUpload
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        // public string Description { get; set; }
        // public DateTime DateAdded { get; set; }
        // public string PublicId { get; set; }

       public FilesForUpload()
       {
          //  DateAdded = DateTime.Now;
       }

    }
}