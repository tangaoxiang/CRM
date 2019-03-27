using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.CRM15.Common
{
    public class Kits
    {
        /// <summary>
        /// 将字符串hash成MD5
        /// 格式的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string MD5Entry(string str)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
        }
    }
}
