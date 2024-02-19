namespace Hallo_Doc.Entity.ViewModel
{
    public class FamilyReq
    {
        public string? f_firstname {  get; set; }
        public string? f_lastname { get; set; }
        public string? f_mobile { get; set; }
        public string? f_email { get; set; }
        public string? f_relation { get; set; }
        public required string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? DOB { get; set; }
        public required string Email { get; set; }
        public string? Mobile { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
    }
}
