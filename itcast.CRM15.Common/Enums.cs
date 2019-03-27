using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.CRM15.Common
{
    /// <summary>
    /// 枚举管理类
    /// </summary>
    public class Enums
    {
        /// <summary>
        /// 负责标记ajax请求以后的json数据状态
        /// </summary>
        public enum EAjaxState
        {
            /// <summary>
            /// 成功
            /// </summary>
            sucess = 0,
            /// <summary>
            /// 错误异常
            /// </summary>
            error = 1,
            /// <summary>
            /// 未登录
            /// </summary>
            nologin = 2
        }

        public enum EState
        {
            /// <summary>
            /// 正常
            /// </summary>
            Nomal = 0,
            /// <summary>
            /// 停用(删除)
            /// </summary>
            Stop = 1
        }

        public enum ENodeType
        {
            /// <summary>
            /// 开始节点
            /// </summary>
            StartNode = 34,
            /// <summary>
            /// 执行节点
            /// </summary>
            ProcessNode = 35,
            /// <summary>
            /// 结束节点
            /// </summary>
            EndNode = 36
        }

        public enum EKeyvalueType
        {
            /// <summary>
            /// 组织结构的类型
            /// </summary>
            OrganType = 1,
            /// <summary>
            /// 角色类型
            /// </summary>
            RoleType = 2,
            /// <summary>
            /// 节点类型
            /// </summary>
            NodeType = 3
        }

        public enum ERequestFormStatus
        {
            /// <summary>
            /// 处理中
            /// </summary>
            Processing = 40,
            /// <summary>
            /// 驳回上级
            /// </summary>
            Back = 41,
            /// <summary>
            /// 拒绝
            /// </summary>
            Reject = 42,
            /// <summary>
            /// 通过
            /// </summary>
            Pass = 43
        }
    }
}
