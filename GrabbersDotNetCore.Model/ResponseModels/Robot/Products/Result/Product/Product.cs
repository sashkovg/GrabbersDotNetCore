using System.Runtime.Serialization;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product
{
    /// <summary>
    /// Product section
    /// </summary>
    [DataContract]
    public class Product
    {
        public Product()
        {
            Catalog = new Catalog.Catalog();
            Position = new Position.Position();
        }

        [DataMember(Name = "catalog")]
        public Catalog.Catalog Catalog { get; set; }

        [DataMember(Name = "position")]
        public Position.Position Position { get; set; }
    }

}
