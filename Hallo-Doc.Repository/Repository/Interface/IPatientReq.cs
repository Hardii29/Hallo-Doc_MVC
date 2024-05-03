using Hallo_Doc.Entity.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Interface
{
    public interface IPatientReq
    {
        public bool checkEmail(string email);
        List<Entity.Models.Region> GetRegions();
        void AddDetails(PatientReq patientReq);
        void FamilyDetails(FamilyReq familyReq);
        void ConciergeDetails(ConciergeReq conciergeReq);
        void BusinessDetails(BusinessReq businessReq);
    }
}
