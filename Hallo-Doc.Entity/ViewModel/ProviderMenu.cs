using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class ProviderMenu
    {
        public List<ProviderMenu>? data;

        public string? AdminName { get; set; }
        public int AdminId { get; set; } = 0;
        public string? ProviderName { get; set; }
        public int ProviderId { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set;}
        public short? Status { get; set; }
        public string? StatusName { get; set; }
        public string? Region { get; set; }
        public int? RegionId { get; set; }
    }
}
