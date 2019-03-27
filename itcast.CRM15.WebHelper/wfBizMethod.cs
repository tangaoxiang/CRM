using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.CRM15.WebHelper
{
    public class wfBizMethod
    {
        /// <summary>
        /// 大于
        /// </summary>
        /// <param name="targetNum"></param>
        /// <param name="maxNum"></param>
        /// <returns></returns>
        public bool Gt(decimal targetNum, decimal maxNum)
        {
            return targetNum > maxNum;
        }

        public bool Lt(decimal targetNum, decimal maxNum)
        {
            return targetNum < maxNum;
        }

        public bool Eq(decimal targetNum, decimal maxNum)
        {
            return targetNum == maxNum;
        }

        public bool GEq(decimal targetNum, decimal maxNum)
        {
            return targetNum >= maxNum;
        }

        public bool LEq(decimal targetNum, decimal maxNum)
        {
            return targetNum <= maxNum;
        }
    }
}
