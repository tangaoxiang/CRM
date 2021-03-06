//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace itcast.CRM15.Services
{
    using System;
    using System.Collections.Generic;

    using itcast.CRM15.Model;
    using itcast.CRM15.IServices;
    using itcast.CRM15.IRepository;
    using itcast.CRM15.Common;

    /// <summary>
    /// 负责每个数据表的数据操作
    /// </summary>
    public partial class sysPermissListServices : BaseServices<sysPermissList>, IsysPermissListServices
    {
        IsysPermissListRepository dal;
        #region 定义构造函数，接收autofac将数据仓储层的具体实现类的对象注入到此类中
        public sysPermissListServices(IsysPermissListRepository dal)
        {
            this.dal = dal;
            base.baseDal = dal;
        }
        #endregion


        #region 针对此表的特殊操作写在此处

        /// <summary>
        /// 负责将用户权限按钮数据进行缓存操作，以及以后直接从缓存中获取
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public List<Usp_GetFunctionsForUser15_Result> GetFunctionsForUserByCache(int userid)
        {
            //注意：缓存key一定是每个用户有一个，彼此不重复
            string cacheKey = Keys.PermissFunctionsForUser + userid;

            //1.0 从缓存中根据用户获取其权限按钮数据
            object data = CacheMgr.GetData<List<Usp_GetFunctionsForUser15_Result>>(cacheKey);
            if (data == null)
            {
                //从数据库获取一份权限按钮数据
                var prmisslist = baseDal.RunProc<Usp_GetFunctionsForUser15_Result>("Usp_GetFunctionsForUser15 " + userid);
                //将数据加入缓存
                CacheMgr.SetData(cacheKey, prmisslist);

                return prmisslist;
            }

            return data as List<Usp_GetFunctionsForUser15_Result>;
        }

        #endregion
    }
}
