﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace itcast.CRM15.Site.Controllers
{
    using IServices;
    using Model.ModelViews;
    using EntityMapping;
    using itcast.CRM15.WebHelper;

    /// <summary>
    /// MVC中控制器类的对象是 DefaultControllerFactory
    /// DefaultControllerFactory:只会查找默认的无参构造函数，所以此处加了有参数的构造函数以后
    /// DefaultControllerFactory在运行的时候报错
    /// 将来只能由autofac中的控制器工厂调用有参数的构造函数来创建具体的控制器类实例
    /// </summary>
    public class HomeController : BaseController
    {

        public HomeController(IsysFunctionServices funSer, IsysKeyValueServices keyvalueSer)
        {
            base.funSer = funSer;
            base.keyvalSer = keyvalueSer;
        }

        //
        // GET: /Home/
        public ActionResult Index()
        {
            var model = funSer.QueryWhere(c => c.fID == 0).FirstOrDefault();
            model.fName = "默认1111";
            //funSer.SaveChanges();

            var keyvalueModel = base.keyvalSer.QueryWhere(c => c.KID == 2).FirstOrDefault();
            keyvalueModel.KName += "1";

            base.keyvalSer.SaveChanges();

            return View(model);
        }

    }
}
