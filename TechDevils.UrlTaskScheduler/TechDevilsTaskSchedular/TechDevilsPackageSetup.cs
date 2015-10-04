using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using log4net;
using log4net.Core;
using umbraco;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.cms.helpers;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Strings;
using umbraco.interfaces;
using Umbraco.Web.WebServices;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular
{
    //<Action runat="install" alias="TechDevilsPackageSetup" source="~/app_data/temp/TechDevils.UrlTaskScheduler/TechDevils.UrlTaskScheduler.dll" target="~/bin/TechDevils.UrlTaskScheduler.dll" minversion="7" />
    public class TechDevilsPackageSetup : IPackageAction
    {

        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //public bool Setup()
        //{



        //    throw new NotImplementedException();
        //}

        private void updateLangFiles(string langFile, string area, string key, string value)
        {
            _log.Info("updateLangFiles: " + langFile);
            //var dashboardConfig = UmbracoConfig.For.DashboardSettings();

            //if (dashboardConfig.Sections.All(x => x.Alias != "TechDevilsTaskDashborad"))
            //{
            var umbrcoPath = GlobalSettings.Path;
            var doc = xmlHelper.OpenAsXmlDocument(string.Format("{0}/config/lang/{1}", umbrcoPath, langFile));
            var actionNode = doc.SelectSingleNode(string.Format("//area[@alias='{0}']", area));

            if (actionNode != null)
            {
                var node = actionNode.AppendChild(doc.CreateElement("key"));
                if (node.Attributes != null)
                {
                    var att = node.Attributes.Append(doc.CreateAttribute("alias"));
                    att.InnerText = key;
                }
                node.InnerText = value;
            }
            doc.Save(HttpContext.Current.Server.MapPath(string.Format("{0}/config/lang/{1}", GlobalSettings.Path, langFile)));

        }

        public bool Execute(string packageName, XmlNode xmlData)
        {
            _log.Info("Execute: " + packageName);
            updateLangFiles("en.xml", "sections", "scheduleUrlsArea", "Schedeuled Urls");
            updateLangFiles("en_us.xml", "sections", "scheduleUrlsArea", "Schedeuled Urls");

            //<task log="true" alias="TechDevilsScheduledTask" interval="60" url="/umbraco/api/UrlTaskRunner/UrlRunner"/>
            //setupScheduledTaskInUmbracoSettingsFile();



            return true;
        }

        public string Alias()
        {
            return "TechDevilsPackageSetup";
        }

        public bool Undo(string packageName, XmlNode xmlData)
        {

            return false;

            //throw new NotImplementedException();
        }

        public XmlNode SampleXml()
        {
            return null;
            //throw new NotImplementedException();
        }

        private void setupScheduledTaskInUmbracoSettingsFile()
        {
            var configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            var xmlDoc = XDocument.Load(configFile);

            //var umbracoSettingFile = xmlDoc.Root.Descendants("//configuration/umbracoConfiguration/settings");
            var umbracoSettingLocation = xmlDoc.Root.Element("umbracoConfiguration");
            var settingFile = umbracoSettingLocation.Element("settings");
            var fileLocation = settingFile.Attribute("configSource").Value;

            var UmbracoSettingsFile = HttpContext.Current.Server.MapPath("/" + fileLocation);

            if (File.Exists(UmbracoSettingsFile))
            {
                var baseUrl = HttpContext.Current.Request["HTTP_HOST"];
                var umbracoSettingFile = XDocument.Load(UmbracoSettingsFile);

                var scheduledtasksSection = umbracoSettingFile.Root.Element("scheduledTasks");

                if (scheduledtasksSection.Elements("task").All(x => x.Attribute("alias").Value != "TechDevilsScheduledTask"))
                {
                    var schedualTaskitem = new XElement("task");

                    var urlAtt = new XAttribute("url", "http://" + baseUrl + "/umbraco/api/UrlTaskRunner/UrlRunner");
                    var intervalAtt = new XAttribute("interval", 60);
                    var logAtt = new XAttribute("log", true);
                    var aliasAtt = new XAttribute("alias", "TechDevilsScheduledTask");

                    schedualTaskitem.Add(urlAtt);
                    schedualTaskitem.Add(intervalAtt);
                    schedualTaskitem.Add(logAtt);
                    schedualTaskitem.Add(aliasAtt);

                    scheduledtasksSection.Add(schedualTaskitem);

                    umbracoSettingFile.Save(UmbracoSettingsFile);
                }
            }
        }

    }
}