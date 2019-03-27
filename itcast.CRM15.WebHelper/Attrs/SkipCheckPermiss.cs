using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.CRM15.WebHelper
{
    /// <summary>
    /// 定义自定义过滤器用于跳过权限检查，
    /// 特点：此过滤器只能贴到方法或者类上
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class SkipCheckPermiss : Attribute
    {
    }
}
