using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using XLPluginInterface;
using Quartz;
using Quartz.Impl;
using Quartz.Job;

namespace XLPluginManager
{
    /// <summary>
    /// 作者：Arvin
    /// 日期：2015/11/13 10:21:31
    /// 描述：一条定时任务实体
    /// </summary>
    public class TimerTask
    {
        #region 属性
        /// <summary>
        /// Cron表达式，即任务表达式
        /// </summary>
        public string Cron { get; private set; }
        /// <summary>
        /// 发送方式
        /// </summary>
        public SendType TaskSendType { get; private set; }
        /// <summary>
        /// Timer插件实体（应该传一个对象，表示此用什么方式将run结果发送出去，有些任务是仅仅执行，有些需要发送，需定义一个实体）
        /// </summary>
        public IPluginTimer PluginTimer { get; private set; }
        /// <summary>
        /// 调度器
        /// </summary>
        public IScheduler Scheduler{get;private set;}
        /// <summary>
        /// 任务
        /// </summary>
        public IJobDetail JobDetail{get;private set;}
        /// <summary>
        /// 触发器
        /// </summary>
        public ITrigger Trigger{get;private set;}

        /// <summary>
        /// 任务的状态，TaskStatus枚举
        /// </summary>
        public TaskStatus Status
        {
            get
            {
                return _status;
            }
        }
        /// <summary>
        /// 上次执行时间
        /// </summary>
        public string LastTime { get; private set; }
        /// <summary>
        /// 下次执行时间
        /// </summary>
        public string NextTime { get; private set; }
        /// <summary>
        /// 执行次数，仅表示启动到现在为止的执行次数，服务重启后置0（需存入数据库）
        /// </summary>
        public int ExecuteCount { get; private set; }

        #endregion

        #region 字段
        //任务状态私有字段
        private TaskStatus _status = TaskStatus.nocreate;
        private string _strJobName;
        private string _strTriggerName;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strCron">cron表达式</param>
        /// <param name="pluginTimer">IPluginTimer对象</param>
        /// <param name="taskSendType">定时任务后的发送方式，SendType枚举</param>
        public TimerTask( string strCron, IPluginTimer pluginTimer, SendType taskSendType )
        {
            this.Cron = strCron;
            this.PluginTimer = pluginTimer;
            this.TaskSendType = taskSendType;

            this._strJobName = PluginTimer.PluginName + "Job";
            this._strTriggerName = PluginTimer.PluginName + "Trigger";
        }

        /// <summary>
        /// 创建定时任务，只是创建，但未启动
        /// </summary>
        public void CreateTimerTask()
        {
            //创建 调度器
            this.Scheduler = StdSchedulerFactory.GetDefaultScheduler();
            CreateJobDetail();
            CreateTrigger();
            this.Scheduler.ScheduleJob( this.JobDetail, this.Trigger );
            
        }

        #region private
        /// <summary>
        /// 创建jobDetail
        /// </summary>
        private IJobDetail CreateJobDetail()
        {
            //创建job
            this.JobDetail = JobBuilder.Create<TaskJob>()
                    .WithIdentity( this._strJobName, this._strJobName )
                    .UsingJobData( new JobDataMap( new Dictionary<string, Func<string,string,string>> { { "Timer", this.RunCallBack } } ) )
                    .UsingJobData( new JobDataMap( new Dictionary<string, SendType> { { "SendType", this.TaskSendType } } ) )
                    .Build();
            return this.JobDetail;
        }

        /// <summary>
        /// 创建触发器
        /// </summary>
        private ITrigger CreateTrigger()
        {
            //创建触发器，创建后立即开始，需要依赖Scheduler
            this.Trigger = TriggerBuilder.Create()
                .WithIdentity( this._strTriggerName, this._strTriggerName )
                .WithCronSchedule( this.Cron )
                .StartNow()
                .ForJob(this.JobDetail)
                .Build();
            return this.Trigger;
        }

