﻿
using System.Web;
using System.Web.Mvc;

namespace itcast.CRM15.Site
{
    using itcast.CRM15.WebHelper;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //将登录验证过滤器注册为全局过滤器,实现整个网站所有action的登录检查
            filters.Add(new CheckLoginAttribute());
            //将权限验证过滤器注册为全局
            filters.Add(new CheckPermissAttribute());

            //将统一的action异常捕获过滤器ExpFilter注册成为 全局
            filters.Add(new ExpFilter());
        }
    }
}