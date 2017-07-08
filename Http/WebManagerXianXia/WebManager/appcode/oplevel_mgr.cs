using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text;

// 操作层级管理
public class OpLevelMgr : SysBase
{
    //private TreeView m_tree = new TreeView();
    private CTree m_tree = new CTree();

   // private Dictionary<string, TreeNode> m_map = new Dictionary<string, TreeNode>();
    private Dictionary<string, CTreeNode> m_map = new Dictionary<string, CTreeNode>();

    private StringBuilder m_textBuilder = new StringBuilder();

    public OpLevelMgr()
    {
        m_sysType = SysType.sysTypeOpLevel;
    }

    public override void initSys()
    {
        reset();
    }

    public void reset()
    {
        m_tree.Nodes.Clear();
        m_map.Clear();
    }

    // 添加根账号
    public void addRootAcc(string rootAcc, URLParam param = null)
    {
        reset();
        if (m_tree.Nodes.Count > 0)
        {
            LOGW.Info("addRootAcc错误........");
            return;
        }

        CTreeNode node = new CTreeNode();
        node.Value = rootAcc;
        node.Tag = param;
        if (param != null)
        {
            param.m_text = node.Value;
            param.m_className = "cAgentLevelStr";
        }
        m_tree.Nodes.Add(node);
        m_map.Add(rootAcc, node);
    }

    // 添加某个指定账号的下级账号
    // owner指定账号 subAcc下级账号
    public void addSub(string owner, string subAcc, URLParam param = null)
    {
        if (!m_map.ContainsKey(owner))
            return;

        if (m_map.ContainsKey(subAcc))
            return;

        bool exist = false;
        CTreeNode node = m_map[owner];
        foreach(CTreeNode child in node.ChildNodes.Values)
        {
            if (child.Value == subAcc)
            {
                exist = true;
                break;
            }
        }

        if (!exist)
        {
            CTreeNode n = new CTreeNode();
            n.Value = subAcc;
            n.Tag = param;
            if (param != null)
            {
                param.m_text = n.Value;
                param.m_className = "cAgentLevelStr";
            }
            node.ChildNodes.Add(n);
            m_map.Add(subAcc, n);
        }
    }

    // 返回目标账号为dstAcc的层级串
    public string getCurLevelStr(string dstAcc)
    {
        if (!m_map.ContainsKey(dstAcc))
            return "";

        m_textBuilder.Remove(0, m_textBuilder.Length);

        CTreeNode node = m_map[dstAcc];
        Stack<string> sk = new Stack<string>();

        while (node != null)
        {
            if (node.Tag != null && sk.Count > 0)
            {
                URLParam param = (URLParam)node.Tag;
                sk.Push(Tool.genHyperlink(param));
            }
            else
            {
                sk.Push(node.Value);
            }
            
            node = node.Parent;
        }

        while (sk.Count > 0)
        {
            m_textBuilder.Append(sk.Pop());
            m_textBuilder.Append("&gt;");
        }
        return m_textBuilder.ToString();
    }
}

//////////////////////////////////////////////////////////////////////////
class CTree : CTreeNode
{
    private CTreeNodeCollection m_nodes;

    public CTree()
    {
        m_nodes = new CTreeNodeCollection(null);
    }

    public CTreeNodeCollection Nodes
    {
        get { return m_nodes; }
    }
}

class CTreeNode
{
    private string m_value;
    private object m_tag;
    private CTreeNode m_parent;

    private CTreeNodeCollection m_nodes;

    public CTreeNode()
    {
        m_nodes = new CTreeNodeCollection(this);
    }

    public CTreeNode Parent
    {
        get { return m_parent; }
        set { m_parent = value; }
    }

    public CTreeNodeCollection ChildNodes
    {
        get { return m_nodes; }
    }

    public string Value
    {
        get { return m_value; }
        set { m_value = value; }
    }

    public object Tag
    {
        get { return m_tag; }
        set { m_tag = value; }
    }
}

class CTreeNodeCollection
{
    private Dictionary<string, CTreeNode> m_nodes = new Dictionary<string, CTreeNode>();

    private CTreeNode m_owner;

    public CTreeNodeCollection(CTreeNode owner)
    {
        m_owner = owner;
    }

    public void Add(CTreeNode node)
    {
        node.Parent = m_owner;
        m_nodes.Add(node.Value, node);
    }

    public void Clear()
    {
        foreach (var n in m_nodes.Values)
        {
            n.ChildNodes.Clear();
        }
        m_nodes.Clear();
    }

    public int Count
    {
        get { return m_nodes.Count; }
    }

    public Dictionary<string, CTreeNode>.ValueCollection Values
    {
        get { return m_nodes.Values; }
    }
}


