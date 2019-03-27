using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.CRM15.WebHelper
{
    using System.Web.Mvc;
    using IServices;
    using itcast.CRM15.Common;
    /// <summary>
    /// 控制器的父类，将来被此网站中的所有控制器继承
    /// </summary>
    public class BaseController : Controller
    {
        /// <summary>
        ///  //1.0 定义当前系统中所有的业务逻辑层的接口成员
        /// </summary>
        protected IsysFunctionServices funSer;
        protected IsysKeyValueServices keyvalSer;
        protected IsysMenusServices menuSer;
        protected IsysOrganStructServices organSer;
        protected IsysPermissListServices permissSer;
        protected IsysRoleServices roleSer;
        protected IsysUserInfoServices userinfoSer;
        protected IsysUserInfo_RoleServices userinfoRoleSer;
        protected IwfProcessServices processSer;
        protected IwfRequestFormServices requestformSer;
        protected IwfWorkServices workSer;
        protected IwfWorkBranchServices workbranchSer;
        protected IwfWorkNodesServices worknodesSer;

        #region 2.0 封装ajax请求的返回方法
        protected ActionResult WriteSuccess(string msg)
        {
            return Json(new { status = (int)Enums.EAjaxState.sucess, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult WriteSuccess(string msg, object obj)
        {
            return Json(new { status = (int)Enums.EAjaxState.sucess, msg = msg, datas = obj }, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult WriteError(string errmsg)
        {
            return Json(new { status = (int)Enums.EAjaxState.error, msg = errmsg }, JsonRequestBehavior.AllowGet);
        }

        protected ActionResult WriteError(Exception ex)
        {
            //获取ex的第一级内部异常
            Exception innerEx = ex.InnerException == null ? ex : ex.InnerException;
            //循环获取内部异常直到获取详细异常信息为止
            while (innerEx.InnerException != null)
            {
                innerEx = innerEx.InnerException;
            }

            return Json(new { status = (int)Enums.EAjaxState.error, msg = innerEx.Message }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        protected virtual void SetStatus()
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            dic.Add(0, "正常");
            dic.Add(1, "停用");

            SelectList clist = new SelectList(dic, "Key", "Value");

            ViewBag.status = clist;
        }
    }
}
