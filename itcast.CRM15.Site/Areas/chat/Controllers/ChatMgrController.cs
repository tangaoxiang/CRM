using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace itcast.CRM15.Site.Areas.chat.Controllers
{
    using Chat.Model;
    using WebHelper;
    [SkipCheckPermiss]
    public class ChatMgrController : BaseController
    {
        public ActionResult GetMessage()
        {
            //1.0 获取当前登录用户的id
            int userid = UserMgr.GetCurrentUserInfo().uID;

            //2.0 调用WCF服务获取当前用户的消息
            ChatMgrClient client = new ChatMgrClient();
            var msgs = client.GetMessage(userid).Select(c => new
            {
                frn = c.FromRealName,
                st = c.SendTime,
                fui = c.FromUserId,
                mb = c.MessageBody
            });

            return WriteSuccess("ok", msgs);
        }

        public ActionResult SetMessage()
        {
            //1.0 接收ajax发送过来的数据
            string touserid = Request.Params["touserid"];
            string torealname = Request.Params["torealname"];
            string msgbody = Request.Params["msgbody"];

            //2.0 参数合法性验证

            ChatMsg msg = new ChatMsg()
            {
                ToUserId = int.Parse(touserid),
                ToRealName = torealname,
                FromRealName = UserMgr.GetCurrentUserInfo().uRealName
                ,
                FromUserId = UserMgr.GetCurrentUserInfo().uID,
                SendTime = DateTime.Now,
                MessageBody = msgbody
            };

            //3.0包装成chatmsg对象发送给WCF聊天服务
            ChatMgrClient c = new ChatMgrClient();
            c.SetMessage(msg);

            return WriteSuccess("消息发送成功");
        }

        public ActionResult GetHistoryMessage()
        {
            //1.0 获取时间间隔
            string begintime = Request.Params["btime"];
            string endtime = Request.Params["etime"];

            //2.0 进行时间格式的合法性验证

            //3.0调用wcf
            ChatMgrClient client = new ChatMgrClient();
            var list = client.GetHistoryMessage(UserMgr.GetCurrentUserInfo().uID, DateTime.Parse(begintime), DateTime.Parse(endtime));
            var nlist = list.Select(c => new
            {
                frn = c.FromRealName,
                st = c.SendTime,
                fui = c.FromUserId,
                mb = c.MessageBody
            });

            return WriteSuccess("ok", nlist);
        }
    }
}
