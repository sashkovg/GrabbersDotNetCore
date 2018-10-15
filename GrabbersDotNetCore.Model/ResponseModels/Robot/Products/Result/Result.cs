using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result
{
    /// <summary>
    /// Result section
    /// </summary>
    [DataContract]
    public class Result
    {
        public Result()
        {
            Products = new List<Robot.Products.Result.Product.Product>();
            Catalogs = new List<dynamic>();
        }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "catalogs")]
        public List<dynamic> Catalogs { get; set; }

        [DataMember(Name = "products")]
        public List<Robot.Products.Result.Product.Product> Products { get; set; }
    }
}
