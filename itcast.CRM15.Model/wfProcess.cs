//------------------------------------------------------------------------------
// <auto-generated>
//    此代码是根据模板生成的。
//
//    手动更改此文件可能会导致应用程序中发生异常行为。
//    如果重新生成代码，则将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace itcast.CRM15.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class wfProcess
    { 
        public int wfPID { get; set; }
        public int wfRFID { get; set; }
        public int wfProcessor { get; set; }
        public int wfRFStatus { get; set; }
        public string wfPDescription { get; set; }
        public int wfnID { get; set; }
        public string wfPExt1 { get; set; }
        public Nullable<int> wfPExt2 { get; set; }
        public int fCreatorID { get; set; }
        public System.DateTime fCreateTime { get; set; }
        public Nullable<System.DateTime> fUpdateTime { get; set; }
    
        public virtual sysKeyValue sysKeyValue { get; set; }
        public virtual wfRequestForm wfRequestForm { get; set; }
        public virtual wfWorkNodes wfWorkNodes { get; set; }
    }
}
