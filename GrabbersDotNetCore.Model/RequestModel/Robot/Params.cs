using System;
using System.Collections.Generic;
using System.Text;

namespace GrabbersDotNetCore.Model.RequestModel.Robot
{
    public class Params
    {
    }

    public class ProductsSearch : Params
    {
        public string PartNumber { get; set; }
        public short Offset { get; set; }
        public short Limit { get; set; }
        public bool List { get; set; }
        public bool Deep { get; set; }
        public string Location { get; set; }
        public string Catalog { get; set; }
        public string Currency { get; set; }
        public object Account { get; set; }
    }

    public class DistributorSearch : Params
    {
    }

    public class ProductSearch : Params
    {
        public string Url { get; set; }
    }
}
