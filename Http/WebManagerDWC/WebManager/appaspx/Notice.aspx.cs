using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;

namespace WebManager.appaspx
{
    // 发布公告
    public partial class Notice : System.Web.UI.Page
    {
        private static string[] s_head = new string[] {"选择", "标题", "内容", "生成时间", "显示天数", "是否显示" };
        private static string[] s_content = new string[s_head.Length];
        // 所选择的checkbox
        private string m_selectStr = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            RightMgr.getInstance().opCheck("notice", Session, Response);

            if (IsPostBack)
            {
                m_selectStr = Request["aaa"];
                if (m_selectStr == null)
                {
                    m_selectStr = "";
                }
                return;
            }

            genTable(false);
        }

        // 发布公告
        protected void onPublishNotice(object sender, EventArgs e)
        {
            try
            {
                int day = 0;
                if (m_day.Text != "")
                {
                    day = Convert.ToInt32(m_day.Text);
                }
                GMUser user = (GMUser)Session["user"];
                NoticeMgr.getInstance().addNotice(m_title.Text, m_content.Text, day, user);
                genTable(false);
                m_res.InnerHtml = user.getOpResultString();
            }
            catch (System.Exception ex)
            {
                LOGW.Info(ex.ToString());
                m_res.InnerHtml = OpResMgr.getInstance().getResultString(OpRes.op_res_failed);
            }
        }

        private void genTable(bool issel)
        {
            NoticeTable.GridLines = GridLines.Both;
            // 添加标题行
            TableRow tr = new TableRow();
            NoticeTable.Rows.Add(tr);
            int col = s_head.Length;
            int i = 0;
            for (; i < col; i++)
            {
                TableCell td = new TableCell();
                tr.Cells.Add(td);
                td.Text = s_head[i];
            }

            // 添加内容
            List<CSystemAnnounce> nlist = NoticeMgr.getInstance().getAllNotice((GMUser)Session["user"]);
            if (nlist != null)
            {
                int j = 0;
                int row = nlist.Count;

                for (i = 0; i < row; i++)
                {
                    s_content[1] = nlist[i].mTitle;
                    s_content[2] = nlist[i].mContent;
                    s_content[3] = nlist[i].mGenTime;
                    s_content[4] = nlist[i].mDayAmount.ToString();
                    s_content[5] = nlist[i].mIsVisible == true ? "是" : "否";
                    // 生成行数据
                    tr = new TableRow();
                    NoticeTable.Rows.Add(tr);
                    for (j = 0; j < col; j++)
                    {
                        TableCell td = new TableCell();
                        tr.Cells.Add(td);
                        if (j == 0)
                        {
                            td.Text = "<input type= \"checkbox\" name = \"aaa\"" +  getChecked(issel) + " value= " + "\"" + i.ToString() + "\"" + " runat=\"server\" />";
                        }
                        else
                        {
                            td.Text = s_content[j];
                        }
                    }
                }
            }
        }

        private string getChecked(bool issel)
        {
            return issel ? "checked=\"true\"" : "";
        }

        // 全部选择
        protected void onSelectAll(object sender, EventArgs e)
        {
            genTable(true);
        }

        // 删除选择中的列表
        protected void onDelete(object sender, EventArgs e)
        {
            if (m_selectStr != "")
            {
                char[] sp = { ',' };
                string[] arr = m_selectStr.Split(sp);
                int[] index = new int[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    index[i] = Convert.ToInt32(arr[0]);
                }
                NoticeMgr.getInstance().deleteNotice(index, (GMUser)Session["user"]);
            }
            genTable(false);
        }

        // 激活某个公告，只激活第一个选择的公告
        protected void onActivate(object sender, EventArgs e)
        {
            if (m_selectStr == "")
            {
                genTable(false);
                return;
            }

            char[] sp = { ',' };
            string[] arr = m_selectStr.Split(sp);
            try
            {
                int index = Convert.ToInt32(arr[0]);
                NoticeMgr.getInstance().activateNotice(index, (GMUser)Session["user"]);
                genTable(false);
            }
            catch (System.Exception ex)
            {
                LOGW.Info(ex.ToString());
            }
        }

        // 隐藏所有公告
        protected void onHideAllNotice(object sender, EventArgs e)
        {
            NoticeMgr.getInstance().hideAllNotice((GMUser)Session["user"]);
            genTable(false);
        }
    }
}