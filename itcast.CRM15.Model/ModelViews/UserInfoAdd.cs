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
    /// 专门给新增视图使用
    /// </summary>
    public class UserInfoAdd
    {
        public int uID { get; set; }
        [DisplayName("用户名"), Required(ErrorMessage = "非空")]
        [System.Web.Mvc.Remote("checkuser", "userinfo", HttpMethod = "post", ErrorMessage = "用户名已经存在")]
        public string uLoginName { get; set; }
        public string uLoginPWD { get; set; }
        [DisplayName("真实姓名")]
        public string uRealName { get; set; }
        public string uTelphone { get; set; }
        [DisplayName("手机"), Required(ErrorMessage = "非空")]
        public string uMobile { get; set; }
        [DisplayName("电邮"), Required(ErrorMessage = "非空")]
        public string uEmial { get; set; }
        public string uQQ { get; set; }
        [DisplayName("性别")]
        public int uGender { get; set; }
        [DisplayName("状态")]
        public int uStatus { get; set; }
        [DisplayName("所属公司")]
        public int uCompanyID { get; set; }
        [DisplayName("所属部门")]
        public Nullable<int> uDepID { get; set; }
        [DisplayName("所属工作组")]
        public Nullable<int> uWorkGroupID { get; set; }
        public string uRemark { get; set; }
        public int uCreatorID { get; set; }
        public System.DateTime uCreateTime { get; set; }
        public Nullable<int> uUpdateID { get; set; }
        public System.DateTime uUpdateTime { get; set; }
    }
}
