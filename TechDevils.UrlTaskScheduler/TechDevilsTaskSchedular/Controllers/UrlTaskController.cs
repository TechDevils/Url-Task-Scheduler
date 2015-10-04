using System.Net.Http.Formatting;
using umbraco;
using umbraco.BusinessLogic.Actions;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskSchedular.Controllers
{
    [Tree("scheduleUrlsArea", "urlTasks", "Url Tasks")]
    [PluginController("TaskScheduler")]
    public class UrlTaskController : TreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            //var cs = ApplicationContext.Current.Services.ContentService;
            //var newNode = cs.CreateContent("Test", -1, "umbFeature");

            //cs.SaveAndPublishWithStatus(newNode);

            var ctrl = new UrlScheduleTaskApiController();
            var nodes = new TreeNodeCollection();

            var allUrls = ctrl.GetAll();

            foreach (var url in allUrls)
            {
                var urlIcon = "icon-globe-inverted-europe-africa color-blue";

                if (url.Disabled)
                {
                    urlIcon = "icon-globe-inverted-europe-africa color-red";

                }

                var node = CreateTreeNode(
                    url.Id.ToString(),
                     "-1",
                    queryStrings,
                    url.Description,
                    urlIcon
                    );

                nodes.Add(node);
            }
            //var randomNumber = new Random();

            //for (int i = 0; i < randomNumber.Next(3,7); i++)
            //{
            //    var node = CreateTreeNode("13", "-1", queryStrings, "Hellos");
            //    nodes.Add(node);
            //}

            return nodes;
            //throw new NotImplementedException();
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();

            if (id == Constants.System.Root.ToInvariantString())
            {
                // root actions              
                menu.Items.Add<CreateChildEntity, ActionNew>(ui.Text("actions", ActionNew.Instance.Alias));
                menu.Items.Add<RefreshNode, ActionRefresh>(ui.Text("actions", ActionRefresh.Instance.Alias), true);
                return menu;
            }
            else
            {
                //menu.DefaultMenuAlias = ActionDelete.Instance.Alias;
                menu.Items.Add<UrlMenuActions>(ui.Text("actions", UrlMenuActions.Instance.Alias));
                menu.Items.Add<ActionDelete>(ui.Text("actions", ActionDelete.Instance.Alias));

            }
            return menu;
        }
    }
}