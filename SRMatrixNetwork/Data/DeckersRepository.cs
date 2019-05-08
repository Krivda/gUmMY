using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SRMatrixNetwork.Data
{
    class DeckersRepository 
    {
        public static Dictionary<string, KnownDecker> LoadDeckers()
        {

            var result = new Dictionary<string, KnownDecker>();

            string path = Path.GetFullPath("data/deckers.xml");
            if (File.Exists(path))
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(path);
                XmlNode root = doc.DocumentElement;

                XmlNodeList nodes = root?.SelectNodes("./Decker");

                if (nodes != null)
                {
                    foreach (XmlNode node in nodes)
                    {
                        string name = "";
                        string pwd = "";
                        string jid = "";

                        if (node.Attributes != null)
                        {
                            XmlAttribute nameAttribute = node.Attributes["name"];
                            XmlAttribute jidAttribute = node.Attributes["jid"];
                            XmlAttribute pwdAttribute = node.Attributes["pwd"];
                            if (nameAttribute != null)
                            {
                                name = nameAttribute.Value;
                            }

                            if (pwdAttribute != null)
                            {
                                pwd = pwdAttribute.Value;
                            }

                            if (jidAttribute != null)
                            {
                                jid = jidAttribute.Value;
                            }
                        }

                        result.Add(name, new KnownDecker(name, pwd, jid));
                    }
                }
            }
            return result;
        }
    }
}