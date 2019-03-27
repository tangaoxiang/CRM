using System.Web.Mvc;

namespace itcast.CRM15.Site.Areas.workflow
{
    public class workflowAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "workflow";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "workflow_default",
                "workflow/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
