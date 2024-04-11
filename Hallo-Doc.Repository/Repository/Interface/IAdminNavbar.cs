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
        void SendMailPhy(string email, string Message, string ProviderName);
        AccountAccess Access();
        UserAccess UserAccess();
        AccountAccess CreateAccess();
        List<Menu> GetMenuList(AccountType accountType);
        void CreateRole(AccountAccess access);
        Schedule Schedule();
        List<Physician> AllPhysician();
        List<Physician> PhysicianCalender(int? regionId);
        void CreateShift(int RegionId, int PhysicianId, DateOnly ShiftDate, TimeOnly StartTime, TimeOnly EndTime);
        List<Schedule> ShiftList();
        Schedule GetShiftDetails(int ShiftId);
        void EditShift(int ShiftId, int RegionId, int PhysicianId, DateOnly ShiftDate, TimeOnly StartTime, TimeOnly EndTime);
        void DeleteShift(int ShiftId);
        MDsOnCall MDsOnCall();
        RequestedShift RequestedShift();
        VendorMenu VendorMenu(string searchValue, int Profession);
        VendorMenu AddBusiness();
        void AddVendor(VendorMenu model);
        VendorMenu EditVendor(int VendorId);
        void EditVendorInfo(int VendorId, VendorMenu model);
        bool DeleteBusiness(int VendorId);
        BlockHistory BlockedHistory(BlockHistory history);
        bool UnBlock(int reqId);
        UserData PatientHistory(string fname, string lname, string email, string phone);
        PatientData PatientRecord(int UserId);
        SearchRecordList SearchRecord(SearchRecordList sl);
        bool RecordsDelete(int RequestId);


    }
}
