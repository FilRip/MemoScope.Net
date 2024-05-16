using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

using NLog;

namespace WinFwk.UITools.Settings
{
    public static class UISettingsMgr<T> where T : UISettings, new()
    {
#pragma warning disable S2743 // Static fields should not be used in generic types
        private static XmlSerializer xml;
#pragma warning restore S2743 // Static fields should not be used in generic types

        public static void Init(string applicationName)
        {
            List<Type> types = WinFwkHelper.GetDerivedTypes(typeof(UISettings));
            xml = new XmlSerializer(typeof(T), types.ToArray());
            UISettings.InitSettings(Load(applicationName));
            InitLogs(applicationName);
        }

        public static void Init()
        {
            Init(Application.ProductName);
        }

        public static T Load(string applicationName)
        {
            string configPath = GetConfigPath(applicationName);
            if (!File.Exists(configPath))
            {
                return new T();
            }
            FileInfo fi = new(configPath);
            if (fi.Length == 0)
            {
                return new T();
            }
            using StreamReader reader = new(configPath);
            object configObj = xml.Deserialize(reader);
            T config = configObj as T;
            return config;
        }

        public static T Load()
        {
            return Load(Application.ProductName);
        }

        public static void Save(string applicationName, T uiSettings)
        {
            string configPath = GetConfigPath(applicationName);
            string dir = Path.GetDirectoryName(configPath);
            if (dir != null && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using StreamWriter reader = new(configPath);
            xml.Serialize(reader, uiSettings);
        }

        public static void Save(T uiSettings)
        {
            Save(Application.ProductName, uiSettings);
        }

        private static string GetConfigPath(string applicationName)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string configPath = Path.Combine(appDataPath, applicationName);
            configPath = Path.Combine(configPath, applicationName);
            configPath = Path.ChangeExtension(configPath, "config");
            return configPath;
        }

        private static void InitLogs(string applicationName)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appPath = Path.Combine(appDataPath, applicationName);
            string logPath = Path.Combine(appPath, "log");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            LogManager.EnableLogging();
            LogManager.GetLogger(typeof(UISettingsMgr<>).Name).Info("Init Logs");
        }
    }
}