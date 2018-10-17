using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GrabbersDotNetCore.Model.RequestModel;
using GrabbersDotNetCore.Model.RequestModel.Robot;
using GrabbersDotNetCore.Model.ResponseModels.Robot;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product;
using GtabbersDotNetCore.Bll.Helpers;
using GtabbersDotNetCore.Bll.Interfaces;
using OpenQA.Selenium;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Diagnostics;

namespace GtabbersDotNetCore.Bll.BllRepositories.Robots.RobotBase
{
    public abstract class RobotBase : DriverFactory, IRobotBase
    {
        protected string PartNumber;
        protected short Offset;
        protected short Limit;
        protected bool List;
        protected bool Deep;
        protected string Location;
        protected string Catalog;
        protected string Currency;
        protected object Account;
        protected string Url;
        protected Response Response;
        protected short ItemsOnPage = 0;
        protected string Domain;
      
        protected RobotBase(Params request, string robot) : this(robot)
        {
           
            switch (request)
            {
                case ProductsSearch _:
                    PartNumber = ((ProductsSearch)request).PartNumber;
                    Offset = ((ProductsSearch)request).Offset;
                    Limit = ((ProductsSearch)request).Limit;
                    List = ((ProductsSearch)request).List;
                    Deep = ((ProductsSearch)request).Deep;
                    Location = ((ProductsSearch)request).Location;
                    Catalog = ((ProductsSearch)request).Catalog;
                    Currency = ((ProductsSearch)request).Currency;
                    Account = ((ProductsSearch)request).Account;
                    break;
                case ProductSearch _:
                    Url = ((ProductSearch)request).Url;
                    break;
            }

            Response = new Response();
        }

        private RobotBase(string robot)
        {
            Initialize(robot);

        }

        private HtmlDocument _dom;

        protected HtmlDocument Dom
        {
            get
            {
                const string regex = "(\\<!--.*?\\-->)";

                if (string.IsNullOrEmpty(_dom?.DocumentNode.InnerText))
                {
                    _dom = new HtmlDocument();
                    _dom.LoadHtml(Regex.Replace(
                        Driver.WaitUntilElementExists(By.CssSelector("body")).GetAttribute("innerHTML"), regex, ""));
                    return _dom;
                }
                return _dom;

            }
            set => _dom = value;
        }

        public async Task<Response> GetProducts(bool skipSearch = false)
        {
            try
            {
                Helper.TypeOfSearchResult type = await Task.FromResult(GetTypeOfSearchResult(skipSearch));
                List<Product> data = new List<Product>();
                switch (type)
                {
                    case Helper.TypeOfSearchResult.Products:
                        if (ItemsOnPage == 0)
                            throw new Exception("The number of ItemsOnPage must be set");
                        short startPage = (short)(Math.Floor((double)Offset / ItemsOnPage) + 1);
                        short endPage = (short)(Math.Ceiling(((double)(Offset + Limit) / ItemsOnPage)) + 1);
                        for (short page = startPage;
                            page < endPage;
                            page++)
                        {
                            Dom = null;
                            data = await GetInfoAboutProducts(page);
                            if (!data.Any())
                                break;
                            Response.Result.Products.AddRange(data);
                        }
                        Response.Result.Products = Response.Result.Products.Skip(Offset % ItemsOnPage).Take(Limit).ToList();
                        if (Deep)
                        {
                            List<Product> deepResult = new List<Product>();
                            foreach(var product in Response.Result.Products)
                            {
                                Dom = null;
                                List<Product> deepProduct =  GetInfoAboutProduct(product.Catalog.Resources.First(x=>x.Type=="source").Uri);
                                deepResult.AddRange(deepProduct);
                            }
                            Response.Result.Products = deepResult;
                        }

                        break;

                    case Helper.TypeOfSearchResult.Product:
                        data =  GetInfoAboutProduct();
                        Response.Result.Products.AddRange(data);
                        break;

                    case Helper.TypeOfSearchResult.Catalogs:

                        break;
                    case Helper.TypeOfSearchResult.NotFound:
                        Response.Result.Type = "not found";
                        break;
                }
                if (Response.Result.Products.Any())
                {
                    Response.Result.Type = "products";
                }
                return Response;
            }
            catch (NoSuchElementException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                Quit();
            }
        }

        public virtual Helper.TypeOfSearchResult GetTypeOfSearchResult(bool skipSearch = false)
        {
            throw new NotImplementedException();
        }

        public virtual Task<List<Product>> GetInfoAboutProducts(short page)
        {
            throw new NotImplementedException();
        }

        public virtual List<Product> GetInfoAboutProduct(string url = null)
        {
            throw new NotImplementedException();
        }


        public virtual async Task<Response> GetProductInfoBase()
        {
            try
            {
                Response.Result.Products =  GetInfoAboutProduct(Url);
                return Response;
            }
            catch (NoSuchElementException ex)
            {
                throw new NoSuchElementException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Quit();
            }
        }
        public void TakeScreenShot(string name)
        {
            string path = System.IO.Path.GetDirectoryName((System.Reflection.Assembly.GetExecutingAssembly().Location)) + "/ScreenShots/"; // your code goes here
            bool exists = System.IO.Directory.Exists(path);
            if (!exists)
                System.IO.Directory.CreateDirectory(path);
            Screenshot screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
            screenshot.SaveAsFile(path + name, ScreenshotImageFormat.Png);
        }
    }
}