        /// <summary>
        /// 执行时的回调，在TaskJob对象中调用
        /// 更新执行次数、上次执行时间、下次执行时间
        /// </summary>
        /// <returns></returns>
        private string RunCallBack( string strLastTime,string strNextTime )
        {
            this.ExecuteCount++;
            this.LastTime = strLastTime;
            this.NextTime = strNextTime;
            return this.PluginTimer.Run();
        }

        #endregion

        /// <summary>
        /// 启动任务
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            this.Scheduler.Start();
            _status = this.Scheduler.IsStarted?TaskStatus.running:_status;
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        public void Stop()
        {
            //不等待任务执行完就shutdown
            this.Scheduler.Shutdown( false );
            _status = this.Scheduler.IsShutdown ? TaskStatus.stopped : _status;
        }
        /// <summary>
        /// 更新定时Cron
        /// </summary>
        /// <param name="strCron"></param>
        public void UpdateCron( string strCron )
        {
            this.Cron = strCron;
            this.Scheduler.RescheduleJob( this.Trigger.Key, CreateTrigger() );
            this.Scheduler.ResumeTrigger( this.Trigger.Key );
        }

    }

    /// <summary>
    /// 任务状态的枚举
    /// </summary>
    public enum TaskStatus
    {
        /// <summary>
        /// 未使用
        /// </summary>
        nocreate = 0,
        /// <summary>
        /// 运行中
        /// </summary>
        running = 1,
        /// <summary>
        /// 已暂停
        /// </summary>
        paused = 2,
        /// <summary>
        /// 已停止
        /// </summary>
        stopped = 3
    }
    /// <summary>
    /// 任务发送枚举
    /// </summary>
    public enum SendType
    {
        /// <summary>
        /// 不发送
        /// </summary>
        nosend=0,
        /// <summary>
        /// 微信发送
        /// </summary>
        webchart=1,
        /// <summary>
        /// 短信发送
        /// </summary>
        message=2
    }


    /// <summary>
    /// 执行的任务
    /// </summary>
    public class TaskJob:IJob
    {
        private static readonly Common.Logging.ILog logger = Common.Logging.LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );
        private FileStream _fileStream = new FileStream(AppDomain.CurrentDomain.SetupInformation.ApplicationBase +"\\log.txt",FileMode.Append,FileAccess.Write,FileShare.Write);
        private object obj=new object();
        
        /// <summary>
        /// 实现Execute
        /// </summary>
        /// <param name="context"></param>
        public void Execute( IJobExecutionContext context )
        {
            try
            {
                //logger.Info( "SampleJob 任务开始运行" );
                JobKey key = context.JobDetail.Key;
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                Func<string,string,string> run = ( Func<string, string, string> )dataMap.Get( "Timer" );
                string strLastTime = context.PreviousFireTimeUtc.HasValue ? context.PreviousFireTimeUtc.Value.LocalDateTime.ToString( "yyyy-MM-dd HH:mm:ss" ) : "";
                string strNextTime = context.NextFireTimeUtc.HasValue ? context.NextFireTimeUtc.Value.LocalDateTime.ToString( "yyyy-MM-dd HH:mm:ss" ) : "";
                string strRtn = run( strLastTime, strNextTime );
                SendType st = (SendType)dataMap.Get( "SendType" );
                byte[] buffer;
                string str = string.Empty;
                if(st==SendType.webchart)
                {
                    str = "微信发送：" + strRtn+"；上次执行时间："+strLastTime+"；下次执行时间："+strNextTime+Environment.NewLine;
                }
                else if(st==SendType.message)
                {
                     str = "短信发送：" + strRtn + "；上次执行时间：" + strLastTime + "；下次执行时间：" + strNextTime + Environment.NewLine;
                }
                else if(st==SendType.nosend)
                {
                    str = strRtn;
                }
                buffer = Encoding.UTF8.GetBytes( str );
                lock ( obj )
                {
                    _fileStream.Write( buffer, 0, buffer.Length );
                }
                _fileStream.Flush();
                
                //logger.Info( "SampleJob任务运行结束" );
            }
            catch ( Exception ex )
            {
                logger.Error( "SampleJob 运行异常", ex );
            }
            _fileStream.Dispose();
        }
    }
}
