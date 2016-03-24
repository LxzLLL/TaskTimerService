using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using XLPluginInterface;
using Quartz;
using Quartz.Impl;
using Quartz.Job;

namespace XLPluginManager
{
    /// <summary>
    /// 作者：Arvin
    /// 日期：2015/11/12 14:23:17
    /// 描述：定时任务的组件管理类，管理TimerTask是否启动暂停或终止等
    /// </summary>
    public class TimerManager
    {
        #region 属性

        public List<TimerTask> TimerTaskList
        {
            get
            {
                return _timerTaskList;
            }
        }
        #endregion

        #region 字段
        private List<TimerTask> _timerTaskList = new List<TimerTask>();
        private static readonly TimerManager _tiemrMarager = new TimerManager();
        #endregion

        /// <summary>
        /// 私有TimerManager
        /// </summary>
        private TimerManager()
        {

        }

        /// <summary>
        /// 获取单例TimerManager
        /// </summary>
        /// <returns></returns>
        public static TimerManager GetTimerManagerInstance()
        {
            return _tiemrMarager;
        }

        /// <summary>
        /// 创建一条任务
        /// </summary>
        /// <param name="strCron">cron表达式</param>
        /// <param name="pluginTimer">IPluginTimer对象</param>
        /// <param name="taskSendType">定时任务后的发送方式，string</param>
        /// <returns>返回任务TimerTask对象</returns>
        public TimerTask CreateTask( string strCron, IPluginTimer pluginTimer, string taskSendType )
        {
            SendType st;
            if ( !Enum.TryParse( taskSendType, out st ) )
            {
                //如果无法转换  则表示不发送状态
                st = SendType.nosend;
            }
            TimerTask timerTask = new TimerTask( strCron, pluginTimer, st );
            timerTask.CreateTimerTask();
            this._timerTaskList.Add( timerTask );
            return timerTask;
        }

        /// <summary>
        /// 指定TimerTask对象启动任务
        /// </summary>
        /// <param name="timerTask"></param>
        public void TaskStart( TimerTask timerTask )
        {
            if(timerTask!=null&&timerTask.Status!=TaskStatus.running)
            {
                timerTask.Start();
            }
        }
        /// <summary>
        /// 制定TimerTask对象停止任务
        /// </summary>
        /// <param name="timerTask"></param>
        public void TaskStop( TimerTask timerTask )
        {
            if ( timerTask != null && timerTask.Status != TaskStatus.stopped )
            {
                timerTask.Stop();
            }
        }

        /// <summary>
        /// 删除所有的Tasks，在服务停止时使用
        /// </summary>
        public void DeleteTasks()
        {
            if(this._timerTaskList==null||this._timerTaskList.Count<=0)
            {
                foreach(TimerTask tt in this._timerTaskList)
                {
                    tt.Scheduler.DeleteJob( tt.JobDetail.Key );
                }
            }
        }
        
        /// <summary>
        /// 更新某个TimerTask对象的定时字符串
        /// </summary>
        /// <param name="timerTask">定时任务对象</param>
        /// <param name="strCron">定时字符串</param>
        public void UpdateCron( TimerTask timerTask,string strCron )
        {
            if(timerTask!=null)
            {
                timerTask.UpdateCron( strCron );
            }
        }
        
    }
}
