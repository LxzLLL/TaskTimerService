using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

using XLPluginManager;
using XLPluginInterface;
using XLPluginSummary;
namespace Demo
{
    public class Program
    {
        static XElement xele;
        static PluginManager pm;
        static TimerManager tm;
        static void Main( string[ ] args )
        {
            xele = XElement.Load( AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\ServiceConfig.xml" );
            pm = PluginManager.GetPluginManagerInstance( xele.Element( "TimingService" ).Element( "TSExtensionsPath" ).Value );
            tm = TimerManager.GetTimerManagerInstance();
            TimerPluginBind( GetTimingServiceConfigs(), pm.TimerPlugins );
            //StatisticsService ss = new StatisticsService();
            //ss.Run();
            //Console.Read();
        }
        /// <summary>
        /// 获取所有定时配置的项
        /// </summary>
        /// <returns></returns>
        public static List<TimingServiceConfig> GetTimingServiceConfigs()
        {
            XElement ele = xele.Element( "TimingService" ).Element( "TSExtensionsItems" );
            IEnumerable<XElement> xeles = ele.Elements();
            List<TimingServiceConfig> tscList = new List<TimingServiceConfig>();
            XmlSerializer xs = new XmlSerializer(typeof(TimingServiceConfig));

            foreach ( XElement xe in xeles )
            {
                TimingServiceConfig tsc = ( TimingServiceConfig )xs.Deserialize( xe.CreateReader() );
                tscList.Add( tsc );
            }
            return tscList;
        }

        /// <summary>
        ///  将xml配置项应用于定时器并与dll中要执行定时的方法绑定
        /// </summary>
        /// <param name="tscList"></param>
        /// <param name="ipts"></param>
        public static void TimerPluginBind( List<TimingServiceConfig> tscList, IEnumerable<IPluginTimer> ipts )
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
