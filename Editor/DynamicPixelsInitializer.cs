﻿using System;
using DynamicPixels.GameService.Models;
using UnityEngine;
using Debug = UnityEngine.Debug;
using LogType = DynamicPixels.GameService.Models.LogType;
using DynamicPixels.GameService.Utils.Logger;
using SystemInfo = DynamicPixels.GameService.Models.SystemInfo;
using DynamicPixels.GameService;

namespace DynamicPixelsInitializer
{
    public class DynamicPixelsInitializer : MonoBehaviour
    {
        public string clientId;
        public string clientSecret;
        public bool developmentMode;
        public bool debugMode = false;
        public bool verboseMode = false;
        
        private static bool _isInit = false;

        public void OnDisable()
        {
            Debug.Log("DynamicPixels was disabled");
            DynamicPixelsDispose();
        }
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void OnEnable()
        {
            // dont initialize SDK multiple time 
            if (_isInit || ServiceHub.IsAuthenticated()) return;
            Debug.Log("DynamicPixels Initializing");

            // getting system info
            var systemInfo = new SystemInfo()
            {
                DeviceUniqueId = UnityEngine.SystemInfo.deviceUniqueIdentifier,
                DeviceModel = UnityEngine.SystemInfo.deviceModel,
                DeviceName = UnityEngine.SystemInfo.deviceName,
                DeviceType = UnityEngine.SystemInfo.deviceType.ToString(),
                OperatingSystem = UnityEngine.SystemInfo.operatingSystem,
                NetworkType = UnityEngine.Application.internetReachability.ToString(),
                ProcessorCount = UnityEngine.SystemInfo.processorCount,
                ProcessorFrequency = UnityEngine.SystemInfo.processorFrequency,
                ProcessorType = UnityEngine.SystemInfo.processorType,
                GraphicsDeviceName = UnityEngine.SystemInfo.graphicsDeviceName,
                GraphicsDeviceVendor = UnityEngine.SystemInfo.graphicsDeviceVendor,
                GraphicsMemorySize = UnityEngine.SystemInfo.graphicsMemorySize
            };

            // configure Sdk instance
            ServiceHub.Configure(clientId, clientSecret, systemInfo, debugMode, developmentMode, verboseMode);

            LogHelper.OnDebugReceived += LoggerOnDebugReceived;

            _isInit = true;
            Debug.Log("DynamicPixels Initialized");
        }

        private void OnDestroy()
        {
            DynamicPixelsDispose();
        }

        private void OnApplicationQuit()
        {
            DynamicPixelsDispose();
        }

        private void DynamicPixelsDispose()
        {
            if(!_isInit) return;
            
            _isInit = false;

            Debug.Log("DynamicPixels Disposed");

            ServiceHub.Dispose();
        }

        private static void LoggerOnDebugReceived(object sender, DebugArgs debug)
        {
            switch (debug.LogTypeType)
            {
                case LogType.Normal:
                    Debug.Log(debug.Data);
                    break;
                case LogType.Error:
                    Debug.LogError(debug.Data);
                    break;
                case LogType.Exception:
                    Debug.LogException(debug.Exception);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // if (!EnableSaveDebugLogs) return;

            // if (Directory.Exists(_appPath + DebugPath))
            //     File.AppendAllText(_appPath + DebugPath + _logFile,debug.Data + "\r\n");
        }
    }
}
