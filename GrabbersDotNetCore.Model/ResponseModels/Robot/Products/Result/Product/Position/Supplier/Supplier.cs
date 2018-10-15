using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Position.Supplier
{
    /// <summary>
    /// Resources section
    /// </summary>
    [DataContract]
    public class Supplier
    {
        [DataMember(Name = "company")]
        public Company.Company Company { get; set; }

        [DataMember(Name = "persons")]
        public List<Person.Person> Persons { get; set; }

    }
}
