using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products
{
    /// <summary>
    /// Grabber's response model
    /// </summary>
    [DataContract]
    public class Response
    {
        public Response()
        {
            Meta = new Meta.Meta();
            Result = new Result.Result();
            Errors = new List<string>();
        }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "errors")]
        public List<string> Errors { get; set; }

        [DataMember(Name = "meta")]
        public Meta.Meta Meta { get; set; }

        [DataMember(Name = "result")]
        public Result.Result Result { get; set; }
    }
}
