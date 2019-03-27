using System.Web.Mvc;

namespace itcast.CRM15.Site.Areas.chat
{
    public class chatAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "chat";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "chat_default",
                "chat/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
