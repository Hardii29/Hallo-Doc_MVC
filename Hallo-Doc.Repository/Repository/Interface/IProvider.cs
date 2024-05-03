using Hallo_Doc.Entity.Models;
using Hallo_Doc.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Interface
{
    public interface IProvider
    {
        Provider CreateProvider();
        List<Region> GetRegions();
        List<Role> GetRoles();
        void AddProvider(Provider provider);
        Provider? PhysicianAccount(int ProviderId);
        void EditPrAccount(Provider pr);
        void EditPrInfo(Provider pr);
        void EditPrBilling(Provider pr);
        void EditPrbusiness(Provider pr);
        void EditOnbording(Provider pr);
        void DeletePrAccount(int ProviderId);
        PhysicianPayrate GetPayRate(int PhysicianId);
        public void EditPayrate(int PhysicianId, string category, string NightShift_Weekend, string Shift, string HouseCalls_Nights_Weekend, string PhoneConsults, string PhoneConsults_Nights_Weekend, string BatchTesting, string HouseCalls);

    }
}
