using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.BasePages;
using umbraco.interfaces;

namespace TechDevils.UrlTaskScheduler.TechDevilsTaskScheduler
{
	public class UrlMenuActions : IAction
	{
	    public char Letter { get; private set; }
	    public bool ShowInNotifier { get; private set; }
	    public bool CanBePermissionAssigned { get; private set; }
	    public string Icon { get; private set; }
	    public string Alias { get; private set; }
	    public string JsFunctionName { get; private set; }
	    public string JsSource { get; private set; }

        private static readonly UrlMenuActions m_instance = new UrlMenuActions();

        public static UrlMenuActions Instance
        {
            get
            {
                return UrlMenuActions.m_instance;
            }
        }

	    public UrlMenuActions()
	    {
	        Letter = '7';
	        ShowInNotifier = true;
	        CanBePermissionAssigned = false;
	        Icon = "departure";
	        Alias = "runNow";
            JsFunctionName = string.Format("{0}.actionRunNow()", (object)ClientTools.Scripts.GetAppActions);
	    }

	}
}