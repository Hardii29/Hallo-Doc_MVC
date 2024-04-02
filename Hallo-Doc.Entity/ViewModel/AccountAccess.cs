using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class AccountAccess
    {
        public List<AccountAccess>? data;
        public int AdminId { get; set; }
        public string? AdminName { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set;}
        public short Accounttype { get; set; }
        public AccountType SelectedType { get; set; }
        public List<int> SelectMenu { get; set; }
    }
    public enum AccountType
    {
        Admin = 1,
        Physician = 2,
        Patient = 3,
        All = 4
    }
}
