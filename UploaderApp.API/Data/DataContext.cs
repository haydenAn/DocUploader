using Microsoft.EntityFrameworkCore;
using UploaderApp.API.Models;

namespace UploaderApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Value> Values { get; set; }
        public DbSet<DocumentInfo> DocumentInfo { get; set; }
    }
}