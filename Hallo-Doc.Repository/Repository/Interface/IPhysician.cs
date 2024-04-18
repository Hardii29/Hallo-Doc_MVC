﻿using Hallo_Doc.Entity.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hallo_Doc.Repository.Repository.Interface
{
    public interface IPhysician
    {
        StatusCount CountRequest(int PhysicianId);
        PaginationModel<PhysicianDash> GetRequestData(int statusid, int PhysicianId, int page, int pagesize);
        bool AcceptCase(int RequestId, int PhysicianId);
        bool TransferCase(int RequestId, string Notes);
        bool Housecall(int RequestId);
        bool Consult(int RequestId);
        bool ConcludeCare(int RequestId, string Notes);
        void CreateReq(PatientReq req, int PhysicianId);
    }
}
