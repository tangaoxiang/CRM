using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace itcast.CRM15.Model.ModelViews
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// 负责处理登录请求的
    /// </summary>
    public class LoginInfo
    {
        [DisplayName("账号"), Required(ErrorMessage = "账号非空")]
        public string uLoginName { get; set; }
        [DisplayName("密码"), Required(ErrorMessage = "密码非空")]
        public string uLoginPWD { get; set; }
        [DisplayName("验证码"), Required(ErrorMessage = "验证码非空")]
        public string VCode { get; set; }
        /// <summary>
        /// 是否记住三天的功能
        /// </summary>
        public bool IsMember { get; set; }
    }
}
