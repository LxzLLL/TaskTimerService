using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

using XLPluginInterface;

namespace XLPluginManager
{
    /// <summary>
    /// 作者：Arvin
    /// 日期：2015/11/12 16:35:24
    /// 描述：组件管理
    /// 缺陷：目前暂未处理卸载的情况
    /// </summary>
    public class PluginManager:IPartImportsSatisfiedNotification
    { 
        #region 属性
        /// <summary>
        /// 程序集的目录路径
        /// </summary>
        public string DirectoryPath { get; private set; }

        /// <summary>
        /// 组件集合信息
        /// </summary>
        [ImportMany(typeof(IPluginTimer),AllowRecomposition=true)]
        public IEnumerable<IPluginTimer> TimerPlugins { get; private set; }

        /// <summary>
        /// 在目录变更时，新增的插件
        /// 在catalog.refresh后，检测到的新增的dll
        /// </summary>
        public IEnumerable<IPluginTimer> NewTimerPlugins { get; private set; }
        #endregion

        #region 字段
        /// <summary>
        /// 承载组件的目录
        /// </summary>
        private DirectoryCatalog _catalog;
        /// <summary>
        /// 承载组件的容器
        /// </summary>
        private CompositionContainer _container;
        private AggregateCatalog _aggCatalog = new AggregateCatalog();
        //
        private IEnumerable<IPluginTimer> OldTimerPlugins = null;
        /// <summary>
        /// 静态XLPluginManagerTimer
        /// </summary>
        private static PluginManager _pluginManager;

        #endregion

        private PluginManager( string strDirectoryPath )
        {
            this.DirectoryPath = strDirectoryPath;
            ComponentInit();
        }

        /// <summary>
        /// 获取单实例
        /// 只能对路径复制一次，第二次调用时路径值不会改变
        /// </summary>
        /// <param name="strDirectoryPath"></param>
        /// <returns></returns>
        public static PluginManager GetPluginManagerInstance( string strDirectoryPath )
        {
            if ( _pluginManager == null )
            {
                _pluginManager = new PluginManager( strDirectoryPath );
            }
            return _pluginManager;
        }


        /// <summary>
        /// 初始化 目录和容器
        /// </summary>
        private void ComponentInit()
        {
            if ( string.IsNullOrEmpty( this.DirectoryPath ) )
            {
                return;
            }
           this._catalog = new DirectoryCatalog( this.DirectoryPath );
           this._aggCatalog.Catalogs.Add( this._catalog );
           this._container = new CompositionContainer( this._aggCatalog );
            try
            {
                this._container.ComposeParts( this );
                OldTimerPlugins = NewTimerPlugins;
            }
            catch ( CompositionException compositionException )
            {

            }
        }
        /// <summary>
        /// 刷新目录，用于dll目录中新加或删除dll后的刷新操作
        /// </summary>
        public void CatalogRefresh()
        {
            this.OldTimerPlugins = this.TimerPlugins;
            this.NewTimerPlugins = null;
            this._catalog.Refresh();
        }

        #region IPartImportsSatisfiedNotification 接口实现
        /// <summary>
        /// 目录变更时触发的事件
        /// 设置新增的部件
        /// </summary>
        public void OnImportsSatisfied()
        {
            if ( this.OldTimerPlugins == null )
            {
                this.NewTimerPlugins = this.TimerPlugins;
            }
            else if ( this.TimerPlugins == null )
            {
                
            }
            else
            {
                var rtnlist = this.TimerPlugins.Where( x =>
                {
                    bool  bln = true;
                    foreach ( IPluginTimer i in OldTimerPlugins )
                    {
                        if ( x.PluginName == i.PluginName )
                        {
                            bln = false;
                            break;
                        }
                    }
                    return bln;
                } ).Select( x => x );
                this.NewTimerPlugins = rtnlist.ToList<IPluginTimer>();
            }
        }
        #endregion

    }
}
