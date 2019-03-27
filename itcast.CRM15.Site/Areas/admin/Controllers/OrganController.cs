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

    public class OrganController : BaseController
    {
        public OrganController(IsysOrganStructServices orgSer)
        {
            base.organSer = orgSer;
        }

        //
        // GET: /admin/Organ/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getlist()
        {
            //1.0 获取kname的参数值
            string kname = Request.Form["kname"];

            object list;
            if (kname.IsEmpty())
            {
                list = organSer.QueryJoin(c =>true, new string[] { "sysKeyvalue" })
               .Select(c => new
               {
                   c.osID,
                   c.osParentID,
                   c.osName,
                   c.osCode,
                   c.sysKeyValue.KName
                   ,
                   c.osShortName,
                   c.osStatus,
                   c.osRemark
               });
            }
            else
            {
                //1.0 获取组织结构表中的正常数据
                list = organSer.QueryJoin(c => c.osName.Contains(kname)
                    , new string[] { "sysKeyvalue" })
                   .Select(c => new
                   {
                       c.osID,
                       c.osParentID,
                       c.osName,
                       c.osCode,
                       c.sysKeyValue.KName
                       ,
                       c.osShortName,
                       c.osStatus,
                       c.osRemark
                   });
            }

            //2.0 序列化
            return Json(new { Rows = list, Total = 0 });
        }

    }
}
