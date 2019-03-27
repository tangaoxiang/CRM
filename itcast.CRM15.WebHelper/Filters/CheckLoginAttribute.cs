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
    /// 统一登录验证过滤器
    /// </summary>
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 统一验证Session[Keys.uinfo]如果为null则跳转到登陆页
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //0.0判断是否有贴跳过登录检查的特性标签
            if (filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(SkipCheckLogin), false))
            {
                return;
            }

            if (filterContext.ActionDescriptor.IsDefined(typeof(SkipCheckLogin), false))
            {
                return;
            }

            //1.0 判断session是否为null
            if (filterContext.HttpContext.Session[Keys.uinfo] == null)
            {

                //1.0.1 查询cookie[Keys.Ismemeber]是否不为null，如果成立则模拟用户的登录，再将用户实体数据存入session[Keys.uinfo]中
                if (filterContext.HttpContext.Request.Cookies[Keys.IsMember] != null)
                {
                    //1.0 取出cookie中存入的uid的值
                    string uid = filterContext.HttpContext.Request.Cookies[Keys.IsMember].Value;
                    uid = DESEncrypt.Decrypt(uid);

                    //2.0 根据uid查询出用户的实体

                    //2.0.1 从缓存中获取autofac的容器对象
                    var cont = CacheMgr.GetData<IContainer>(Keys.AutofacContainer);
                    //2.0.2 找autofac容器获取IsysUserInfoServices接口的具体实现类的对象实例
                    IsysUserInfoServices userSer = cont.Resolve<IsysUserInfoServices>();

                    //2.0.3 根据userSer 集合uid查询数据
                    int iuserid = int.Parse(uid);
                    var userinfo = userSer.QueryWhere(c => c.uID == iuserid).FirstOrDefault();
                    if (userinfo != null)
                    {
                        //2.0.4 将userinfo存入session
                        filterContext.HttpContext.Session[Keys.uinfo] = userinfo;
                    }
                    else
                    {
                        ToLogin(filterContext);
                    }
                }
                else
                {

                    //2.0 跳转到登录页面
                    // filterContext.HttpContext.Response.Redirect("/admin/login/login");

                    //ContentResult cr = new ContentResult();
                    //cr.Content = "<script>alert('您未登录');window.location='/admin/login/login';</script>";

                    ToLogin(filterContext);
                }
            }

            base.OnActionExecuting(filterContext);
        }

        private static void ToLogin(ActionExecutingContext filterContext)
        {
            //1.0 判断当前请求是否为一个ajax请求
            bool isAjaxRequst = filterContext.HttpContext.Request.IsAjaxRequest();

            if (isAjaxRequst)
            {
                //ajax的请求,则返回json格式
                JsonResult json = new JsonResult();
                json.Data = new { status = Enums.EAjaxState.nologin, msg = "您未登录或者登录已经失效，请重新登录" };
                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                filterContext.Result = json;
            }
            else
            {
                //浏览器的请求
                ViewResult view = new ViewResult();
                view.ViewName = "/Areas/admin/Views/Shared/Tip.cshtml";
                filterContext.Result = view;
            }
        }
    }
}
