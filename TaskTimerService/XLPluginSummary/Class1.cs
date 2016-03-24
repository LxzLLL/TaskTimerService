using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Reflection;

using System.IO;
using System.Net;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Web.Services.Description;
using System.Xml.Serialization;
using System.Web.Services.Discovery;
using System.Xml.Schema;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using Microsoft.CSharp;

using XLPluginInterface;
namespace XLPluginSummary
{
    /// <summary>
    /// 作者：Arvin
    /// 日期：2015/11/11 17:25:39
    /// 描述：
    /// </summary>
    /// <summary>
    /// 导入
    /// </summary>
    [Export( typeof( IPluginTimer ) ), PartCreationPolicy( CreationPolicy.Shared )]
    public class StatisticsService : IPluginTimer
    {
        public string Run()
        {
            //BuildingSiteSummaryServiceSoapClient bsssc = new BuildingSiteSummaryServiceSoapClient();
            //bsssc.BSSummarySendInfo();
            //string strUrl = 
            object[] args = new object[ ] { };
            string strError = string.Empty;
            string strUrl  = GetUrlByXml( ref strError );
            if ( !string.IsNullOrEmpty(strUrl) && string.IsNullOrEmpty(strError))
            {
                try{
                    InvokeWebService( strUrl, "BuildingSiteSummaryService", "BSSummarySendInfo", args );
                }
                catch(Exception ex)
                {
                    strError = ex.Message;
                }
            }
            string strLog =  "XLPluginBSSummary：" + DateTime.Now.ToString( "yyyy-MM-dd HH:mm:ss" ) + Environment.NewLine + strError;
            if(!string.IsNullOrEmpty(strError))
            {
                strLog +=Environment.NewLine;
            }
            return strLog;
        }

        public string Author
        {
            get
            {
                return "Arvin";
            }
        }

        public string Version
        {
            get
            {
                return "1.0";
            }
        }

        public string PluginName
        {
            get
            {
                return "XLPluginSummary";
            }
        }

        public string PluginCNName
        {
            get
            {
                return "测试插件";
            }
        }

        public string PluginDes
        {
            get
            {
                return "测试插件，不拉不拉";
            }
        }

        public string CompanyName
        {
            get
            {
                return "鋆达";
            }
        }

        /// <summary>
        /// 获取xml中的url配置信息
        /// </summary>
        /// <returns></returns>
        private string GetUrlByXml( ref string strError)
        {
            string strRtn = string.Empty;
            try
            {
                XElement xele = XElement.Load( AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Extensions\\SummaryConfig.xml" );
                strRtn = xele.Element( "UrlConnection" ).Value;
            }
            catch( Exception ex )
            {
                strError = ex.Message;
            }

            return strRtn;
            
        }


        /// <summary> 
        /// 动态调用WebService 
        /// </summary> 
        /// <param name="url">WebService地址</param> 
        /// <param name="classname">类名</param> 
        /// <param name="methodname">方法名(模块名)</param> 
        /// <param name="args">参数列表</param> 
        /// <returns>object</returns> 
        private static object InvokeWebService( string url, string classname, string methodname, object[ ] args )
        {
            string @namespace = "ServiceBase.WebService.DynamicWebLoad";
            if ( classname == null || classname == "" )
            {
                classname = GetClassName( url );
            }
            //获取服务描述语言(WSDL) 
            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead( url + "?WSDL" );
            ServiceDescription sd = ServiceDescription.Read( stream );
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription( sd, "", "" );
            CodeNamespace cn = new CodeNamespace( @namespace );
            //生成客户端代理类代码 
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add( cn );
            sdi.Import( cn, ccu );

            CSharpCodeProvider csc = new CSharpCodeProvider();
            ICodeCompiler icc = csc.CreateCompiler();
            //设定编译器的参数 
            CompilerParameters cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;
            cplist.GenerateInMemory = true;
            cplist.ReferencedAssemblies.Add( "System.dll" );
            cplist.ReferencedAssemblies.Add( "System.XML.dll" );
            cplist.ReferencedAssemblies.Add( "System.Web.Services.dll" );
            cplist.ReferencedAssemblies.Add( "System.Data.dll" );
            //编译代理类 
            CompilerResults cr = icc.CompileAssemblyFromDom( cplist, ccu );
            if ( true == cr.Errors.HasErrors )
            {
                System.Text.StringBuilder sb = new StringBuilder();
                foreach ( CompilerError ce in cr.Errors )
                {
                    sb.Append( ce.ToString() );
                    sb.Append( System.Environment.NewLine );
                }
                throw new Exception( sb.ToString() );
            }
            //生成代理实例,并调用方法 
            System.Reflection.Assembly assembly = cr.CompiledAssembly;
            Type t = assembly.GetType( @namespace + "." + classname, true, true );
            object obj = Activator.CreateInstance( t );
            System.Reflection.MethodInfo mi = t.GetMethod( methodname );
            return mi.Invoke( obj, args );
        }

        private static string GetClassName( string url )
        {
            string[] parts = url.Split( '/' );
            string[] pps = parts[ parts.Length - 1 ].Split( '.' );
            return pps[ 0 ];
        }
    }

}
