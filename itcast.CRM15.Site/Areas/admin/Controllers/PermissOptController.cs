using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace itcast.CRM15.Site.Areas.admin.Controllers
{
    using System.Web.WebPages;
    using WebHelper;
    using Common;
    using IServices;
    using Model.ModelViews;
    using Model;
    using EntityMapping;

    [SkipCheckPermiss]
    public class PermissOptController : BaseController
    {
        public PermissOptController(IsysPermissListServices pSer)
        {
            base.permissSer = pSer;
        }

        /// <summary>
        /// 负责获取当前登录用户所在菜单的权限按钮
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getFunctions()
        {
            //1.0 获取用户id
            int uid = UserMgr.GetCurrentUserInfo().uID;

            //2.0 获取从前台传入的当前页面的url
            string url = Request.Form["murl"];

            //3.0 根据url从当前用户的权限按钮缓存数据中获取指定的按钮
            var allFunctionsFromCache = permissSer.GetFunctionsForUserByCache(uid);
            //3.0.1 从allFunctionsFromCache 集合中获取指定的url对于的按钮数据
            var functions = allFunctionsFromCache.Where(c => c.mUrl.ToLower() == url.ToLower());

            //4.0 遍历functions集合拼接成ligerGrid中的Toolbar要求的json格式的字符串
            /*
             * ： [
                { text: '增加', click: add, icon: 'add' },
                { line: true },
                { text: '修改', click: edit, icon: 'modify' },
                { line: true },
                { text: '删除', click: del, icon: 'delete' },
                { line: true },
                { text: '刷新', click: getlist, icon: 'refresh' }
                ]
             */
            System.Text.StringBuilder resJson = new System.Text.StringBuilder("[", 200);
            if (functions.Any())
            {
                foreach (var item in functions)
                {
                    resJson.Append("{ \"text\": \"" + item.fName + "\", \"click\": \"" + item.fFunction + "\", \"icon\": \"" + item.fPicname + "\" }, { \"line\": \"true\" },");
                }

                //将最后一个逗号移除
                if (resJson.Length > 1)
                {
                    resJson.Remove(resJson.Length - 1, 1);
                }
            }
            resJson.Append("]");

            //返回
            return Content(resJson.ToString());
        }

    }
}
