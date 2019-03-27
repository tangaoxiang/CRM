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

    public class userinfoController : BaseController
    {

        public userinfoController(IsysUserInfoServices uSer, IsysOrganStructServices oSer, IsysRoleServices rSer, IsysUserInfo_RoleServices urSer)
        {
            base.userinfoSer = uSer;
            base.organSer = oSer;
            base.roleSer = rSer;
            base.userinfoRoleSer = urSer;
        }


        #region 1.0 列表
        /// <summary>
        /// 返回用户列表视图
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 接收ligerGrid的请求
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult getlist()
        {
            //分页获取用户
            //1.0 获取用户传入的查询条件以及分页条件
            string pageindex = Request.Form["page"];//page是ligerGrid固定传入的名称,表示当前的页码
            string pagesize = Request.Form["pagesize"]; //pagesize是ligerGrid固定传入的名称,表示当前的页容量
            string kname = Request.Form["kname"];

            //2.0 分页参数合法性验证
            int ipageindex = pageindex.AsInt();
            int ipagesize = pagesize.AsInt();
            int rowcount = 0;

            //2.0 实现分页数据的获取操作
            object list;
            if (kname.IsEmpty())
            {
                list = userinfoSer.QueryByPage(ipageindex, ipagesize, out rowcount, c => c.uID, c => true)
                    .Select(c => new
                    {
                        c.uID,
                        c.uLoginName,
                        c.uRealName,
                        c.uTelphone,
                        c.uMobile,
                        c.uEmial,
                        c.uQQ,
                        c.uGender,
                        c.uStatus,
                        c.uWorkGroupID
                    });
            }
            else
            {
                list = userinfoSer.QueryByPage(ipageindex, ipagesize, out rowcount, c => c.uID, c => c.uLoginName.Contains(kname) || c.uRealName.Contains(kname)).Select(c => new
                {
                    c.uID,
                    c.uLoginName,
                    c.uRealName,
                    c.uTelphone,
                    c.uMobile,
                    c.uEmial,
                    c.uQQ,
                    c.uGender,
                    c.uStatus,
                    c.uWorkGroupID
                });
            }

            //将分页数据返回
            return Json(new { Rows = list, Total = rowcount });

        }
        #endregion

        #region 帮助方法

        private void SetCompany()
        {
            //获取[sysOrganStruct]表中cateid=3的所有有效数据
            var list = organSer.QueryWhere(c => c.osCateID == 3);
            list.Insert(0, new sysOrganStruct() { osID = -1, osName = "请选择公司" });

            SelectList clist = new SelectList(list, "osID", "osName");

            ViewBag.company = clist;
        }

        #endregion

        #region 2.0 新增

        [HttpGet]
        public ActionResult add()
        {
            SetStatus();
            SetCompany();

            return View();
        }

        [HttpPost]
        public ActionResult add(UserInfoAdd model)
        {
            if (ModelState.IsValid == false)
            {
                return WriteError("验证失败");
            }

            try
            {
                //1.0 
                sysUserInfo user = new sysUserInfo()
                {
                    uLoginName = model.uLoginName,
                    uStatus = model.uStatus,
                    uGender = model.uGender
                    ,
                    uEmial = model.uEmial,
                    uRealName = model.uRealName,
                    uMobile = model.uMobile
                    ,
                    uLoginPWD = Kits.MD5Entry("123456"),
                    uUpdateTime = DateTime.Now,
                    uCreatorID = UserMgr.GetCurrentUserInfo().uID,
                    uCreateTime = DateTime.Now,
                    uCompanyID = model.uCompanyID,
                    uDepID = model.uDepID,
                    uWorkGroupID = model.uWorkGroupID
                };


                userinfoSer.Add(user);
                userinfoSer.SaveChanges();

                return WriteSuccess("新增成功");

            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }
        }

        /// <summary>
        /// 检查用户是否存在
        /// </summary>
        /// <returns></returns>
        [SkipCheckPermiss]
        public ActionResult checkuser()
        {
            //1.0 获取用户名称
            string username = Request.Form["uLoginName"];

            //2.0 
            bool isexist = userinfoSer.QueryWhere(c => c.uLoginName == username).Any();
            if (isexist)
            {
                return Content("false");
            }
            return Content("true");
        }

        #endregion

        #region 2.0.1 联动ajax请求方法

        /// <summary>
        /// 根据公司id获取部门数据,或者部门下面的工作组数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [SkipCheckPermiss]
        public ActionResult getDep(int id)
        {
            var deplist = organSer.QueryWhere(c => c.osParentID == id);
            //.Select(c => new { c.osID, c.osName });
            deplist.Insert(0, new sysOrganStruct() { osName = "请选择", osID = -1 });

            var dlist = deplist.Select(c => new { c.osID, c.osName });

            return WriteSuccess("ok", dlist);
        }
        #endregion

        #region 3.0 设置角色

        /// <summary>
        /// 负责将设置角色的视图页面呈现给用户，但是不包括权限数据的显示
        /// </summary>
        /// <param name="id">格式：用户id-工作组id </param>
        /// <returns></returns>
        [HttpGet, SkipCheckPermiss]
        public ActionResult setRole(string id)
        {
            //1.0 先将id打断成数组
            string[] useridWkids = id.Split('-');
            ViewBag.uid = useridWkids[0];
            ViewBag.wkid = useridWkids[1];
            return View();
        }

        /// <summary>
        /// 负责将工作组下面的权限数据查询出来返回
        /// select * from sysrole where edepid=id
        /// </summary>
        /// <param name="id">当前用户的id和所在工作组的id ，格式：uid-wkid </param>
        /// <returns></returns>
        [HttpPost, SkipCheckPermiss]
        public ActionResult getRoleList(string id)
        {
            //1.0 打断参数
            string[] useridWkids = id.Split('-');
            int uid = useridWkids[0].AsInt();
            int wkid = useridWkids[1].AsInt();

            //1.0 获取当前用户在sysUserInfo_Role表中的角色数据集合
            var roleList = userinfoRoleSer.QueryWhere(c => c.uID == uid);

            int iid = id.AsInt();
            //2.0 查询数据
            var list = roleSer.QueryWhere(c => c.eDepID == wkid).Select(c => new
            {
                c.rID,
                c.rName
                ,
                ischecked = roleList.Any(d => d.rID == c.rID)
            });
            //将结果响应回ligerGrid方法
            return Json(new { Rows = list, Total = 0 });
        }


        /// <summary>
        /// 给用户设置新的角色数据
        /// </summary>
        /// <param name="id">格式： 用户id-此用户的最新角色id  数据格式：19-19,20,</param>
        /// <returns></returns>
        [HttpPost, SkipCheckPermiss]
        public ActionResult setUserRole(string id)
        {
            try
            {
                //1.0 解析出id参数
                string[] ids = id.Split('-');
                int uid = ids[0].AsInt(); //获取用户id
                //获取当前用户的所有角色id
                string[] roleids = ids[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                //2.0 根据用户id删除表sysUserInfo_Role中的所有老数据
                userinfoRoleSer.QueryWhere(c => c.uID == uid).ForEach(c => userinfoRoleSer.Delete(c, true));

                //3.0 批量新增此用户新的角色数据
                foreach (var rid in roleids)
                {
                    //批量添加到EF容器
                    userinfoRoleSer.Add(new sysUserInfo_Role() { uID = uid, rID = rid.AsInt() });
                }
                
                //开启分布式事务
                using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
                {
                    //4.0 统一进行批量删除和批量新增操作
                    userinfoRoleSer.SaveChanges();

                    //4.0.1 提交事务
                    scop.Complete();
                }

                //5.0 响应
                return WriteSuccess("用户角色设置完毕");
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }
        }
        #endregion

    }
}
