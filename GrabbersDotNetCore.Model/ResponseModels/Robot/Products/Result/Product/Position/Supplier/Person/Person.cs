using System.Runtime.Serialization;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Position.Supplier.Person
{
    /// <summary>
    /// Person section
    /// </summary>
    [DataContract]
    public class Person
    {
        [DataMember(Name = "personal")]
        public dynamic Personal { get; set; }

        [DataMember(Name = "contacts")]
        public dynamic Contacts { get; set; }

    }
}
