using Hallo_Doc.Entity.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Interface
{
    public interface IPatientUser
    {
        DashboardList PatientDashboard(HttpContext httpContext);
        ViewDocument ViewDocument(int fileId, HttpContext httpContext);
        IActionResult DownloadFile(int fileID);
        PatientProfile? Edit(int userId, HttpContext httpContext);
        void EditProfile(int userId, PatientProfile patientProfile);
    }
}
