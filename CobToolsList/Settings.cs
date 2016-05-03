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
            if (!File.Exists(path())) return new List<Item>();

            using (FileStream file = File.OpenRead(path()))
            {
                List<Item> items = (List<Item>)xml.Deserialize(file);
                for (int i = 0; i < items.Count; i++)
                {
                    if (items[i].directory == null || items[i].directory == "")
                        items[i].directory = Path.GetDirectoryName(items[i].path);
                }
                return items;
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
        public string directory;
        public string args;

        public Item()
        {

        }
        public Item(string path,string directory,string label,string args)
        {
            this.path = path;
            this.directory = directory;
            this.label = label;
            this.args = args;
        }
        public Item(string path):this(path,Path.GetDirectoryName(path),Path.GetFileNameWithoutExtension(path),"")
        {
            
        }
    }
}
