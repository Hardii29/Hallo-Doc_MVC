using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Interface
{
    public interface IAdminDashboard
    {
        CountStatusWiseRequest CountRequestData();
        List<AdminDash> GetRequestData(int statusid);
        ViewCase GetView(int requestId);
        void UpdateViewCase(int requestId, ViewCase viewCase);
        List<CaseTag> GetReasons();
        List<Region> GetRegions();
        List<Physician> GetPhysician(int regionId);
        bool CancleCaseInfo(int? RequestId, string Notes, string caseTag);
        bool BlockCaseReq(int RequestId, string Notes);

    }
}
