using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace LWFramework.Asset
{
    public static class DownloadTxtFile
    {
        public const string txtFile = "download.txt";
        private const char splitKey = '=';

        public static Dictionary<string, string> data = new Dictionary<string, string>();

        public static void Load()
        {
            Clear();
            var path = LWUtility.updatePath + txtFile;
            if (File.Exists(path))
            { 
                using (var s = new StreamReader(path))
                {
                    string line;
                    while ((line = s.ReadLine()) != null)
                    {
                        if (line == string.Empty)
                            continue;
                        var fields = line.Split(splitKey);
                        if (fields.Length > 1)
                            data.Add(fields[0], fields[1]);
                    }
                }
            }
        }

        public static void Clear()
        {
            data.Clear();
        }

        public static void Set(string key, string version)
        {
            data[key] = version;
        }

        public static string Get(string key)
        {
            string version;
            data.TryGetValue(key, out version);
            return version;
        }

        public static void Save()
        {
            var path = LWUtility.updatePath + txtFile;
            if (File.Exists(path))
                File.Delete(path);

            using (var s = new StreamWriter(path))
            {
                foreach (var item in data)
                    s.WriteLine(item.Key + splitKey + item.Value);
                s.Flush();
                s.Close();
            }  
        }
    }
}