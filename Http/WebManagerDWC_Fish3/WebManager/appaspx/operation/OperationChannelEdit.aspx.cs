using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;

namespace WebManager.appaspx.operation
{
    public partial class OperationChannelEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck(RightDef.OP_CHANNEL_EDIT, Session, Response);

            if (!IsPostBack)
            {
                genSrcChannel();
                genTestChannel();
            }
        }

        protected void addTestChannelSelf(object sender, EventArgs e)
        {
            _addTestChannel(m_selfDefChannel);
        }

        protected void addTestChannelBuilt(object sender, EventArgs e)
        {
            _addTestChannel(m_builtInChanngel);
        }

        protected void _addTestChannel(CheckBoxList checkList)
        {
            ParamAddChannel param = new ParamAddChannel();
            param.m_isAdd = true;

            /*foreach (var node in trvAllChannel.CheckedNodes)
            {
                TreeNode t = (TreeNode)node;
                param.m_channels.Add(t.Value);
            }*/

            foreach (ListItem item in checkList.Items)
            {
                if (item.Selected)
                {
                    param.m_channels.Add(item.Value);
                }
            }

            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeEditChannel, user);
            if (res == OpRes.opres_success)
            {
                // genSrcChannel();
                genTestChannel();
            }
        }

        protected void delTestChannel(object sender, EventArgs e)
        {
            ParamAddChannel param = new ParamAddChannel();
            param.m_isAdd = false;

            foreach (var node in testChannelList.CheckedNodes)
            {
                TreeNode t = (TreeNode)node;
                param.m_channels.Add(t.Value);
            }

            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            DyOpAddChannel c = (DyOpAddChannel)mgr.getDyOp(DyOpType.opTypeEditChannel);
            OpRes res = mgr.doDyop(param, DyOpType.opTypeEditChannel, user);
            if (res == OpRes.opres_success)
            {
               // genSrcChannel();
                genTestChannel();
            }
        }

        protected void genSrcChannel()
        {
          //  trvAllChannel.Nodes.Clear();
           /* AccessDb.getAccDb().setConnDb("channel.mdb");

            string sql = "select* from channel where enable=true order by ID;";
            OleDbDataReader r = AccessDb.getAccDb().startQuery(sql);
            if (r != null)
            {
                while (r.Read())
                {
                    TreeNode node = new TreeNode();
                    node.NavigateUrl = "#";
                    node.Value = r["channelNo"].ToString();
                    node.Text = r["channelName"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;渠道编号:" + node.Value;
                    trvAllChannel.Nodes.Add(node);
                }
            }
            AccessDb.getAccDb().end();*/

            List<ChannelInfo> cList = Channel.getInstance().m_cList;
            foreach (var c in cList)
            {
                ListItem item = new ListItem(c.channelName + "&nbsp;&nbsp;&nbsp;渠道编号:" + c.channelNo,
                    c.channelNo);

                if (c.ID >= 1000)
                {
                    m_selfDefChannel.Items.Add(item);
                }
                else
                {
                    m_builtInChanngel.Items.Add(item);
                }
            }
            return;

            foreach(var c in cList)
            {
                TreeNode node = new TreeNode();
                node.NavigateUrl = "#";
                node.Value = c.channelNo;
                node.Text = c.channelName + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;渠道编号:" + node.Value;
                //trvAllChannel.Nodes.Add(node);
            }
        }

        protected void genTestChannel()
        {
            testChannelList.Nodes.Clear();

            GMUser user = (GMUser)Session["user"];
            DyOpMgr mgr = user.getSys<DyOpMgr>(SysType.sysTypeDyOp);
            DyOpAddChannel c = (DyOpAddChannel)mgr.getDyOp(DyOpType.opTypeEditChannel);
            string str = c.getTestChannel(user);
           /* if (str != "")
            {
                AccessDb.getAccDb().setConnDb("channel.mdb");

                string sql = string.Format("select* from channel where channelNo in ({0})", str);
                OleDbDataReader r = AccessDb.getAccDb().startQuery(sql);
                if (r != null)
                {
                    while (r.Read())
                    {
                        TreeNode node = new TreeNode();
                        node.NavigateUrl = "#";
                        node.Value = r["channelNo"].ToString();
                        node.Text = r["channelName"].ToString() + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;渠道编号:" + node.Value;
                        testChannelList.Nodes.Add(node);
                    }
                }
                AccessDb.getAccDb().end();
            }*/

            string[] arr = str.Split(',');
            foreach (var s in arr)
            {
                ChannelInfo info = Channel.getInstance().getChannel(s);
                if (info != null)
                {
                    TreeNode node = new TreeNode();
                    node.NavigateUrl = "#";
                    node.Value = info.channelNo;
                    node.Text = info.channelName + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;渠道编号:" + node.Value;
                    testChannelList.Nodes.Add(node);
                }
            }
        }
    }
}