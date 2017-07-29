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

        public static SoftwareLib DeserializeSoft(string path)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SoftwareLib));
            // To write to a file, create a StreamWriter object.  
            TextReader reader = new StreamReader(path);
            var result = (SoftwareLib)xmlSerializer.Deserialize(reader);
            reader.Close();

            return result;
        }

        public static string SerializeSoft(SoftwareLib softwareLib)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(SoftwareLib));
            // To write to a file, create a StreamWriter object.  
            TextWriter textWriter = new StringWriter();
            xmlSerializer.Serialize(textWriter, softwareLib);
            string strXml = textWriter.ToString();
            textWriter.Close();

            return strXml;
        }

    }
}
