using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.CRM15.WebHelper
{
    using itcast.CRM15.Common;
    using System.Web.Mvc;

    /// <summary>
    /// 自定义异常捕获过滤器
    /// </summary>
    public class ExpFilter : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            Exception exp = filterContext.Exception;

            //TODO:捕获异常记录到日志文本中(数据库中)  LogNet4.dll

            //获取ex的第一级内部异常
            Exception innerEx = exp.InnerException == null ? exp : exp.InnerException;
            //循环获取内部异常直到获取详细异常信息为止
            while (innerEx.InnerException != null)
            {
                innerEx = innerEx.InnerException;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                JsonResult json = new JsonResult();
                json.Data = new { status = (int)Enums.EAjaxState.error, msg = innerEx.Message };
                json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

                filterContext.Result = json;
            }
            else
            {
                ViewResult viewResult = new ViewResult();
                viewResult.ViewName = "/Views/Shared/Error.cshtml";
                viewResult.ViewData["exp"] = exp;
                filterContext.Result = viewResult;
            }
            
            //告诉MVC框架异常被处理
            filterContext.ExceptionHandled = true;

            base.OnException(filterContext);
        }
    }
}
