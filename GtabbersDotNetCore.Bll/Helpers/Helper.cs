using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Microsoft.Extensions.Configuration;

namespace GtabbersDotNetCore.Bll.Helpers
{

    public static class Extentions
    {
        public static bool IsElementExist(this IWebDriver driver, By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
       

        public static bool IsElementExist(this IWebElement element, By by)
        {
            try
            {
                element.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static IWebElement WaitUntilElementExists(this IWebDriver driver, By elementLocator, double timeout = 0.005)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                return wait.Until(ExpectedConditions.ElementExists(elementLocator));
            }
            catch (NoSuchElementException)
            {
                //Console.WriteLine("Element with locator: '" + elementLocator + "' was not found in current context page.");
                return null;
            }
            catch (Exception)
            {
                //Console.WriteLine("Element with locator: '" + elementLocator + "' was not found in current context page.");
                return null;
            }


        }

        public static string NormalizeStr(this string str)
        {
            return Regex.Replace(str, @"\s\s+", " ", RegexOptions.Multiline).Trim();
        }

      
    }

    public static class Helper
    {
        public static IConfigurationRoot ConfigurationGrabber { get; set; }

      

        public static T DeepClone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }


        public enum TypeOfSearchResult
        {
            [Description("not found")]
            NotFound,
            [Description("product")]
            Product,
            [Description("products")]
            Products,
            [Description("catalogs")]
            Catalogs
        }
    }
}
