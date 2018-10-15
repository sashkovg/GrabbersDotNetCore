using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Catalog;

namespace GtabbersDotNetCore.Bll.Interfaces
{
    public interface IRobot : IRobotBase
    {
        List<Catalog> GetCatalogsFromPage();
        Task<List<Product>> GetInfoAboutProductsList(int page);
        string GetTypeOfSearchResult(bool skipSearch = false);
        Task<List<Product>> GetInfoAboutProduct(Product item, int position = 0, string url = null);

    }
}
