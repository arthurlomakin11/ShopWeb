using ShopWebData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace ShopWeb.Models
{
    public static class SettingsManager
    {
        static Dictionary<string, string> Dictionary = null;
        static SettingsManager()
        {
            InitTimer();
        }
        static void LoadValues()
        {            
            Dictionary = new ShopWebContext().Settings.ToDictionary(s => s.Key, s => s.Value);
        }
        public static string GetValue(string Key)
        {
            if(Dictionary == null)
            {
                LoadValues();
            }

            bool ValueFound = Dictionary.TryGetValue(Key, out string Value);
            if (ValueFound)
            {
                return Value;
            }
            else
            {
                return null;
                // throw new Exception("Can't find key: " + Key);
            }
        }

        public static bool GetValueBool(string Key)
        {
            int Result = int.Parse(GetValue(Key));
            return Result == 1; 
        }

        public static int GetValueInt(string Key)
        {
            return int.Parse(GetValue(Key));
        }


        static void InitTimer()
        {
            Timer timer = new();
            timer.Elapsed += Timer_Tick;
            timer.Interval = 5000; // in miliseconds
            timer.Start();
        }

        static void Timer_Tick(object sender, EventArgs e)
        {
            LoadValues();
        }
    }
}
