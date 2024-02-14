namespace Hallo_Doc.Models.ViewModel
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
        public List<Request> Requests { get; set; }
        public List<RequestWiseFile> RequestFiles { get; set; }
    }
}
