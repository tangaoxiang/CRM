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
    
      /// <summary>
      /// 负责每个数据表的数据操作
      /// </summary>
    public partial class sysMenusServices :BaseServices<sysMenus>,IsysMenusServices
    {
       IsysMenusRepository dal;
      #region 定义构造函数，接收autofac将数据仓储层的具体实现类的对象注入到此类中
    	public sysMenusServices(IsysMenusRepository dal)
    	{
    		this.dal = dal;
    		base.baseDal = dal;
    	}
      #endregion
    
    
       #region 针对此表的特殊操作写在此处
            
      #endregion
    }
}
