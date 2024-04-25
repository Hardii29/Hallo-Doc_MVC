using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Interface
{
    public interface IAdminNavbar
    {
        List<Entity.Models.Region> GetRegions();
        ProviderMenu? ProviderMenu(int? regionId = null);
        void StopNotfy(int ProviderId);
        bool SendMailPhy(string email, string Message, string ProviderName);
        bool SendSMS(string Mobile, string Message, string ProviderName);
        AccountAccess Access();
        List<AspNetRole> GetNetRoles();
        UserAccess UserAccess(string AccountType);
        AccountAccess CreateAccess();
        List<Menu> GetMenuList(AccountType accountType);
        void CreateRole(AccountAccess access);
        AccountAccess ViewEditRole(int RoleId);
        bool SaveEditRole(AccountAccess roles);
        bool DeleteRole(int RoleId);
        Schedule Schedule();
        List<Physician> AllPhysician();
        List<Physician> PhysicianCalender(int? regionId);
        void CreateShift(Schedule schedule);
        List<Schedule> ShiftList();
        Schedule GetShiftDetails(int ShiftId);
        void EditShift(int ShiftId, int RegionId, int PhysicianId, DateOnly ShiftDate, TimeOnly StartTime, TimeOnly EndTime);
        void DeleteShift(int ShiftId);
        void ChangeStatus(int ShiftId);
        MDsOnCall MDsOnCall();
        RequestedShift RequestedShift(int? regionId);
        Task<bool> DeleteReqShift(string s);
        Task<bool> UpdateStatusShift(string s);
        VendorMenu VendorMenu(string searchValue, int Profession);
        VendorMenu AddBusiness();
        void AddVendor(VendorMenu model);
        VendorMenu EditVendor(int VendorId);
        void EditVendorInfo(int VendorId, VendorMenu model);
        bool DeleteBusiness(int VendorId);
        BlockHistory BlockedHistory(BlockHistory history);
        bool UnBlock(int reqId);
        UserData PatientHistory(UserData data);
        PatientData PatientRecord(int UserId);
        SearchRecordList SearchRecord(SearchRecordList sl);
        bool RecordsDelete(int RequestId);
        ProviderLocation ProviderLocation();
        Logs EmailLog(Logs el);
        SMSLog SMSLog(SMSLog sl);
        AdminProfile CreateAdmin();
        void AddAdmin(AdminProfile admin);

    }
}
