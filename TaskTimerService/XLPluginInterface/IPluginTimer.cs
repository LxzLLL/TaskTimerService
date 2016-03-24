using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLPluginInterface
{
    /// <summary>
    /// 作者：Arvin
    /// 日期：2015/11/12 13:32:04
    /// 描述：处理定时任务的接口
    /// </summary>
    public interface IPluginTimer:IPluginBase
    {
        /// <summary>
        /// 运行组件
        /// </summary>
        /// <returns></returns>
        string Run();
    }
}
