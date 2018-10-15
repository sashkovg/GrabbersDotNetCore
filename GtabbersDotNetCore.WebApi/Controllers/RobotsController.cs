using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GrabbersDotNetCore.Model.RequestModel.Robot;
using GrabbersDotNetCore.Model.ResponseModels.Robot;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products;
using GtabbersDotNetCore.Bll.BllFactory;
using GtabbersDotNetCore.Bll.BllRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace GtabbersDotNetCore.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class RobotsController : Controller
    {
        private readonly IBllFactory _bllFactory;
        private readonly ILogger _logger;

        public RobotsController(IBllFactory factoryBll, ILogger<RobotsController> logger)
        {
            _bllFactory = factoryBll ?? throw new ArgumentNullException(nameof(factoryBll));
            _logger = logger;
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody]JObject request)
        {
            Stopwatch timer = new Stopwatch();
            Response result = new Response();
            timer.Start();
            try
            {
                Request req = request.ToObject<Request>();
                result = await _bllFactory.ActionsFactory.RunAction(req, result);
                result.Status = "ok";
            }
            catch (Exception ex)
            {
                result.Status = "error";
                result.Errors.Add($"Message:{ex.Message.Trim()}; StackTrace: {ex.StackTrace.ToString()}");
                _logger.LogError($"Request: {request}; Message: {ex.Message.Trim()}; StackTrace: {ex.StackTrace.ToString()}");
            }
            finally
            {
                timer.Stop();
                result.Meta.ExecutionTime = timer.Elapsed.ToString();
            }

            return Ok(result);
        }
    }
}