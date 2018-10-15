using System.Runtime.Serialization;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Position.PriceInfo
{
    /// <summary>
    /// PriceInfo section
    /// </summary>
    [DataContract]
    public class PriceInfo
    {
        [DataMember(Name = "price")]
        public decimal Price { get; set; }

        [DataMember(Name = "from")]
        public int From { get; set; }

    }
}
