using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;

using XLPluginManager;
using XLPluginInterface;

namespace TimingService
{
    /// <summary>
    /// add arvin
    /// 定时服务
    /// </summary>
    partial class StatisticsService : ServiceBase,IPluginTimer
    {
        private PluginManager pm;
        private TimerManager tm;
        /// <summary>
        /// XML配置文件
        /// </summary>
        public XElement XElementXMLConfig
        {
            get
            {
                return XElement.Load( AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\ServiceConfig.xml" );
            }
        }

        public StatisticsService()
        {
            InitializeComponent();
            pm = PluginManager.GetPluginManagerInstance( this.XElementXMLConfig.Element( "TimingService" ).Element( "TSExtensionsPath" ).Value );
            tm = TimerManager.GetTimerManagerInstance();
        }

        protected override void OnStart( string[ ] args )
        {
            // 将xml配置项应用于定时器并与dll中要执行定时的方法绑定
            TimerPluginBind( GetTimingServiceConfigs(), pm.TimerPlugins );
            // 创建自身的定时任务，每10s定时catalog的刷新
            TimerTask tt = tm.CreateTask( "0/10 * * * * ?", this, "0" );
            tm.TaskStart( tt );
        }

        protected override void OnStop()
        {
            // TODO:  在此处添加代码以执行停止服务所需的关闭操作。
            tm.DeleteTasks();
        }

        /// <summary>
        /// 获取所有定时配置的项
        /// </summary>
        /// <returns></returns>
        public List<TimingServiceConfig> GetTimingServiceConfigs( string[ ] strTSExtensionsItemName = null )
        {
            XElement ele = this.XElementXMLConfig.Element( "TimingService" ).Element( "TSExtensionsItems" );
            IEnumerable<XElement> xeles = ele.Elements();
            List<TimingServiceConfig> tscList = new List<TimingServiceConfig>();
            XmlSerializer xs = new XmlSerializer( typeof( TimingServiceConfig ) );

            foreach ( XElement xe in xeles )
            {
                TimingServiceConfig tsc = ( TimingServiceConfig )xs.Deserialize( xe.CreateReader() );
                if ( strTSExtensionsItemName != null && strTSExtensionsItemName.Length > 0 )
                {
                    if ( strTSExtensionsItemName.Contains( tsc.TSExtensionsItemName ) )
                    {
                        tscList.Add( tsc );
                    }
                }
                else
                {
                    tscList.Add( tsc );
                }

            }
            return tscList;
        }

        /// <summary>
        ///  将xml配置项应用于定时器并与dll中要执行定时的方法绑定
        /// </summary>
        /// <param name="tscList"></param>
        /// <param name="ipts"></param>
        public void TimerPluginBind( List<TimingServiceConfig> tscList, IEnumerable<IPluginTimer> ipts )
        {
            if ( tscList==null||ipts==null||tscList.Count<=0||ipts.Count()<=0)
            {
                return;
            }

            foreach ( IPluginTimer ipt in ipts )
            {
                var tscs = from obj in tscList
                          where obj.TSExtensionsItemName == ipt.PluginName
                          select obj;
                if(tscs.Count()>0)
                {
                    TimingServiceConfig tsc = tscs.First();
                    TimerTask tt = tm.CreateTask( tsc.TSExtensionsItemCron, ipt, tsc.TSExtensionsItemSendType );
                    tm.TaskStart( tt );
                }
            }
        }

        /// <summary>
        /// 刷新dll目录，后添加定时任务
        /// </summary>
        public void CatalogRefresh()
        {
            pm.CatalogRefresh();
            if ( pm.NewTimerPlugins == null || pm.NewTimerPlugins.Count() <= 0 )
            {
                return;
            }
            string str=string.Empty;
            foreach ( IPluginTimer ipt in pm.NewTimerPlugins )
            {
                str += ipt.PluginName + ",";
            }

            List<TimingServiceConfig> tscList = GetTimingServiceConfigs( str.TrimEnd( ',' ).Split( ',' ) );
            TimerPluginBind( tscList, pm.NewTimerPlugins );
        }

        #region IPluginTimer接口实现
        public string Run()
        {
            CatalogRefresh();
            return "";
        }

        public string Author
        {
            get { return ""; }
        }

        public string Version
        {
            get { return ""; }
        }

        public string PluginName
        {
            get { return ""; }
        }

        public string PluginCNName
        {
            get { return ""; }
        }

        public string PluginDes
        {
            get { return ""; }
        }

        public string CompanyName
        {
            get { return ""; }
        }
        #endregion

    }

    /// <summary>
    /// 定时任务配置
    /// </summary>
    [XmlRoot( "TSExtensionsItem" )]
    public class TimingServiceConfig
    {
        /// <summary>
        /// dll的名称
        /// </summary>
        [XmlElement( "TSExtensionsItemName" )]
        public string TSExtensionsItemName { get; set; }
        /// <summary>
        /// dll要定时运行的定时字符串
        /// </summary>
        [XmlElement( "TSExtensionsItemCron" )]
        public string TSExtensionsItemCron { get; set; }
        /// <summary>
        /// dll要定时运行后发送信息的方式
        /// </summary>
        [XmlElement( "TSExtensionsItemSendType" )]
        public string TSExtensionsItemSendType { get; set; }
    }

}
