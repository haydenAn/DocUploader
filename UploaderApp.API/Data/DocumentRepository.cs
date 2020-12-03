using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UploaderApp.API.Helpers;
using UploaderApp.API.Models;

namespace UploaderApp.API.Data
{
    public class DocumentRepository : ISendLinkRepository
    {
        private readonly DataContext _context;
        public DocumentRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }     

        public async Task<DocumentInfo> GetEmailLink(Guid guid, string guidString)
        {
            if (guidString.Contains("99"))
            {
                return await _context.DocumentInfo.FirstOrDefaultAsync(d => d.Id == 1);
            }
            return await _context.DocumentInfo.FirstOrDefaultAsync(d => d.UniqueLinkId == guidString);
        }

         public async Task<DocumentInfo> GetDocumentInfoById(int id)
        {
            return await _context.DocumentInfo.FirstOrDefaultAsync(doc => doc.Id == id);
        }

        public async Task<PagedList<DocumentInfo>> GetReport(ReportParams rptParams, string filter = "", string search = "")
        {
            // DbSet<DocumentInfo> docs = _context.DocumentInfo; // as IQueryable<DocumentInfo>;
            IQueryable<DocumentInfo> query;
            if (!string.IsNullOrEmpty(search)) {
               query = _context.DocumentInfo
                    .Where(doc => doc.LastName.Contains(search) || doc.Company.Contains(search))
                    .OrderByDescending(r => (filter == "Agreed" ? r.dateAgreed : filter == "Resent" ? r.dateResent : filter == "Viewed" ? r.dateViewed : r.dateSent));
               if (!string.IsNullOrEmpty(filter)) { 
                   query = query.Where(doc => doc.Status == filter);
               }
               return await PagedList<DocumentInfo>.CreateAsync(query, rptParams.PageNumber, rptParams.PageSize);
            }
            else if (filter == "Agreed" || filter=="Viewed" || filter == "Sent" || filter == "Resent") {
               var q = _context.DocumentInfo.Where(doc => doc.Status == filter).OrderByDescending(r => (filter == "Agreed" ? r.dateAgreed : filter == "Resent" ? r.dateResent : filter == "Viewed" ? r.dateViewed : r.dateSent));
               return await PagedList<DocumentInfo>.CreateAsync(q, rptParams.PageNumber, rptParams.PageSize);
            } else { 
                // DbSet<DocumentInfo> docs = _context.DocumentInfo;
                var docs = _context.DocumentInfo.Where(x => x != null).OrderByDescending(r => (filter == "Agreed" ? r.dateAgreed : filter == "Resent" ? r.dateResent : filter == "Viewed" ? r.dateViewed : r.dateSent));
                var r =  await PagedList<DocumentInfo>.CreateAsync(docs, rptParams.PageNumber, rptParams.PageSize);
                return r;
            }            
        }
       public async Task<PagedList<DocInfo>> GetDocs(ReportParams rptParams, string filter = "", string search = "")
      {
                      IQueryable<DocInfo> query;
            if (!string.IsNullOrEmpty(search)) {
               query = _context.DocInfo
                    .Where(doc => doc.LastName.Contains(search) || doc.Company.Contains(search))
                    .OrderByDescending(r => (filter == "Agreed" ? r.dateAgreed : filter == "Resent" ? r.dateResent : filter == "Viewed" ? r.dateViewed : r.dateSent));
               if (!string.IsNullOrEmpty(filter)) { 
                   query = query.Where(doc => doc.Status == filter);
               }
               return await PagedList<DocInfo>.CreateAsync(query, rptParams.PageNumber, rptParams.PageSize);
            }
            else if (filter == "Agreed" || filter=="Viewed" || filter == "Sent" || filter == "Resent") {
               var q = _context.DocInfo.Where(doc => doc.Status == filter).OrderByDescending(r => (filter == "Agreed" ? r.dateAgreed : filter == "Resent" ? r.dateResent : filter == "Viewed" ? r.dateViewed : r.dateSent));
               return await PagedList<DocInfo>.CreateAsync(q, rptParams.PageNumber, rptParams.PageSize);
            } else {  
                // DbSet<DocumentInfo> docs = _context.DocumentInfo;
                var docs = _context.DocInfo.Where(x => x != null).OrderByDescending(r => (filter == "Agreed" ? r.dateAgreed : filter == "Resent" ? r.dateResent : filter == "Viewed" ? r.dateViewed : r.dateSent));
                return await PagedList<DocInfo>.CreateAsync(docs, rptParams.PageNumber, rptParams.PageSize);
            }         
       }
 
        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}