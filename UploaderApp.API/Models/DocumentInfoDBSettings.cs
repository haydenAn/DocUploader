namespace UploaderApp.API.Models
{

     public class DocumentInfoDBSettings : IDocumentInfoDBSettings
    {
        public string DocInfoCollectionName { get; set; }
        public string UserCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IDocumentInfoDBSettings
    {
        string DocInfoCollectionName { get; set; }
        string UserCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}