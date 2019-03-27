
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace itcast.CRM15.Site.Areas.admin.Controllers
{
    using itcast.CRM15.Model.ModelViews;
    using IServices;
    using itcast.CRM15.WebHelper;
    using itcast.CRM15.Common;
    using System.Web.WebPages;
    using itcast.CRM15.Model;

    [SkipCheckLogin, SkipCheckPermiss]
    public class LoginController : BaseController
    {
        public LoginController(IsysUserInfoServices userSer, IsysPermissListServices pSer)
        {
            base.userinfoSer = userSer;
            base.permissSer = pSer;
        }

        #region 1.0 登录
        /// <summary>
        /// 负责将登录视图返回
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Login()
        {
            LoginInfo info = new LoginInfo()
            {
                uLoginName = "admin",
                IsMember = false
            };

            //1.0 判断cookie[Keys.IsMember]!=null 则应该将登录视图上的记住3天勾选上
            if (Request.Cookies[Keys.IsMember] != null)
            {
                info.IsMember = true;
            }

            return View(info);
        }

        /// <summary>
        /// 负责接收页面提交过来的数据进行登录处理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginInfo model)
        {
            try
            {
                //1.0 实体参数合法性验证
                if (ModelState.IsValid == false)
                {
                    return WriteError("实体验证失败");
                }

                //2.0 检查验证码的合法性
                string vcodeFromSession = string.Empty;
                if (Session[Keys.vcode] != null)
                {
                    vcodeFromSession = Session[Keys.vcode].ToString();
                }
                if (model.VCode.IsEmpty()
                    || vcodeFromSession.Equals(model.VCode, StringComparison.OrdinalIgnoreCase) == false)
                {
                    return WriteError("验证码不合法");
                }

                //3.0 检查用户名和密码的正确性
                string md5PWD = Kits.MD5Entry(model.uLoginPWD);
                var userinfo = userinfoSer.QueryWhere(c => c.uLoginName == model.uLoginName && c.uLoginPWD == md5PWD).FirstOrDefault();
                if (userinfo == null)
                {
                    return WriteError("用户名或者密码错误");
                }

                //4.0 将userinfo存入session
                Session[Keys.uinfo] = userinfo;


                //5.0 判断logininfo实体model中的ismemeber是否为true，如果成立则将用户id写入cookie中
                //输出给浏览器存入硬盘中，过期时间为3天
                if (model.IsMember)
                {
                    //一般要将用户ID利用DES(对称加密算法使用自己定义的一个密码)进行加密成，将来可以使用同一个密码进行解密
                    string entrystr = DESEncrypt.Encrypt(userinfo.uID.ToString());
                    HttpCookie cookie = new HttpCookie(Keys.IsMember, entrystr);
                    cookie.Expires = DateTime.Now.AddDays(3);
                    Response.Cookies.Add(cookie);
                }
                else
                {
                    //清除cookie操作
                    HttpCookie cookie = new HttpCookie(Keys.IsMember, "");
                    cookie.Expires = DateTime.Now.AddYears(-3);
                    Response.Cookies.Add(cookie);
                }

                //5.0 将当前用户的所有权限按钮缓存起来，选择此缓存永久有效，当管理员操作用户分配角色和设置此用户所在角色的权限菜单的时候，要使缓存失效
                permissSer.GetFunctionsForUserByCache(userinfo.uID);

                //6.0 返回登录成功消息
                return WriteSuccess("登录成功");
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }
        }
        #endregion

        #region 2.0 登出

        [HttpGet]
        public ActionResult Logout()
        {
            //1.0 清空Session[Keys.uinfo]对象
            Session[Keys.uinfo] = null;

            //2.0 看具体的需求，可以清除cookie也可以保留

            //3.0 跳转到登录页面
            return RedirectToAction("Login", "Login");
        }

        #endregion
    }
}
