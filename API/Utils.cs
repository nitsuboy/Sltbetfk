using  System.Xml;

namespace API.Utils
{
    public class Utils {
        public static List<string> getSettings(string path)
        {
            List<string> _ret = new List<string>();
            if (File.Exists(path))
            {
                StreamReader reader = new StreamReader
                (
                    new FileStream(
                        path,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read)
                );
                XmlDocument doc = new XmlDocument();
                string xmlIn = reader.ReadToEnd();
                reader.Close();
                doc.LoadXml(xmlIn);
                foreach (XmlNode child in doc.ChildNodes)
                    if (child.Name.Equals("Settings"))
                        foreach (XmlNode node in child.ChildNodes)
                            if (node.Name.Equals("add")){
                                _ret.Add(node.Attributes["value"].Value);
                            }
            }else{
                Console.WriteLine("no");
            }
            return (_ret);
        }
    }
}
