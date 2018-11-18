using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GrabbersDotNetCore.Model.RequestModel;
using GrabbersDotNetCore.Model.RequestModel.Robot;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Catalog;
using GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Catalog.Resource;
using GtabbersDotNetCore.Bll.Helpers;
using OpenQA.Selenium;
using HtmlAgilityPack.CssSelectors.NetCore;
using OpenQA.Selenium.Support.UI;

namespace GtabbersDotNetCore.Bll.BllRepositories.Robots
{
    public class DigikeyRobot : RobotBase.RobotBase
    {
        private string urlForPaging = string.Empty;
        public DigikeyRobot(Params request, string robot) : base(request, robot)
        {
            ItemsOnPage = 25;
            Domain = "https://www.digikey.com";
        }

        public override Helper.TypeOfSearchResult GetTypeOfSearchResult(bool skipSearch = false)
        {
            try
            {
                Driver.Navigate().GoToUrl(Domain);
                IWebElement element = Driver.FindElement(By.CssSelector("#content .searchbox-inner .searchbox-inner-searchtext input[name=keywords]"));
                element.SendKeys(PartNumber);
                Thread.Sleep(500);
                IWebElement searchButton = Driver.FindElement(By.CssSelector("#content .searchbox-inner #header-search-button"));
                searchButton.Click();
                
                if (Driver.IsElementExist(By.Id("qpLinkList")))
                {
                    var link = Driver.FindElement(By.CssSelector("#qpLinkList a"));
                    link.Click();
                }
                else if (Driver.IsElementExist(By.CssSelector(".topResultsSection table td")))
                {
                    var link = Driver.FindElement(By.CssSelector(".topResultsSection table td a"));
                    link.Click();
                }
                else if (Driver.IsElementExist(By.Id("productIndexList")))
                {
                    var link = Driver.FindElement(By.CssSelector("#productIndexList a"));
                    link.Click();
                }   
                if (Driver.IsElementExist(By.CssSelector("#productTable")))
                {
                    return Helper.TypeOfSearchResult.Products;
                }
                else if (Driver.IsElementExist(By.CssSelector("#pdp_content")))
                {
                    return Helper.TypeOfSearchResult.Product;
                }
            }
            catch (NoSuchElementException)
            {
                TakeScreenShot($"digikey_error_{DateTime.Now.Millisecond}");
                throw;
            }
            catch (Exception)
            {
                TakeScreenShot($"digikey_error_{DateTime.Now.Millisecond}");
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
                    if(urlForPaging.IndexOf("k=") < 0)
                    {

                    }
                    else
                    {
                        int indexOfSearch = urlForPaging.IndexOf("?k=");
                        currentPageUrl = urlForPaging.Insert(indexOfSearch, $"/page/{page}");
                    }
                   
                    Driver.Navigate().GoToUrl(currentPageUrl);
                }
                else
                {
                    urlForPaging = Driver.Url;
                }
                if (Dom.QuerySelector("#lnkPart") != null)
                {
                    var labels = Dom.QuerySelectorAll("#tblhead tr:nth-child(1) th").ToList();
                    var rows = Dom.QuerySelectorAll("#lnkPart tr").ToList();
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

                                if (labels[i].Attributes["class"].Value.Contains("image") || label.Equals("image"))
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

                                if (labels[i].Attributes["class"].Value.Contains("dkPartNumber") || label.Equals("digi-key part number"))
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

                                if (labels[i].Attributes["class"].Value.Contains("mfgPartNumber") || label.Equals("manufacturer part number"))
                                {
                                    var pnSuplier = webElement.QuerySelector($"td:nth-child({i + 1})").InnerText.NormalizeStr();
                                    product.Catalog.CustomFields["suplierPartNumber"] = pnSuplier;
                                    isCustomField = false;
                                }

                                if (labels[i].Attributes["class"].Value.Contains("vendor") || label.Equals("manufacturer"))
                                {
                                    var vendor = webElement.QuerySelector($"td:nth-child({i + 1})").InnerText.NormalizeStr();
                                    product.Catalog.Manufacturer = vendor;
                                    isCustomField = false;
                                }

                                if (labels[i].Attributes["class"].Value.Contains("description") || label.Equals("description"))
                                {
                                    var descr = webElement.QuerySelector($"td:nth-child({i + 1})").InnerText.NormalizeStr();
                                    product.Catalog.Description = descr;
                                    isCustomField = false;
                                }

                                if (labels[i].Attributes["class"].Value.Contains("qtyAvailable") || label.Equals("quantity available"))
                                {
                                    if (webElement.QuerySelector($"td:nth-child({i + 1}) span.desktop") != null)
                                    {
                                        string quantityString = Regex.Replace(webElement.QuerySelector($"td:nth-child({i + 1}) span.desktop").InnerText, "[^0-9.]", "");
                                        if (!string.IsNullOrWhiteSpace(quantityString))
                                        {
                                            product.Position.Quantity = Int32.Parse(quantityString);
                                        }
                                    }
                                  

                                    isCustomField = false;
                                }

                                if (labels[i].Attributes["class"].Value.Contains("unitPrice") || label.Equals("unit price"))
                                {
                                    string price = Regex.Replace(webElement.QuerySelector($"td:nth-child({i + 1})").InnerText, "[^0-9.]", "");
                                    if (!string.IsNullOrWhiteSpace(price))
                                    {
                                        product.Position.Prices.Add(new GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Position.PriceInfo.PriceInfo()
                                        {
                                            From = 1,
                                            Price = decimal.Parse(price)
                                        });
                                    }
                                    isCustomField = false;
                                }

                                if (labels[i].Attributes["class"].Value.Contains("minQty") || label.Equals("minimum quantity "))
                                {
                                    if(webElement.QuerySelector($"td:nth-child({i + 1}) span.desktop") != null)
                                    {
                                        string quantityString = Regex.Replace(webElement.QuerySelector($"td:nth-child({i + 1}) span.desktop").InnerText, "[^0-9.]", "");
                                        if (!string.IsNullOrWhiteSpace(quantityString))
                                        {
                                            product.Position.MinQuota = Int32.Parse(quantityString);
                                        }

                                    }

                                    isCustomField = false;
                                }
                                if (isCustomField)
                                {
                                    string value = webElement.QuerySelector($"td:nth-child({i + 1})").InnerText.NormalizeStr();
                                    product.Catalog.CustomFields[label] = value;
                                }

                            }
                        }
                        catch (Exception e)
                        {
                            throw;
                        }
                        product.Position.Currency = Dom.QuerySelector($"div.locale--lang-cur > span:last-child").InnerText.NormalizeStr();
                        result.Add(product);
                    }
                }
            }
            catch (NoSuchElementException)
            {
                TakeScreenShot($"digikey_error_{DateTime.Now.Millisecond}");
                throw;
            }
            catch (Exception)
            {
                TakeScreenShot($"digikey_error_{DateTime.Now.Millisecond}");
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

                if (page.QuerySelector($"#product-photo") != null)
                {
                    var imageLink = page.QuerySelector($"#product-photo img").Attributes["src"]?.Value;
                    var title = page.QuerySelector($"#product-photo img").Attributes["title"]?.Value;
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

                product.Catalog.Resources.Add(new Resource()
                {
                    Type = "source",
                    Uri = Driver.Url,
                    Attributes = new Dictionary<string, object>()
                                            {
                                                { "information", new string[]{ "catalog", "stock" } }
                                            }
                });
                if (page.QuerySelector($".product-details-documents-media a") != null)
                {
                    var documents = page.QuerySelectorAll($".product-details-documents-media a").Where(x => !string.IsNullOrWhiteSpace(x.Attributes["href"].Value) && ((x.Attributes["class"] != null && x.Attributes["class"].Value == "lnkDatasheet") || x.Attributes["href"].Value.Contains(".pdf"))).ToList();
                    foreach (var doc in documents)
                    {
                        bool isDatasheet = false;
                        var datasheetLink = doc.Attributes["href"].Value;
                        var title = doc.InnerText.NormalizeStr();
                        if (datasheetLink.Substring(0, 2) == "//")
                        {
                            datasheetLink = datasheetLink.Substring(2);
                        }
                        if (doc.Attributes["class"] != null)
                        {
                            if (doc.Attributes["class"].Value == "lnkDatasheet")
                            {
                                isDatasheet = true;
                            }
                        }
                        product.Catalog.Resources.Add(new Resource()
                        {
                            Type = "document",
                            Uri = datasheetLink,
                            Attributes = new Dictionary<string, object>()
                                            {
                                                { "mime", "application/pdf"},
                                                { "datasheet", isDatasheet},
                                                { "title", title},
                                            }
                        });
                    }
                }
                if (page.QuerySelector($".product-details-product-attributes tr") != null)
                {
                    var attributes = page.QuerySelectorAll($".product-details-product-attributes tr").ToList();
                    for (int i = 1; i < attributes.Count; i++)
                    {
                        if (attributes[i].QuerySelector("th") != null)
                        {
                            var label = attributes[i].QuerySelector("th").InnerText.NormalizeStr();
                            string value = string.Empty;
                            if (attributes[i].QuerySelector("th").Attributes["rowspan"] != null)
                            {
                                int row = int.Parse(attributes[i].QuerySelector("th").Attributes["rowspan"].Value);
                                for (int j = 0; j < row; j++)
                                {
                                    value += $"{page.QuerySelector($".product-details-product-attributes #product-attribute-table tr:nth-child({i + j + 1}) td").InnerText.NormalizeStr()} ";
                                }
                            }
                            else
                            {
                                value = attributes[i].QuerySelector("td").InnerText.NormalizeStr();
                            }
                            product.Catalog.CustomFields[label] = value;
                        }
                    }
                }
                if (page.QuerySelector($".product-details-environmental-export tr") != null)
                {
                    var attributes = page.QuerySelectorAll($".product-details-environmental-export tr").ToList();
                    for (int i = 1; i < attributes.Count; i++)
                    {
                        if (attributes[i].QuerySelector("th") != null)
                        {
                            var label = attributes[i].QuerySelector("th").InnerText.NormalizeStr();
                            var value = attributes[i].QuerySelector("td").InnerText.NormalizeStr();
                            product.Catalog.CustomFields[label] = value;
                        }
                    }
                }
                if (page.QuerySelector($"#product-overview tr") != null)
                {
                    var details = page.QuerySelectorAll($"#product-overview tr").ToList();
                    for (int i = 0; i < details.Count; i++)
                    {
                        var value = details[i].QuerySelector("td").InnerText.NormalizeStr();

                        if (details[i].QuerySelector("th").InnerText.NormalizeStr().ToLower() == "digi-key part number")
                        {
                            product.Catalog.PartNumber = product.Catalog.ProductLine = value;
                        }
                        if (details[i].QuerySelector("th").InnerText.NormalizeStr().ToLower() == "manufacturer")
                        {
                            product.Catalog.Manufacturer = value;
                        }
                        if (details[i].QuerySelector("th").InnerText.NormalizeStr().ToLower() == "detailed description")
                        {
                            product.Catalog.Description = value;
                        }
                        if (details[i].QuerySelector("th").InnerText.NormalizeStr().ToLower() == "manufacturer part number")
                        {
                            product.Catalog.CustomFields["suplierPartNumber"] = value;
                        }
                        if (details[i].QuerySelector("th").InnerText.NormalizeStr().ToLower() == "quantity available")
                        {
                            string quantityString = Regex.Replace(value, "[^0-9]", "");
                            if (!string.IsNullOrWhiteSpace(quantityString))
                            {
                                product.Position.Quantity = Int32.Parse(quantityString);
                            }
                        }
                        if (details[i].QuerySelector("th").InnerText.NormalizeStr().ToLower() == "manufacturer standard lead time")
                        {
                            int days = value.ToLower().Contains("weeks") ? 7 : 1;
                            string leadTimeString = Regex.Replace(value, "[^0-9.]", "");
                            if (!string.IsNullOrWhiteSpace(leadTimeString))
                            {
                                product.Position.LeadTime = Int32.Parse(leadTimeString) * days;
                            }
                        }
                    }
                }
                if (page.QuerySelector($"#qty") != null)
                {
                    string minQtyString = Regex.Replace(page.QuerySelector($"#qty").Attributes["value"].Value, "[^0-9.]", "");
                    if (!string.IsNullOrWhiteSpace(minQtyString))
                    {
                        product.Position.MinQuota = Int32.Parse(minQtyString);
                    }

                }
                if (page.QuerySelector($".product-dollars tr") != null)
                {
                    var priceInfo = page.QuerySelectorAll($".product-dollars tr").ToList();
                    for (int i = 1; i < priceInfo.Count; i++)
                    {
                        var from = priceInfo[i].QuerySelector("td:nth-child(1)").InnerText;
                        var price = priceInfo[i].QuerySelector("td:nth-child(2)").InnerText;
                        product.Position.Prices.Add(new GrabbersDotNetCore.Model.ResponseModels.Robot.Products.Result.Product.Position.PriceInfo.PriceInfo()
                        {
                            From = int.Parse(Regex.Replace(from, "[^0-9.]", "")),
                            Price = decimal.Parse(Regex.Replace(price, "[^0-9.]", ""))
                        });

                    }

                }
                product.Position.Currency = Dom.QuerySelector($"div.locale--lang-cur > span:last-child").InnerText.NormalizeStr();
                result.Add(product);
            }
            catch (NoSuchElementException)
            {
                TakeScreenShot($"digikey_error_{DateTime.Now.Millisecond}");
                throw;
            }
            catch (Exception)
            {
                TakeScreenShot($"digikey_error_{DateTime.Now.Millisecond}");
                throw;
            }
            return result;
        }

    }
}
