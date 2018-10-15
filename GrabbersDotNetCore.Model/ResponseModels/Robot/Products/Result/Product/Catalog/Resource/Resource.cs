using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.Serialization;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Catalog.Resource
{
    /// <summary>
    /// Resources section
    /// </summary>
    [DataContract]
    public class Resource
    {

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }

        [DataMember(Name = "attrs")]
        public Dictionary<string,object> Attributes { get; set; }
    }
}
