using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosina.Components
{
    public static class CoLogManager
    {
        public static List<string> logs;
        
        static CoLogManager()
        {
            if (logs != null)
            {
                Debug.LogError("Hey this logs is not null");
            }
            logs = new List<string>(0);
        }
    
        public static void Log(string s)
        {
            s = $"{Time.frameCount.ToString().WrapColor("red")}\t{s}\n{Environment.StackTrace}";
            logs.Add(s);
        }
    
        public static void Clear()
        {
            logs.Clear();
        }
    }

}
