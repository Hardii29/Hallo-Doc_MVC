using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Implementation;
using Microsoft.AspNetCore.Mvc;
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
        List<AdminDash> GetRequestData(int statusid, string searchValue);
        ViewCase GetView(int requestId);
        void UpdateViewCase(int requestId, ViewCase viewCase);
        bool CancelViewCase(int RequestId);
        List<CaseTag> GetReasons();
        List<Region> GetRegions();
        List<Physician> GetPhysician(int regionId);
        List<HealthProfessionalType> GetProfession();
        List<HealthProffessional> GetBusiness(int businessId);
        Order GetBusinessDetails(int VendorId);
        bool CancleCaseInfo(int? RequestId, int CaseTagId, string Notes);
        bool BlockCaseReq(int RequestId, string Notes);
        bool AssignCaseReq(int RequestId, int PhysicianId, string Notes);
        List<ViewDocument> GetFiles(int requestId);
        void UploadFiles(int requestId, ViewDocument viewDocument);
        IActionResult? DownloadFile(int fileID);
        void DeleteFile(int fileID);
        void DeleteAllFiles(int RequestId);
        bool TransferCaseReq(int RequestId, int PhysicianId, string Notes);
        bool ClearCaseReq(int RequestId);
        void SendAgreementEmail(string email);
        Order GetOrderView(int requestId);
        void SendOrder(Order order);
        ViewCase GetClearCaseView(int requestId);
        void UpdateCloseCase(int requestId, ViewCase viewCase);
        bool CloseCaseReq(int RequestId);

    }
}
