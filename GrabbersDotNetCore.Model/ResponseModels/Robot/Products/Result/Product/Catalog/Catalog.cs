using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Catalog
{
    /// <summary>
    /// Catalog section
    /// </summary>
    [DataContract]
    public class Catalog
    {
        public Catalog()
        {
            CustomFields = new Dictionary<string, string>();
            Path = new List<string>();
            Resources = new List<Resource.Resource>();
            Modules = new List<string>();
            DateCode = string.Empty;
            Manufacturer = string.Empty;
            Description = string.Empty;
            ProductLine = string.Empty;
            PartNumber = string.Empty;
        }

        [DataMember(Name = "productLine")]
        public string ProductLine { get; set; }

        [DataMember(Name = "partNumber")]
        public string PartNumber { get; set; }

        [DataMember(Name = "manufacturer")]
        public string Manufacturer { get; set; }

        [DataMember(Name = "path")]
        public List<string> Path { get; set; }

        [DataMember(Name = "customFields")]
        public Dictionary<string, string> CustomFields { get; set; }

        [DataMember(Name = "modules")]
        public List<string> Modules { get; set; }

        [DataMember(Name = "dateCode")]
        public string DateCode { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }


        [DataMember(Name = "resources")]
        public List<Resource.Resource> Resources { get; set; }

    }
}
