using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;

// 线程当前状态
public enum ThreadState
{
    state_idle,         // 空闲中
    state_busy,         // 忙碌中
}

// 导出Excel工作线程
public class WorkThreadExport : ServiceBase
{
    private ThreadState m_curState;
    private bool m_run = true;

    // 任务队列
    private Queue<ExportParam> m_taskQue = new Queue<ExportParam>();
    private object m_lockQue = new object();

    private DateTime m_lastCheckTime = DateTime.Now;
    private const int CHECK_DAY = 1; // 1
    private const int DELETE_DAY = 2; // 2小时删除导出的表格
    private const int MAX_TASK_COUNT = 5000; //5000;  // 最大任务数量

    private ExportMgr m_exportMgr = new ExportMgr();

    public WorkThreadExport()
    {
        m_sysType = ServiceType.serviceTypeExportExcel;
    }

    public override void initService()
    {
        m_exportMgr.init();

        m_curState = ThreadState.state_idle;
        Thread t = new Thread(new ThreadStart(run));
        t.Start();
    }

    public override void exitService()
    {
        m_run = false;
    }

    public override bool isBusy()
    {
        if (m_curState == ThreadState.state_busy)
            return true;

        if (m_taskQue.Count > 0)
            return true;

        return false;
    }

    public override int doService(ServiceParam param)
    {
        if (param == null)
            return (int)RetCode.ret_error;

        ExportParam ep = (ExportParam)param;
        if (m_taskQue.Count >= MAX_TASK_COUNT)
            return (int)RetCode.ret_busy;

        // 写入队列
        lock (m_lockQue)
        {
            m_taskQue.Enqueue(ep);
        }

        return (int)RetCode.ret_success;
    }

    private void run()
    {
        while (m_run)
        {
            switch (m_curState)
            {
                case ThreadState.state_idle:
                    {
                        if (m_taskQue.Count > 0)
                        {
                            m_curState = ThreadState.state_busy;
                        }
                        else
                        {
                            delExcel();
                        }
                        Thread.Sleep(1000);
                    }
                    break;
                case ThreadState.state_busy:
                    {
                        ExportParam data = null;
                        while (m_taskQue.Count > 0)
                        {
                            lock (m_lockQue)
                            {
                                data = m_taskQue.Dequeue();
                            }

                            if (!m_run)
                            {
                                break;
                            }

                            if (data != null)
                            {
                                m_exportMgr.export(data);
                            }
                        }

                        m_curState = ThreadState.state_idle;
                    }
                    break;
            }
        }
    }

    private void delExcel()
    {
        DateTime now = DateTime.Now;
        TimeSpan span = now - m_lastCheckTime;
        if (span.TotalHours < CHECK_DAY)
        {
            return;
        }

        string dirPath = m_exportMgr.m_exportDir;
        DirectoryInfo dinfo = null;
        DirectoryInfo[] subDirs = null;
        try
        {
            dinfo = new DirectoryInfo(dirPath);
            subDirs = dinfo.GetDirectories();
        }
        catch (System.Exception ex)
        {
            return;	
        }
        
        foreach (var info in subDirs)
        {
            FileInfo[] fileList = info.GetFiles();

            foreach (var file in fileList)
            {
                span = now - file.LastWriteTime;
                if (span.TotalHours >= DELETE_DAY)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (System.Exception ex)
                    {	
                    }   
                }
            }
        }
    }
}



