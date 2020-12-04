using System.Collections.Generic;
using System;

namespace UploaderApp.API.Helpers
{
    public class ReportParams
    {
        private const int MaxPageSize = 50;
        private int pageSize=10;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value>MaxPageSize) ? MaxPageSize: value ; }
        }

        private String[] keys;
        public String[] Keys
        {
            get { return keys; }
            set { keys = value!=null ? value[0].Split(',') : null; }
        }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Title { get; set; }
        public string Company { get; set; }
        public string DocumentFullName { get; set; }
        public string Description { get; set; }
        public string UniqueLinkId { get; set; }
        
        public string Status { get; set; }

        public DateTime dateSent { get; set; }
        public DateTime dateViewed { get; set; }
        public DateTime dateAgreed { get; set; }
        public DateTime dateResent { get; set; }
        
        public string Keyword { get; set;}
   } 

}