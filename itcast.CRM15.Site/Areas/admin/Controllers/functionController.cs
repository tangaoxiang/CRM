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

    public class functionController : BaseController
    {
        public functionController(IsysFunctionServices fSer, IsysMenusServices mSer)
        {
            base.funSer = fSer;
            base.menuSer = mSer;
        }

        #region 1.0 列表

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        ///  负责获取id指定的在sysFuntion中的数据
        ///  sql:select * from sysFuntion where mid=id
        /// </summary>
        /// <param name="id">代表sysFunction中的mID的值</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getlist(int id)
        {
            try
            {
                //1.0 获取数据
                var list = funSer.QueryWhere(c => c.mID == id).Select(c => new
                {
                    c.fID,
                    c.fName,
                    c.fFunction,
                    c.fPicname,
                    c.fStatus
                });

                return Json(new { Rows = list, Total = 0 });
            }
            catch (Exception ex)
            {
                return WriteError(ex); //{status:1,msg:"error"}
            }
        }

        /// <summary>
        /// 负责给ligerTree提供数据的,不需要进行权限验证
        /// </summary>
        /// <returns></returns>
        [HttpPost, SkipCheckPermiss]
        public ActionResult getMenusTree()
        {
            //1.0 查询sysMenus表的所有有效数据
            var list = menuSer.QueryWhere(c => c.mStatus == (int)Enums.EState.Nomal).Select(c => new
            {
                c.mID,
                c.mParentID,
                c.mName
            });


            //2.0 
            return Json(list);
        }

        #endregion

        #region 2.0 新增

        /// <summary>
        /// 返回新增视图
        /// </summary>
        /// <param name="id">表示的菜单id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult add(int id)
        {
            //获取状态的集合以ViewBag.status传入给视图
            base.SetStatus();
            return View();
        }

        /// <summary>
        /// 负责将数据保存到db中
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult add(int id, sysFunctionView model)
        {
            try
            {
                //1.0 判断视图的合法性
                if (ModelState.IsValid == false)
                {
                    return WriteError("实体验证失败");
                }

                //2.0 将model中的Mid补全
                model.mID = id;
                model.fCreateTime = DateTime.Now;
                model.fUpdateTime = DateTime.Now;
                model.fCreatorID = UserMgr.GetCurrentUserInfo().uID;

                //3.0 实体转换和保存
                var entity = model.EntityMap();
                funSer.Add(entity);
                funSer.SaveChanges();

                return WriteSuccess("新增成功");
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }
        }

        #endregion

    }
}
