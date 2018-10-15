using System;
using System.Collections.Generic;
using System.Text;

namespace GrabbersDotNetCore.Model.RequestModel.Robot
{
    public class Request
    {
        public Request()
        {
        }
        public string Robot { get; set; }
        private string _action;
        public Params Data { get; set; }

        public string Action
        {
            get => _action;
            set
            {
                _action = value;
                switch (_action.ToLower())
                {
                    case "getproducts":
                        Data = new ProductsSearch();
                        break;
                    case "getproductinfobase":
                    case "getproductinfostock":
                        Data = new ProductSearch();
                        break;
                    default:
                        throw new Exception("Action doesn't exist");
                }
            }
        }

       
    }
}
