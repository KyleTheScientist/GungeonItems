using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace ItemAPI
{
    class Serializer
    {
        public static void Serialize(object obj, string file)
        {
            try
            {
                string json = JsonUtility.ToJson(obj);
                json.Replace("{", "{\n");
                using (StreamWriter sw = new StreamWriter(Path.Combine(ETGMod.ResourcesDirectory, file + ".txt")))
                {
                    sw.WriteLine(json);
                }
            }catch(Exception e)
            {
                ETGModConsole.Log(e.Message);
            }
        }

    }
}
