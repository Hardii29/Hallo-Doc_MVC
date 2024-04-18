using Microsoft.AspNetCore.Http;

namespace Hallo_Doc.Entity.ViewModel
{
    public class ViewDocument
    {
        public int RequestWiseFileID { get; set; }
        public int RequestId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? FileName { get; set; }
        public string? UserName { get; set; }
        public int UserId { get; set; }
        public IFormFile? File { get; set; }
        public virtual Request Request { get; set; } = null!;
        public List<ViewDocument> documents { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; }
    }
}
