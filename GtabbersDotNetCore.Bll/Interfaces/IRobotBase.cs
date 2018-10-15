using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GrabbersDotNetCore.Model.ResponseModels.Robot;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products;

namespace GtabbersDotNetCore.Bll.Interfaces
{
    public interface IRobotBase
    {
        Task<Response> GetProducts( bool skipSearch = false);
    }
}
