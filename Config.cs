using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;

namespace Vanilla.Config
{
    internal class Config
    {
        public static string ConfigFile = "Config.env";
        static bool _hasLoaded = false;
        public static void SaveKey(string key, object value, bool savenow = true)
        {

            config[Hash(key.ToLower())] = Base64Encode(value.ToString());

            if (savenow)
                SaveConfig();
        }

        public static string GetKey(string key)
        {
            if (config.TryGetValue(Hash(key.ToLower()), out var value))
                return Base64Decode(value);

            return null;
        }

        public static bool GetBool(string key)
        {
            bool.TryParse(GetKey(key), out bool value);
            return value;
        }

        public static int GetInt(string key)
        {
            int.TryParse(GetKey(key), out int value);
            return value;
        }
        public static double GetDouble(string key)
        {
            double.TryParse(GetKey(key), out double value);
            return value;
        }

        public static void SaveConfig()
        {
            using (StreamWriter stream = new StreamWriter(ConfigFile))
            {
                foreach (KeyValuePair<string, string> configuration in config)
                {
                    stream.WriteLine(configuration.Key + "|" + configuration.Value);
                }
            }
        }

        public static void LoadConfig()
        {
            if (_hasLoaded)
                return;
            else
                _hasLoaded = true;

            if (!File.Exists(ConfigFile))
                return;

            string[] ConVigValues = File.ReadAllText(ConfigFile).Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string CV in ConVigValues)
            {
                string[] Cin = CV.Split('|');
                config.Add(Cin[0], Cin[1]);
            }
        }
        
        static string Hash(string input)
        {

            // Simple SHA1 Alg for testing use some outher method here for security reasons
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            using (SHA1 sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(inputBytes);
                string hashString = BitConverter.ToString(hashBytes).Replace("-", "");
                return hashString.ToLower();
            }
        }

        static string Base64Encode(string input)
        {
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytesToEncode);
        }

        static string Base64Decode(string input)
        {
            byte[] bytes = Convert.FromBase64String(input);
            return Encoding.UTF8.GetString(bytes);
        }

        static Dictionary<string, string> config = new Dictionary<string, string>();
    }
}
