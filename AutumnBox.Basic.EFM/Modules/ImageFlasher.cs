﻿/* =============================================================================*\
*
* Filename: ImgFlasher
* Description: 
*
* Version: 1.0
* Created: 2017/11/14 16:37:58 (UTC+8:00)
* Compiler: Visual Studio 2017
* 
* Author: zsh2401
* Company: I am free man
*
\* =============================================================================*/
using System;
using System.Collections.Generic;
using System.Text;
using AutumnBox.Basic.Executer;
using AutumnBox.Basic.Function.Args;
using AutumnBox.Basic.Function.Event;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using AutumnBox.Support.CstmDebug;
using AutumnBox.Basic.Devices;

namespace AutumnBox.Basic.Function.Modules
{
    public class ImageFlasher : FunctionModule
    {
        AndroidShell _shell;
        ImgFlasherArgs _args;
        private string _imgTmpFileName = "lateinautumn.img";
        private bool _suNotFound = false;
        private bool _success = true;
        private bool _imageNotFound = false;
        protected override void AnalyzeArgs(ModuleArgs args)
        {
            base.AnalyzeArgs(args);
            _args = (ImgFlasherArgs)args;
            _shell = new AndroidShell(_args.DeviceBasicInfo.Id);
            _shell.ProcessStarted += (s, e) => { OnProcessStarted(e); };
            _shell.OutputReceived += (s, e) => { OnOutputReceived(e); };
        }
        protected override OutputData MainMethod()
        {
            _shell.Connect();
            OutputData output_r = new OutputData
            {
                OutSender = _shell
            };
            Logger.D("connected");
            if (_shell.Switch2Su() != true) { _suNotFound = true; return output_r; }
            Logger.D("switched to su");
            string fullPath = DeviceImageHelper.Find(_shell, _args.ImgType);
            if (fullPath == null)
            {
                _imageNotFound = true;
                return output_r;
            }
            SendImageToSdcard();
            Logger.D("send finish");
            var r = _shell.SafetyInput($"mv /sdcard/{_imgTmpFileName} {fullPath}");
            if (!r.IsSuccess) { _success = false; }
            Logger.D("move finish...");
            return output_r;
        }
        protected override void AnalyzeOutput(ref ExecuteResult executeResult)
        {
            base.AnalyzeOutput(ref executeResult);
            if (_suNotFound || !_success) executeResult.Level = ResultLevel.Unsuccessful;
            Logger.D("_su not found?" + _suNotFound);
            Logger.D("_success?" + _success);
            _shell.Disconnect();
        }
        private bool SendImageToSdcard()
        {
            FunctionModuleProxy fmp =
                FunctionModuleProxy.Create<FileSender>(new FileSenderArgs(DevSimpleInfo)
                {
                    FilePath = _args.ImgPath,
                    SaveName = _imgTmpFileName
                });
            return fmp.SyncRun().Level == ResultLevel.Successful;
        }
    }
}
