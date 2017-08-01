using System.IO;
using System.Text;
using System.Xml.Serialization;
using DexNetwork.Structure;

namespace DexNetwork
{
    public class Serializer
    {
        public static Network DeserializeNet(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Network));
            // To write to a file, create a StreamWriter object.  
            TextReader reader = new StreamReader(path);
            var result = (Network) xmlSerializer.Deserialize(reader);
            reader.Close();

            result.MakeTreeLike();

            return result;
        }

        public static string SerializeNet(Network network)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Network));
            // To write to a file, create a StreamWriter object.  
            TextWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, network);
            string strXml = textWriter.ToString();
            textWriter.Close();

            return strXml;
        }

        public static string SerializeNetAndDump(Network network, string fileName)
        {
            string xmlString = SerializeNet(network);
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            
            File.WriteAllBytes(fileName, Encoding.UTF8.GetBytes(xmlString));

            return xmlString;
        }

        public static T Deserialize<T>(string path) where T:class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            // To write to a file, create a StreamWriter object.  
            TextReader reader = new StreamReader(path);
            var result = (T)xmlSerializer.Deserialize(reader);
            reader.Close();

            return result;
        }

        public static string Serialize<T>(T persistingObject) where T : class
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            // To write to a file, create a StreamWriter object.  
            TextWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, persistingObject);
            string strXml = textWriter.ToString();
            textWriter.Close();

            return strXml;
        }


        public static string SerializeAndDump<T>(T persistingObject, string fileName) where T : class
        {
            string xmlString = Serialize(persistingObject);
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            File.WriteAllBytes(fileName, Encoding.UTF8.GetBytes(xmlString));

            return xmlString;
        }
    }
}
