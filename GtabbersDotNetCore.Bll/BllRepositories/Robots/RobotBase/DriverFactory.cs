using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GtabbersDotNetCore.Bll.Helpers;
using GtabbersDotNetCore.Bll.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.PhantomJS;
using System.Reflection;
using OpenQA.Selenium.Remote;

namespace GtabbersDotNetCore.Bll.BllRepositories.Robots.RobotBase
{
    public abstract class DriverFactory : IDriver
    {
        protected IWebDriver Driver;

        public void Initialize(string robot)
        {
            var basePath =
                System.IO.Path.GetDirectoryName((System.Reflection.Assembly.GetExecutingAssembly().Location)) + "/Resources/";
            var driver = Helper.ConfigurationGrabber.GetSection($"Grabbers:{robot}").Value;
            Type type = Type.GetType($"{driver},CoreCompat.Selenium.WebDriver");
            switch (driver)
            {
                case "OpenQA.Selenium.Chrome.ChromeDriver":
                    ChromeDriverService service = ChromeDriverService.CreateDefaultService(basePath);
                    ChromeOptions options = new ChromeOptions();
                  
                    if (Helper.ConfigurationGrabber.GetSection($"Drivers:{driver}").GetSection("arguments") != null)
                        options.AddArguments(
                            (Helper.ConfigurationGrabber.GetSection($"Drivers:{driver}").GetSection("arguments").GetChildren().Select(x => x.Value.ToString())));

                    Driver = (ChromeDriver)Activator.CreateInstance(type, service, options);
                    break;

                case "OpenQA.Selenium.Firefox.FirefoxDriver":

                    FirefoxDriverService serv = FirefoxDriverService.CreateDefaultService(basePath);
                    Driver = (FirefoxDriver)Activator.CreateInstance(type, serv);
                    break;

                case "OpenQA.Selenium.PhantomJS.PhantomJSDriver":
                    Driver = new PhantomJSDriver();
                    break;

                default:
                    throw new Exception("Robot does'n exist");
            }
        }

        public void Quit()
        {
            Driver?.Quit();
        }
    }
}
