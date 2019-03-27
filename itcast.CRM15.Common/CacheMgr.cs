using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.CRM15.Common
{
    using System.Web;

    /// <summary>
    /// 缓存管理类
    /// </summary>
    public class CacheMgr
    {
        /// <summary>
        /// 根据cacheKey获取缓存的对象
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public static T GetData<T>(string cacheKey)
        {
            return (T)HttpRuntime.Cache[cacheKey];
        }

        /// <summary>
        /// 存入的数据不过期（在IIS重启的时候才消失）
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="val"></param>
        public static void SetData(string cacheKey, object val)
        {
            HttpRuntime.Cache[cacheKey] = val;
        }

    }
}
