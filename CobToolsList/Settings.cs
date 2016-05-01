using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CobToolsList
{
    static class Settings
    {
        private static XmlSerializer xml = new XmlSerializer(typeof(List<Item>));
        public static List<Item> Load()
        {
            if (!File.Exists(path())) throw new FileNotFoundException();
            
            using(FileStream file =  File.OpenRead(path()))
            {
                return (List<Item>)xml.Deserialize(file);
            }            
        }
       
        public static bool Save(List<Item> Files)
        {
            try
            {
                using (FileStream file = File.OpenWrite(path()))
                {
                    file.SetLength(0);
                    xml.Serialize(file, Files);
                }
                return true;
            }
            catch { return false; }
        }
        private static string path()
        {
            return Path.GetDirectoryName(typeof(Settings).Assembly.Location) + "\\settings.xml";
        }
    }
    public class Item
    {
        public string label;
        public string path;
    }
}
