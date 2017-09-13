using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoImg.Core.Common
{
    public abstract class JsonConfigItem
    {
        public abstract string CfgFile { get; }


        public string CfgPath
        {
            get
            {
                return Path.Combine(JsonConfig.BaseDir, this.CfgFile);
            }
        }

        public virtual JsonConfigItem Load()
        {
            if (File.Exists(this.CfgPath))
            {
                try
                {
                    var json = File.ReadAllText(this.CfgPath);
                    return (JsonConfigItem)JsonConvert.DeserializeObject(json, this.GetType());
                }
                catch
                {

                }
            }

            return null;
        }
    }

    public class JsonConfig
    {

        public static string BaseDir
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Cfgs");
            }
        }

        private static Dictionary<string, JsonConfigItem> Map
        {
            get;
        } = new Dictionary<string, JsonConfigItem>();

        static JsonConfig()
        {
            Watch();
        }

        public static void Regist<T>() where T : JsonConfigItem, new()
        {
            var cfg = new T();

            var key = Path.GetFileNameWithoutExtension(cfg.CfgFile.ToLower());

            if (!Map.ContainsKey(key))
            {
                Map.Add(key, cfg);
                Reload(key);
            }
        }

        private static void Watch()
        {
            if (!Directory.Exists(BaseDir))
                Directory.CreateDirectory(BaseDir);

            var fw = new FileSystemWatcher(BaseDir, "*.json");
            fw.Changed += Fw_Changed;
            fw.Created += Fw_Created;
            fw.Deleted += Fw_Deleted;
            fw.EnableRaisingEvents = true;
        }

        private static void Fw_Deleted(object sender, FileSystemEventArgs e)
        {
            var key = GetKey(e.Name);
            if (Map.ContainsKey(key))
            {
                Map.Remove(key);
            }
        }

        private static void Fw_Created(object sender, FileSystemEventArgs e)
        {
            Reload(e.Name);
        }

        private static void Fw_Changed(object sender, FileSystemEventArgs e)
        {
            Reload(e.Name);
        }

        private static string GetKey(string name)
        {
            return Path.GetFileNameWithoutExtension(name).ToLower();
        }

        private static void Reload(string name)
        {
            var key = GetKey(name);
            if (Map.ContainsKey(key))
            {
                var o = Map[key];
                var no = o.Load();
                if (no != null)
                {
                    Map[key] = no;
                }
            }
        }

        public static T Get<T>() where T : JsonConfigItem
        {
            var type = typeof(T);
            var a = Map.Values.FirstOrDefault(t => type.IsInstanceOfType(t));
            return (T)a;
        }
    }
}
