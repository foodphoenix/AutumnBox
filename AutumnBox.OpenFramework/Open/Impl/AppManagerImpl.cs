﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/8/2 1:53:51 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.OpenFramework.Content;
using AutumnBox.OpenFramework.Open.Impl.AutumnBoxApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutumnBox.OpenFramework.Open.Impl
{
    internal partial class AppManagerImpl : IAppManager
    {
        private readonly IAutumnBoxGuiApi sourceApi;
        private readonly Context ctx;
        public AppManagerImpl(Context ctx)
        {
            this.ctx = ctx;
            this.sourceApi = AutumnBoxGuiApiProvider.Get();
        }

        public bool IsRunAsAdmin
        {
            get
            {
                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public string CurrentLanguageCode => sourceApi.GetCurrentLanguageCode();

        public void CloseLoadingWindow()
        {
            sourceApi.CloseLoadingWindow();
        }

        public Window CreateDebuggingWindow()
        {
            return sourceApi.CreateDebugWindow();
        }

        public Window GetMainWindow()
        {
            return sourceApi.GetMainWindow();
        }

        public object GetPublicResouce(string key)
        {
            return sourceApi.GetResouce(key);
        }

        public TReturn GetPublicResouce<TReturn>(string key) where TReturn : class
        {
            return (TReturn)sourceApi.GetResouce(key);
        }

        public void RefreshExtensionList()
        {
            sourceApi.RefreshExtensionList();
        }

        public void RestartApp()
        {
            sourceApi.Restart();
        }

        public void RestartAppAsAdmin()
        {
            sourceApi.RestartAsAdmin();
        }

        public void RunOnUIThread(Action act)
        {
            sourceApi.RunOnUIThread(act);
        }

        public ChoiceBoxResult ShowChoiceBox(string title, string msg, string btnLeft = null, string btnRight = null)
        {
            return sourceApi.ShowChoiceBox(title,msg,btnLeft,btnRight);
        }

        public void ShowLoadingWindow()
        {
            sourceApi.ShowLoadingWindow();
        }

        public void ShowMessageBox(string title, string msg)
        {
            sourceApi.ShowMessageBox(title, msg);
        }

        public void ShutdownApp()
        {
            sourceApi.Shutdown();
        }
    }
}
