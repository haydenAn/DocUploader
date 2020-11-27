using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UploaderApp.API.Models
{
    public class DocumentInfo
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR(250)")]
        public string FirstName { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string LastName { get; set; }

        [Column(TypeName = "VARCHAR(200)")]
        public string EmailAddress { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string Title { get; set; }

        [Column(TypeName = "VARCHAR(100)")]
        public string Company { get; set; }

        [Column(TypeName = "VARCHAR(50)")]
        public string SalesforceId { get; set; }

        [Column(TypeName = "VARCHAR(300)")]
        public string DocumentFullName { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "VARCHAR(50)")]
        public string UniqueLinkId { get; set; }
        public DateTime dateSent { get; set; }
        public DateTime dateViewed { get; set; }
        public DateTime dateAgreed { get; set; }
        public DateTime dateResent { get; set; }

        [Column(TypeName = "VARCHAR(20)")]
        public string Status { get; set; }
    }
}