using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Entity.ViewModel
{
    public class UserAccess
    {
        public List<UserAccess>? data;
        public int AdminId { get; set; }
        public string AdminName { get; set; }
        public string UserName { get; set; }
        public string AccountType { get; set; }
        public short Status { get; set; }
        public int RequestCount { get; set; }
        public string Phone {  get; set; }
        public int Id { get; set; }
    }
    public enum state
    {
        Active = 4,
        Pending = 2,
    }
}
