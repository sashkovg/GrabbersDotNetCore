using System.Runtime.Serialization;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Position.Supplier.Company
{
    /// <summary>
    /// Resources section
    /// </summary>
    [DataContract]
    public class Company
    {
        [DataMember(Name = "information")]
        public dynamic Information { get; set; }

        [DataMember(Name = "contacts")]
        public dynamic Contacts { get; set; }

    }
}
