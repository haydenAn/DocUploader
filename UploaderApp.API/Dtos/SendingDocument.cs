namespace UploaderApp.API.Dtos
{
    public class SendingDocument
    {
        public string[] Documents { get; set; }
        public string[] RecipientList { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
    
    }
}