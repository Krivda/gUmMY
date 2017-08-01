namespace DexNetwork.Structure
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Serialization;

    namespace DexNetwork.Structure
    {

        [XmlRoot(ElementName = "SoftwareEffectsLib")]
        public class SoftwareEffectsLib
        {
            private string _fileName;

            [XmlArray("SoftwareEffects")]
            [XmlArrayItem("SoftwareEffect", Type = typeof(SoftwareEffect))]
            public List<SoftwareEffect> SoftwareEffectsList { get; set; }

            [XmlIgnore]
            public Dictionary<string, SoftwareEffect> SoftwareEffects { get; set; } = new Dictionary<string, SoftwareEffect>();

            public void Init(string fileName)
            {
                _fileName = fileName;

                StringBuilder errors = new StringBuilder();
                foreach (var softwareEffect in SoftwareEffectsList)
                {
                    try
                    {
                        AddNewEffect(softwareEffect);
                    }
                    catch (ArgumentException e)
                    {
                        errors.AppendLine(e.Message);
                    }
                }

                if (!string.IsNullOrEmpty(errors.ToString()))
                {
                    throw new Exception($"Can't load software effects lib from {_fileName}: \n{errors}");
                }
            }

            public void AddNewEffect(SoftwareEffect effect)
            {

                if (SoftwareEffects.ContainsKey(effect.Name))
                    throw new ArgumentException($"Effect  '{effect.Name}' is duplicated in library.");

                SoftwareEffects.Add(effect.Name, effect);
            }

            public void DumpToFile()
            {
                Serializer.SerializeAndDump(this, _fileName);
            }
        }

        public class SoftwareEffect
        {

            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlAttribute(AttributeName = "description")]
            public string Effect { get; set; }
        }
    }
}