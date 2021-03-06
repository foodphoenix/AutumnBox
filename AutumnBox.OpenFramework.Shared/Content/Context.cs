﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/3/6 16:48:15 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.Logging;
using AutumnBox.OpenFramework.Fast;
using AutumnBox.OpenFramework.Management;
using AutumnBox.OpenFramework.Open;
using AutumnBox.OpenFramework.Open.Impl;
using AutumnBox.OpenFramework.Open.Management;
using AutumnBox.OpenFramework.Running;
using System;
using System.Linq;

namespace AutumnBox.OpenFramework.Content
{
    /// <summary>
    /// AutumnBox开放框架上下文
    /// </summary>
    [ContextPermission(CtxPer.Normal)]
    public abstract class Context : object
    {
        /// <summary>
        /// 权限
        /// </summary>
        internal CtxPer Permission
        {
            get
            {
                if (permission == CtxPer.None)
                {
                    var attr = Attribute
                    .GetCustomAttribute(GetType(),
                    typeof(ContextPermissionAttribute), true);
                    permission = (attr as ContextPermissionAttribute)?.Value ?? CtxPer.Normal;
                }
                return permission;
            }
        }
        private CtxPer permission = CtxPer.None;

        /// <summary>
        /// 日志标签
        /// </summary>
        public virtual string LoggingTag
        {
            get
            {
                return GetType().Name;
            }
        }

        /// <summary>
        /// Ux api
        /// </summary>
        public IUx Ux => _lazyUX.Value;
        private Lazy<IUx> _lazyUX;

        /// <summary>
        /// 日志API
        /// </summary>
        public ILogger Logger => _lazyLogger.Value;
        private Lazy<ILogger> _lazyLogger;

        /// <summary>
        /// 秋之盒整体程序相关API
        /// </summary>
        public IAppManager App => _lazyApp.Value;
        private Lazy<IAppManager> _lazyApp;

        /// <summary>
        /// 临时文件管理器
        /// </summary>
        public ITemporaryFloder Tmp => _lazyTmp.Value;
        private Lazy<ITemporaryFloder> _lazyTmp;

        /// <summary>
        /// 兼容性相关API
        /// </summary>
        public ICompApi Comp => _comp;
        private readonly static ICompApi _comp = new CompImpl();


        /// <summary>
        /// 嵌入资源提取器
        /// </summary>
        public IEmbeddedFileManager EmbeddedManager => _lazyEmb.Value;

        /// <summary>
        /// 嵌入资源提取器
        /// </summary>
        public IEmbeddedFileManager EmbFileManager => _lazyEmb.Value;
        private Lazy<IEmbeddedFileManager> _lazyEmb;

        internal IBaseApi BaseApi
        {
            get
            {
                return OpenFx.BaseApi;
            }
        }
        /// <summary>
        /// 构建
        /// </summary>
        public Context()
        {
            InitFactory();
        }
        /// <summary>
        /// 获取一个新的拓展模块线程
        /// </summary>
        /// <param name="extensionType"></param>
        /// <returns></returns>
        public IExtensionThread NewExtensionThread(Type extensionType)
        {
            var wrappers = from wrapper in OpenFx.LibsManager.Wrappers()
                           where extensionType == wrapper.Info.ExtType
                           select wrapper;
            if (wrappers.Count() == 0)
            {
                throw new Exception("Extension not found");
            }
            return wrappers.First().GetThread();
        }
        /// <summary>
        /// 获取一个新的拓展模块线程
        /// </summary>
        /// <returns></returns>
        public IExtensionThread NewExtensionThread(string className)
        {
            var wrappers = from wrapper in OpenFx.LibsManager.Wrappers()
                           where className == wrapper.Info.ExtType.Name
                           select wrapper;
            if (!wrappers.Any())
            {
                throw new Exception("Extension not found");
            }
            return wrappers.First().GetThread();
        }
        /// <summary>
        /// 启动一个另一个模块
        /// </summary>
        /// <param name="t"></param>
        /// <param name="callback"></param>
        public void StartExtension(Type t, Action<int> callback = null)
        {
            var thread = NewExtensionThread(t);
            thread.Finished += (s, e) =>
            {
                callback?.Invoke(e.Thread.ExitCode);
            };
            thread.Start();
        }
        /// <summary>
        /// 在UI线程运行代码
        /// </summary>
        /// <param name="act"></param>
        public void RunOnUIThread(Action act)
        {
            App.RunOnUIThread(act);
        }
        /// <summary>
        /// 初始化各种懒加载工厂方法
        /// </summary>
        private void InitFactory()
        {
            _lazyApp = new Lazy<IAppManager>(() =>
            {
                return OpenApiFactory.Get<IAppManager>(this);
            });
            _lazyUX = new Lazy<IUx>(() =>
            {
                return OpenApiFactory.Get<IUx>(this);
            });
            _lazyLogger = new Lazy<ILogger>(() =>
            {
                return LoggerFactory.Auto(LoggingTag);
            });
            _lazyTmp = new Lazy<ITemporaryFloder>(() =>
            {
                return OpenApiFactory.Get<ITemporaryFloder>(this);
            });
            _lazyEmb = new Lazy<IEmbeddedFileManager>(() =>
            {
                return OpenApiFactory.Get<IEmbeddedFileManager>(this);
            });
        }
    }
}