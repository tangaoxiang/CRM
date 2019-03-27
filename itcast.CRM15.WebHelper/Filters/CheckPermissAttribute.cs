using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.CRM15.WebHelper
{
    using System.Web.Mvc;
    using System.Web;
    using itcast.CRM15.Common;
    using IServices;
    using Autofac;


    /// <summary>
    /// 负责检查权限的全局过滤器
    /// </summary>
    public class CheckPermissAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //0.0判断是否有贴跳过权限检查的特性标签
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipCheckPermiss), false))
            {
                return;
            }

            if (filterContext.ActionDescriptor.IsDefined(typeof(SkipCheckPermiss), false))
            {
                return;
            }

            //1.0 获取当前触发此OnActionExecuting的aciton
            string actionName = filterContext.ActionDescriptor.ActionName.ToLower();

            //2.0 控制名称
            string controlerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();

            //3.0 获取区域的名称
            string areaName = string.Empty;
            if (filterContext.RouteData.DataTokens.ContainsKey("area"))
            {
                areaName = filterContext.RouteData.DataTokens["area"].ToString().ToLower();
            }

            //4.0 根据上面的三个成员的值作为条件去当前用户的权限按钮缓存数据查找，如果没有找到则表示没有权限
            //2.0.1 从缓存中获取autofac的容器对象
            var cont = CacheMgr.GetData<IContainer>(Keys.AutofacContainer);
            IsysPermissListServices iperSer = cont.Resolve<IsysPermissListServices>();
            var list = iperSer.GetFunctionsForUserByCache(UserMgr.GetCurrentUserInfo().uID);

            var isOK = list.Any(c => c.mArea.ToLower() == areaName
                && c.mController.ToLower() == controlerName
                && c.fFunction.ToLower() == actionName);

            if (isOK == false)
            {
                isOK = list.Any(c => c.mArea.ToLower() == areaName
                && c.mController.ToLower() == controlerName
                && c.mAction.ToLower() == actionName);
            }

            if (isOK == false)//无权限
            {
                ToLogin(filterContext);
            }

        }

        private static void ToLogin(ActionExecutingContext filterContext)
        {
            //1.0 判断当前请求是否为一个ajax请求
            bool isAjaxRequst = filterContext.HttpContext.Request.IsAjaxRequest();

            if (isAjaxRequst)
            {
                //ajax的请求,则返回json格式
                JsonResult json = new JsonResult();
                json.Data = new { status = Enums.EAjaxState.error, msg = "您没有权限访问此页面" };
                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                filterContext.Result = json;
            }
            else
            {
                //浏览器的请求
                ViewResult view = new ViewResult();
                view.ViewName = "/Areas/admin/Views/Shared/NoPermiss.cshtml";
                filterContext.Result = view;
            }
        }
    }
}
