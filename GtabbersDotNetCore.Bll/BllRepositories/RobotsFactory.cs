using System;
using System.Collections.Generic;
using System.Text;
using GrabbersDotNetCore.Model.RequestModel.Robot;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products;
using GtabbersDotNetCore.Bll.BllRepositories.Robots;
using GtabbersDotNetCore.Bll.Interfaces;

namespace GtabbersDotNetCore.Bll.BllRepositories
{ 
    public interface IRobotsFactory
    {
        IRobotBase GetRobotInstance(Request request);
    }

    public class RobotsFactory : IRobotsFactory
    {
        public IRobotBase GetRobotInstance(Request request)
        {
            IRobotBase robot;
            switch (request.Robot.ToLower())
            {
                case "digikey":
                    robot = new DigikeyRobot(request.Data, request.Robot.ToLower());
                    break;
                default:
                    throw new Exception($"{request.Robot} doesn't exist");
            }
            return robot;
        }
    }
}
