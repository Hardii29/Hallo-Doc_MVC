using Hallo_Doc.Entity.Models;
namespace Hallo_Doc.Entity.ViewModel

{
    public class DashboardList
    {
        public RequestWiseFile? SpecificFile { get; set; }
        public List<Request>? Requests { get; set; }
        public List<RequestWithFile> RequestWithFiles { get; set; }
        public List<RequestWiseFile>? RequestWiseFiles { get; set; }
        public string? UserName { get; set; }
        public int UserId { get; set; }
        public DashboardList()
        {
            RequestWithFiles = new List<RequestWithFile>();
        }
    }
    public class RequestWithFile
    {
        public int RequestId { get; set; }
        public int fileId { get; set; }
        public Request? Request { get; set; }
        public bool HasFiles { get; set; }
    }
    public class Request
    {
        public int? RequestId { get; set; }
        public DateTime CreatedDate { get; set; }
        public short? Status { get; set; }
        public int UserId { get; set; }
        public int fileId { get; set; }
    }
    public class RequestWiseFile
    {
        public int? RequestWiseFileID { get; set; }
        public string? FileName { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}

