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

namespace GtabbersDotNetCore.Bll.BllRepositories.Robots.RobotBase
{
    public abstract class DriverFactory : IDriver
    {
        protected IWebDriver Driver;

        public void Initialize(string robot)
        {
            var basePath =
                System.IO.Path.GetDirectoryName((System.Reflection.Assembly.GetExecutingAssembly().Location)) + "/Resources/";
            var driver = Helper.ConfigurationGrabber.GetSection($"grabbers:digikey").Value;
            Type type = Type.GetType($"{driver},CoreCompat.Selenium.WebDriver");
            switch (driver)
            {
                case "OpenQA.Selenium.Chrome.ChromeDriver":
                    ChromeDriverService service = ChromeDriverService.CreateDefaultService(basePath);

                    //if (Helper.ConfigurationGrabber.GetSection($"drivers:{driver}") != null)
                    //{
                    //    service.SuppressInitialDiagnosticInformation = Convert.ToBoolean(Helper.ConfigurationGrabber.GetSection($"drivers:{driver}").GetSection("suppressInitialDiagnosticInformation").Value);
                    //    service.HideCommandPromptWindow = Convert.ToBoolean(Helper.ConfigurationGrabber.GetSection($"drivers:{driver}").GetSection("hideCommandPromptWindow").Value);
                    //}

                    ChromeOptions options = new ChromeOptions();
                    options.AddArguments("--proxy-server=socks5://195.201.37.174:21");

                    if (Helper.ConfigurationGrabber.GetSection($"drivers:{driver}").GetSection("arguments") != null)
                        options.AddArguments(
                            (Helper.ConfigurationGrabber.GetSection($"drivers:{driver}").GetSection("arguments").GetChildren().Select(x => x.Value.ToString())));

                    Driver = (ChromeDriver)Activator.CreateInstance(type, service, options);

                    //if (ExtentionApp.ConfigurationGrabber.GetSection($"drivers:{driver}").GetSection("pageLoadTimeOut") != null)
                    //{
                    //    Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(int.Parse(ExtentionApp.ConfigurationGrabber.GetSection($"drivers:{driver}").GetSection("pageLoadTimeOut").Value));
                    //};
                    break;

                case "OpenQA.Selenium.Firefox.FirefoxDriver":
                    Driver = new FirefoxDriver();
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
