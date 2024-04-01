using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using Hallo_Doc.Repository.Repository.Implementation;
using Microsoft.AspNetCore.Http;
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
        PaginatedViewModel<AdminDash> GetRequestData(int statusid, string searchValue, int page, int pagesize, int? Region, string sortColumn, string sortOrder, int? requesttype);
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
        void SendAgreementEmail(string email, int RequestId);
        bool SendAgreement_accept(int RequestID);
        bool SendAgreement_Reject(int RequestID, string Notes);
        Order GetOrderView(int requestId);
        void SendOrder(Order order);
        ViewCase GetClearCaseView(int requestId);
        void UpdateCloseCase(int requestId, ViewCase viewCase);
        bool CloseCaseReq(int RequestId);
        AdminProfile? Profile(int adminId);
        void EditProfile(int adminId, AdminProfile profile);
        PatientReq? Admin();
        void CreateReq(PatientReq req);
        List<AdminDash> Export(string status, int? Region, int? requesttype);
        void SendLink(string email, string firstName, string lastName);
        ProviderMenu? ProviderMenu(int? regionId = null);
        void StopNotfy(int ProviderId);
        void SendMailPhy(string email, string Message, string ProviderName);
        AccountAccess Access();
        UserAccess UserAccess();
        AccountAccess CreateAccess();
    }
}
