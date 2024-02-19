using Hallo_Doc.Entity.Models;
namespace Hallo_Doc.Entity.ViewModel
{
    //public class RequestViewModel
    //{
    //    public int RequestId { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public short Status { get; set; }
    //    public bool HasFile { get; set; } 
    //}

    public class DashboardList
    {
        public List<Request>? Requests { get; set; }
        public List<RequestWiseFile>? RequestWiseFiles { get; set; }
        //public int RequestWiseFileID { get; set; }
        //public string FileName { get; set; }
        //public DateTime CreatedDate { get; set; }
    }
 
}
