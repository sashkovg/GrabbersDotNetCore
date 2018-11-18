using GrabbersDotNetCore.Model.RequestModel.Robot;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Catalog.Resource;
using GtabbersDotNetCore.Bll.Helpers;
using HtmlAgilityPack.CssSelectors.NetCore;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GtabbersDotNetCore.Bll.BllRepositories.Robots
{
    public class FarnellRobot : RobotBase.RobotBase
    {
        private string urlForPaging = string.Empty;
        public FarnellRobot(Params request, string robot) : base(request, robot)
        {
            ItemsOnPage = 25;
            Domain = "https://uk.farnell.com/";
        }

        public override Helper.TypeOfSearchResult GetTypeOfSearchResult(bool skipSearch = false)
        {
            try
            {
                Driver.Navigate().GoToUrl($"https://uk.farnell.com/search?st={PartNumber}");
                if (Driver.IsElementExist(By.CssSelector(".categoryContainer a")))
                {
                    var link = Driver.FindElement(By.CssSelector(".categoryContainer a")).GetAttribute("href");
                    Driver.Quit();
                    Initialize("farnell");
                    Driver.Navigate().GoToUrl(link);
                }

                if (Driver.IsElementExist(By.CssSelector("#sProdList tbody tr")))
                {
                    return Helper.TypeOfSearchResult.Products;
                }
                else if (Driver.IsElementExist(By.CssSelector("#product")))
                {
                    return Helper.TypeOfSearchResult.Product;
                }
              
            }
            catch (NoSuchElementException)
            {
                TakeScreenShot($"farnell_error_{DateTime.Now.Millisecond}");
                throw;
            }
            catch (Exception)
            {
                TakeScreenShot($"farnell_error_{DateTime.Now.Millisecond}");
                throw;
            }
            return Helper.TypeOfSearchResult.NotFound;
        }

        public override Task<List<Product>> GetInfoAboutProducts(short page)
        {
            List<Product> result = new List<Product>();
            try
            {
                if (page > 1)
                {
                    string currentPageUrl = string.Empty;
                    if (urlForPaging.IndexOf("st=") < 0)
                    {

                    }
                    else
                    {
                        int indexOfSearch = urlForPaging.IndexOf("?st=");
                        currentPageUrl = urlForPaging.Insert(indexOfSearch, $"/results/{page}");
                    }
                    Driver.Quit();
                    Initialize("farnell");
                    Driver.Navigate().GoToUrl(currentPageUrl);
                }
                else
                {
                    urlForPaging = Driver.Url;
                }
                if (Dom.QuerySelector("#sProdList tbody tr") != null)
                {
                    var labels = Dom.QuerySelectorAll("#sProdList thead tr:nth-child(1) th").ToList();
                    var rows = Dom.QuerySelectorAll("#sProdList tbody tr").ToList();
                    foreach (var webElement in rows)
                    {
                        Product product = new Product();
                        try
                        {
                            for (int i = 1; i < labels.Count; i++)
                            {
                                string label = labels[i].InnerText.NormalizeStr().ToLower();
                                bool isCustomField = true;// признак того, что свойство необходимо внести в объект customField
                                if (labels[i].Attributes["class"].Value.Contains("datasheet") || label.Equals("datasheet"))
                                {
                                    if (webElement.QuerySelector($"td:nth-child({i + 1}) > a") != null)
                                    {
                                        var datasheetLink = webElement.QuerySelector($"td:nth-child({i + 1}) > a").Attributes["href"].Value;
                                        var title = webElement.QuerySelector($"td:nth-child({i + 1}) > a > img").Attributes["title"]?.Value;
                                        if (datasheetLink.Substring(0, 2) == "//")
                                        {
                                            datasheetLink = datasheetLink.Substring(2);
                                        }
                                        product.Catalog.Resources.Add(new Resource()
                                        {
                                            Type = "document",
                                            Uri = datasheetLink,
                                            Attributes = new Dictionary<string, object>()
                                            {
                                                { "mime", "application/pdf"},
                                                { "datasheet", true},
                                                { "title", title},
                                            }
                                        });
                                    }
                                    isCustomField = false;

                                }

                                if (webElement.QuerySelector($"td:nth-child({i+1})").Attributes["class"].Value.Contains("productImage"))
                                {
                                    if (webElement.QuerySelector($"td:nth-child({i + 1}) img") != null)
                                    {
                                        var imageLink = webElement.QuerySelector($"td:nth-child({i + 1}) img").Attributes["src"].Value;
                                        var title = webElement.QuerySelector($"td:nth-child({i + 1}) img").Attributes["title"]?.Value;
                                        if (imageLink.Substring(0, 2) == "//")
                                        {
                                            imageLink = imageLink.Substring(2);
                                        }
                                        product.Catalog.Resources.Add(new Resource()
                                        {
                                            Type = "image",
                                            Uri = imageLink,
                                            Attributes = new Dictionary<string, object>()
                                            {
                                                { "mime", "application/jpeg"},
                                                { "preview", true},
                                                { "title", title},
                                            }
                                        });
                                    }
                                    isCustomField = false;
                                }

                                if (webElement.QuerySelector($"td:nth-child({i + 1})").Attributes["class"].Value.Contains("mftrPart"))
                                {
                                    if (webElement.QuerySelector($"td:nth-child({i + 1}) a") != null)
                                    {
                                        var pn = webElement.QuerySelector($"td:nth-child({i + 1}) a").InnerText.NormalizeStr();
                                        var pnLink = webElement.QuerySelector($"td:nth-child({i + 1}) a").Attributes["href"].Value;
                                        if (pnLink.Substring(0, 2) == "//")
                                        {
                                            pnLink = pnLink.Substring(2);
                                        }
                                        product.Catalog.ProductLine = product.Catalog.PartNumber = pn;
                                        product.Catalog.Resources.Add(new Resource()
                                        {
                                            Type = "source",
                                            Uri = Domain + pnLink,
                                            Attributes = new Dictionary<string, object>()
                                            {
                                                { "information", new string[]{ "catalog", "stock" } }
                                            }
                                        });
                                    }
                                    isCustomField = false;
                                }

                               

                                if ((webElement.QuerySelector($"td:nth-child({i + 1})").Attributes["class"].Value.Contains("description")))
                                {
                                    var vendor = webElement.QuerySelector($"td:nth-child({i + 1}) input.hVal");
                                    //product.Catalog.Manufacturer = vendor;
                                    isCustomField = false;
                                }

                                //if (labels[i].Attributes["class"].Value.Contains("description") || label.Equals("description"))
                                //{
                                //    var descr = webElement.QuerySelector($"td:nth-child({i + 1})").InnerText.NormalizeStr();
                                //    product.Catalog.Description = descr;
                                //    isCustomField = false;
                                //}

                                //if (labels[i].Attributes["class"].Value.Contains("qtyAvailable") || label.Equals("quantity available"))
                                //{
                                //    if (webElement.QuerySelector($"td:nth-child({i + 1}) span.desktop") != null)
                                //    {
                                //        string quantityString = Regex.Replace(webElement.QuerySelector($"td:nth-child({i + 1}) span.desktop").InnerText, "[^0-9.]", "");
                                //        if (!string.IsNullOrWhiteSpace(quantityString))
                                //        {
                                //            product.Position.Quantity = Int32.Parse(quantityString);
                                //        }
                                //    }


                                //    isCustomField = false;
                                //}

                                //if (labels[i].Attributes["class"].Value.Contains("unitPrice") || label.Equals("unit price"))
                                //{
                                //    string price = Regex.Replace(webElement.QuerySelector($"td:nth-child({i + 1})").InnerText, "[^0-9.]", "");
                                //    if (!string.IsNullOrWhiteSpace(price))
                                //    {
                                //        product.Position.Prices.Add(new GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Position.PriceInfo.PriceInfo()
                                //        {
                                //            From = 1,
                                //            Price = decimal.Parse(price)
                                //        });
                                //    }
                                //    isCustomField = false;
                                //}

                                //if (labels[i].Attributes["class"].Value.Contains("minQty") || label.Equals("minimum quantity "))
                                //{
                                //    if (webElement.QuerySelector($"td:nth-child({i + 1}) span.desktop") != null)
                                //    {
                                //        string quantityString = Regex.Replace(webElement.QuerySelector($"td:nth-child({i + 1}) span.desktop").InnerText, "[^0-9.]", "");
                                //        if (!string.IsNullOrWhiteSpace(quantityString))
                                //        {
                                //            product.Position.MinQuota = Int32.Parse(quantityString);
                                //        }

                                //    }

                                //    isCustomField = false;
                                //}
                                //if (isCustomField)
                                //{
                                //    string value = webElement.QuerySelector($"td:nth-child({i + 1})").InnerText.NormalizeStr();
                                //    product.Catalog.CustomFields[label] = value;
                                //}

                            }
                        }
                        catch (Exception e)
                        {
                            throw;
                        }
                        //product.Position.Currency = Dom.QuerySelector($"div.locale--lang-cur > span:last-child").InnerText.NormalizeStr();
                        result.Add(product);
                    }
                }
            }
            catch (NoSuchElementException)
            {
                TakeScreenShot($"farnell_error_{DateTime.Now.Millisecond}");
                throw;
            }
            catch (Exception)
            {
                TakeScreenShot($"farnell_error_{DateTime.Now.Millisecond}");
                throw;
            }
            return Task.FromResult(result);
        }
        public override List<Product> GetInfoAboutProduct(string url = null)
        {
            if (url != null)
            {
                Driver.Navigate().GoToUrl(url);
            }

            List<Product> result = new List<Product>();
            try
            {

                Product product = new Product();
                var page = Dom.QuerySelector("#content");
                result.Add(product);
            }
            catch (NoSuchElementException)
            {
                TakeScreenShot($"farnell_error_{DateTime.Now.Millisecond}");
                throw;
            }
            catch (Exception)
            {
                TakeScreenShot($"farnell_error_{DateTime.Now.Millisecond}");
                throw;
            }
            return result;
        }

    }
}
