using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Position
{
    /// <summary>
    /// Position section
    /// </summary>
    [DataContract]
    public class Position
    {
        public Position()
        {
            Prices = new List<PriceInfo.PriceInfo>();
            Description = string.Empty;
            MinQuota = 0;
            Increment = 0;
            Quantity = 0;
            Units = string.Empty;
            Currency = string.Empty;
            Suppliers = new List<Supplier.Supplier>();
        }

        [DataMember(Name = "minQuota")]
        public int MinQuota { get; set; }

        [DataMember(Name = "increment")]
        public int Increment { get; set; }

        [DataMember(Name = "quantity")]
        public int Quantity { get; set; }

        [DataMember(Name = "leadTime")]
        public int LeadTime { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "units")]
        public string Units { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "prices")]
        public List<PriceInfo.PriceInfo> Prices { get; set; }

        [DataMember(Name = "suppliers")]
        public List<Supplier.Supplier> Suppliers { get; set; }
    }
}
