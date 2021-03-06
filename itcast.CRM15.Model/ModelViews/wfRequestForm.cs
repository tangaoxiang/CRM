//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace itcast.CRM15.Model.ModelViews
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    public partial class wfRequestFormView
    {


        public int wfRFID { get; set; }
        [DisplayName("所属工作流")]
        public int wfID { get; set; }
        [DisplayName("标题"), Required(ErrorMessage = "标题非空")]
        public string wfRFTitle { get; set; }
        [DisplayName("详细描述"), Required(ErrorMessage = "详细描述非空")]
        public string wfRFRemark { get; set; }
        [DisplayName("优先级")]
        public int wfRFPriority { get; set; }
        [DisplayName("状态")]
        public int wfRFStatus { get; set; }
        public string wfRFExt1 { get; set; }
        public Nullable<int> wfRFExt2 { get; set; }
        public string wfRFLogicSymbol { get; set; }
        [DisplayName("天数/金额")]
        public decimal wfRFNum { get; set; }
        public int fCreatorID { get; set; }
        public System.DateTime fCreateTime { get; set; }
        public Nullable<System.DateTime> fUpdateTime { get; set; }

        public virtual sysKeyValueView sysKeyValue { get; set; }
        public virtual sysKeyValueView sysKeyValue1 { get; set; }
        public virtual sysUserInfoView sysUserInfo { get; set; }
        public virtual ICollection<wfProcessView> wfProcess { get; set; }
        public virtual wfWorkView wfWork { get; set; }
    }
}
