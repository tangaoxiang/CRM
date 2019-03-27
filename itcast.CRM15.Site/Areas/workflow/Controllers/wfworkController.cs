using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace itcast.CRM15.Site.Areas.workflow.Controllers
{
    using System.Web.WebPages;
    using WebHelper;
    using Common;
    using IServices;
    using Model.ModelViews;
    using Model;
    using EntityMapping;

    public class wfworkController : BaseController
    {
        public wfworkController(IwfWorkServices wSer, IwfWorkNodesServices nSer, IsysKeyValueServices kSer, IwfWorkBranchServices bSer)
        {
            base.workSer = wSer;
            base.keyvalSer = kSer;
            base.worknodesSer = nSer;
            base.workbranchSer = bSer;
        }

        #region 1.0 工作流列表
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult getlist()
        {
            //1.0 
            var list = workSer.QueryWhere(c => true).Select(c => new
              {
                  c.wfID,
                  c.wfTitle,
                  c.wfRemark,
                  c.wfStatus
              }).ToList();

            return Json(new { Rows = list });
        }

        #endregion

        #region 2.0 设置工作流的节点方法

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">代表工作流表wfWrok的主键wfid</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult setNodes(int id)
        {
            ViewBag.wfid = id;
            return View();
        }

        /// <summary>
        /// 负责获取id下面的所有工作流节点
        /// select * from wfWorkNodes where wfid=id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, SkipCheckPermiss]
        public ActionResult getNodes(int id)
        {
            var list = worknodesSer.QueryJoin(c => c.wfID == id, new string[] { "syskeyvalue" })
                .Select(c => new
                {
                    c.wfnID,
                    c.wfID,
                    c.wfNodeTitle,
                    c.wfnOrderNo,
                    NodeType = c.sysKeyValue.KName,
                    c.wfNodeType,
                    RoleType = c.sysKeyValue1.KName,
                    //35：执行节点才显示逻辑处理方法
                    Biz = c.wfNodeType == (int)Enums.ENodeType.ProcessNode ? c.wfnBizMethod + "(targentNum," + c.wfnMaxNum + ")" : ""
                }).ToList().OrderBy(c => c.wfnOrderNo);

            //返回
            return Json(new { Rows = list });
        }

        #endregion

        #region 2.0.1 新增工作流节点

        /// <summary>
        /// 返回工作流节点新增视图
        /// </summary>
        /// <param name="id">工作流id wfid</param>
        /// <returns></returns>
        [HttpGet, SkipCheckPermiss]
        public ActionResult addNode(int id)
        {
            setNodeType();
            setRoleType();

            return View();
        }

        #region 辅助方法
        private void setNodeType()
        {
            var list = keyvalSer.QueryWhere(c => c.KType == (int)Enums.EKeyvalueType.NodeType);
            SelectList clist = new SelectList(list, "KID", "KName");
            ViewBag.nodetypes = clist;
        }

        private void setRoleType()
        {
            var list = keyvalSer.QueryWhere(c => c.KType == (int)Enums.EKeyvalueType.RoleType);
            SelectList clist = new SelectList(list, "KID", "KName");
            ViewBag.roletypes = clist;
        }
        #endregion

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, SkipCheckPermiss]
        public ActionResult addNode(int id, wfWorkNodesView model)
        {
            try
            {
                if (ModelState.IsValid == false)
                {
                    return WriteError("实体验证失败");
                }

                //1.0 获取id工作流下面的最后一个节点
                var lastNode = worknodesSer.QueryOrderBy(c => c.wfID == id, c => c.wfnOrderNo).LastOrDefault();

                //1.0 将id赋值给wfID表示为当前节点是添加到id指定的工作流下
                model.wfID = id;
                model.fCreateTime = DateTime.Now;
                model.fCreatorID = UserMgr.GetCurrentUserInfo().uID;
                model.fUpdateTime = DateTime.Now;
                model.wfnOrderNo = lastNode != null ? lastNode.wfnOrderNo + 1 : 1;

                worknodesSer.Add(model.EntityMap());

                worknodesSer.SaveChanges();

                return WriteSuccess("节点添加成功");
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }
        }

        #endregion

        #region 2.0.2 将节点上移一位
        /// <summary>
        /// 将节点上移一位
        /// </summary>
        /// <param name="id">代表的是wfWorkNodes中的主键wfnid</param>
        /// <returns></returns>
        [HttpPost, SkipCheckPermiss]
        public ActionResult toup(int id)
        {
            try
            {
                //1.0 
                int wfnid = id;

                //2.0 将当前的节点和它的上一个节点的orderno值交换
                //2.0.1 获取id对应的节点
                var currentNode = worknodesSer.QueryWhere(c => c.wfnID == wfnid).FirstOrDefault();
                //2.0.2 判断当前上移的节点是否为结束节点，如果是则不允许移动
                if (currentNode.wfNodeType == (int)Enums.ENodeType.EndNode)
                {
                    return WriteError("结束节点不能上移");
                }

                //2.0.2 获取id对应的节点的上一个节点对象
                int preSortNo = currentNode.wfnOrderNo - 1;
                var preNode = worknodesSer.QueryWhere(c => c.wfnOrderNo == preSortNo && c.wfID == currentNode.wfID).FirstOrDefault();

                //2.0.3判断上一个节点如果是开始节点，则终止移动
                if (preNode.wfNodeType == (int)Enums.ENodeType.StartNode)
                {
                    return WriteError("上一个节点已经开始节点，不能再上移");
                }

                //3.0 交换currentNode和preNode 中的wfnOrderNo的值
                int tmpid = currentNode.wfnOrderNo;
                currentNode.wfnOrderNo = preNode.wfnOrderNo;
                preNode.wfnOrderNo = tmpid;

                //4.0 统一跟新两个数据
                using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
                {
                    worknodesSer.SaveChanges();
                    scop.Complete();
                }

                return WriteSuccess("节点上移成功");
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }
        }

        #endregion

        #region 2.0.3 将节点下移一位

        [HttpPost, SkipCheckPermiss]
        public ActionResult todown(int id)
        {
            try
            {
                //1.0 接收传入的id
                int wfnid = id;

                //2.0 根据id获取其对象
                var currentNode = worknodesSer.QueryWhere(c => c.wfnID == wfnid).FirstOrDefault();

                //3.0 判断当前节点是否为开始节点，如果是则不允许下移
                if (currentNode.wfNodeType == (int)Enums.ENodeType.StartNode)
                {
                    return WriteError("此节点是开始节点不能下移");
                }

                //4.0 获取currentNode 中的 工作流id 和 当前节点的排序号获取下一个节点
                int wfid = currentNode.wfID;
                int nextorderNO = currentNode.wfnOrderNo + 1;
                var nextNode = worknodesSer.QueryWhere(c => c.wfID == wfid && c.wfnOrderNo == nextorderNO).FirstOrDefault();

                //5.0 判断下一个节点是否为结束节点，如果是，则不允许移动
                if (nextNode.wfNodeType == (int)Enums.ENodeType.EndNode)
                {
                    return WriteError("此节点下一个节点已经是结束节点，不能再下移");
                }

                //6.0 交换两个节点的排序号
                int tmpid = currentNode.wfnOrderNo;
                currentNode.wfnOrderNo = nextNode.wfnOrderNo;
                nextNode.wfnOrderNo = tmpid;

                //7.0事务保存
                using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
                {
                    worknodesSer.SaveChanges();

                    scop.Complete();
                }

                //8.0 返回          
                return WriteSuccess("节点下移成功");
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }
        }

        #endregion

        #region 3.0 删除节点

        [HttpPost]
        public ActionResult del(int id)
        {
            try
            {
                //1.0 获取id
                int nodeid = id;

                //2.0 获取到id对应的节点实体
                var node = worknodesSer.QueryWhere(c => c.wfnID == nodeid).FirstOrDefault();

                //3.0 删除
                worknodesSer.Delete(node, true);

                //4.0 将删除的这条数据的后面所有数据的排序号减一
                var nodes = worknodesSer.QueryWhere(c => c.wfnOrderNo > node.wfnOrderNo && c.wfID == node.wfID);

                foreach (var item in nodes)
                {
                    item.wfnOrderNo--;
                }

                //5.0 开启事务统一提交
                using (System.Transactions.TransactionScope scop = new System.Transactions.TransactionScope())
                {
                    worknodesSer.SaveChanges();

                    scop.Complete();
                }

                //6.0 
                return WriteSuccess("删除成功");
            }
            catch (Exception ex)
            {
                return WriteError(ex);
            }
        }

        #endregion

        #region 4.0 设置节点分支

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">节点id</param>
        /// <returns></returns>
        [HttpGet, SkipCheckPermiss]
        public ActionResult setNodeBranch(int id)
        {
            ViewBag.wfnid = id;
            return View();
        }

        /// <summary>
        /// 获取分支列表数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, SkipCheckPermiss]
        public ActionResult getNodeBranch(int id)
        {
            //1.0 获取节点id
            int wfnid = id;

            //2.0 获取表wfWorkBranch中的当前wfnid的数据
            var blist = workbranchSer.QueryJoin(c => c.wfnID == wfnid, new string[] { "wfWorkNodes" })
                .Select(c => new
                {
                    c.wfbID,
                    c.wfnToken,
                    c.wfWorkNodes1.wfNodeTitle
                }).ToList();


            return Json(new { Rows = blist });
        }
        #endregion

        #region 5.0 新增一个分支数据

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">wfnid</param>
        /// <returns></returns>
        [HttpGet, SkipCheckPermiss]
        public ActionResult addBranch(int id)
        {
            //获取当前节点所在工作流下面的所有节点
            var nodes = worknodesSer.QueryWhere(c => c.wfnID == id).FirstOrDefault().wfWork.wfWorkNodes.ToList();

            SelectList List = new SelectList(nodes, "wfnID", "wfNodeTitle");

            ViewBag.nextNodes = List;

            return View();
        }

        [HttpPost, SkipCheckPermiss]
        public ActionResult addBranch(int id, wfWorkBranchView model)
        {
            model.wfnID = id;
            model.fCreateTime = DateTime.Now;
            model.fUpdateTime = DateTime.Now;
            model.fCreatorID = UserMgr.GetCurrentUserInfo().uID;

            workbranchSer.Add(model.EntityMap());
            workbranchSer.SaveChanges();

            return WriteSuccess("添加成功");
        }

        #endregion

    }
}
