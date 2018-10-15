using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GrabbersDotNetCore.Model.RequestModel.Robot;
using GrabbersDotNetCore.Model.ResponseModels.Robot;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products;



namespace GtabbersDotNetCore.Bll.BllRepositories.Robots
{

    public interface IActionsFactory
    {
        Task<Response> RunAction(Request request, Response result);
    }
    public class ActionsFactory : IActionsFactory
    {
        private readonly IRobotsFactory _robotsFactory;

        public ActionsFactory(IRobotsFactory robotsFactory)
        {
            _robotsFactory = robotsFactory;
        }

        public async Task<Response> RunAction(Request request, Response response)
        {
            switch (request.Action.ToLower())
            {
                case "getproducts":
                    response = await _robotsFactory.GetRobotInstance(request).GetProducts();
                    break;
                default:
                    throw new Exception("Action does't exist");
            }
            return response;
        }
    }
}
