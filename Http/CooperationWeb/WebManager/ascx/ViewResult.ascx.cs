using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebManager.ascx
{
    public partial class ViewResult : System.Web.UI.UserControl
    {
        // 存渠道ID
        protected string[] m_head = null;
        
        protected string[] m_content = null;

        // 渠道列表
        protected List<string> m_channelList;

        protected DateTime m_minT;
        protected DateTime m_maxT;

        protected QueryType m_queryType;

        protected string[] m_dbFields;

        /*
         *      传渠道ID列表
         *      传空列表，将不能查看任何渠道的数据
        */
        public List<string> channelList
        {
            set 
            {
                m_channelList = value;

                if (m_channelList.Count > 0)
                {
                    m_head = new string[1 + m_channelList.Count];
                    m_content = new string[1 + m_channelList.Count];

                    m_dbFields = new string[m_channelList.Count + 1];
                    for (int i = 0; i < m_channelList.Count; i++)
                    {
                        m_head[i + 1] = m_channelList[i];
                        m_dbFields[i + 1] = m_channelList[i];
                    }
                    m_dbFields[0] = "date";
                    m_head[0] = "日期";
                }
            }
        }

        // 设置查询时间区间
        public bool setTimeRange(string timeStr)
        {
            bool res = Tool.splitTimeStr(timeStr, ref m_minT, ref m_maxT);
            return res;
        }

        public QueryType queryType
        {
            set { m_queryType = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void startQuery()
        {
            GMUser user = (GMUser)Session["user"];
            if (m_channelList.Count == 0)
                return;

            ParamSearch param = new ParamSearch();
            param.m_minT = m_minT;
            param.m_maxT = m_maxT;
            param.m_retFields = m_dbFields;
            param.m_countEachPage = 0;
            param.m_curPage = 1;

            QueryMgr mgr = user.getSys<QueryMgr>(SysType.sysTypeQuery);
            OpRes res = mgr.doQuery(param, m_queryType, user);
            genTable(m_resultData, OpRes.opres_success, user, mgr);
        }

        private void genTable(Table table, OpRes res, GMUser user, QueryMgr mgr)
        {
            TableRow tr = new TableRow();
            table.Rows.Add(tr);
            TableCell td = null;
            if (res != OpRes.opres_success)
            {
                td = new TableCell();
                tr.Cells.Add(td);
              //  td.Text = OpResMgr.getInstance().getResultString(res);
                return;
            }

            int i = 0, j = 0;
            // 表头
            for (i = 0; i < m_head.Length; i++)
            {
                td = new TableCell();
                tr.Cells.Add(td);

                if (i > 0)
                {
                    ChannelInfo info = Channel.getInstance().getChannel(m_head[i]);
                    if (info != null)
                    {
                        td.Text = info.channelName;
                    }
                    else
                    {
                        td.Text = "";
                    }
                }
                else
                {
                    td.Text = m_head[i];
                }
            }

            int k = 1;

            List<ResultItem> qresult = (List<ResultItem>)mgr.getQueryResult(m_queryType);

            for (i = 0; i < qresult.Count; i++)
            {
                tr = new TableRow();
                
                table.Rows.Add(tr);

                if ((i & 1) == 0)
                {
                    tr.CssClass = "alt";
                }

                m_content[0] = qresult[i].m_time;
                int count = m_head.Length;
                for (k = 1; k < count; k++)
                {
                    m_content[k] = qresult[i].getValue(m_head[k]).ToString();
                }

                for (j = 0; j < m_head.Length; j++)
                {
                    td = new TableCell();
                    tr.Cells.Add(td);                   
                    td.Text = m_content[j];
                }
            }
        }
    }
}