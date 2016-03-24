using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLPluginInterface
{
    /// <summary>
    /// 作者：Arvin
    /// 日期：2015/11/12 13:23:05
    /// 描述：组件接口基类，主要描述组件基本信息
    /// </summary>
    public interface IPluginBase
    {
        /// <summary>
        /// 编写者
        /// </summary>
        string Author { get; }
        /// <summary>
        /// 版本
        /// </summary>
        string Version { get; }
        /// <summary>
        /// 插件文件名称（不包括.dll）
        /// </summary>
        string PluginName { get;}
        /// <summary>
        /// 插件中文名
        /// </summary>
        string PluginCNName { get; }
        /// <summary>
        /// 插件描述
        /// </summary>
        string PluginDes { get;}
        /// <summary>
        /// 公司名称
        /// </summary>
        string CompanyName { get; }
    }
}
