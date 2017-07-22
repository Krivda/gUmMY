﻿using System;
using System.Xml;

namespace Sharp.Xmpp.Extensions.Dataforms
{
    /// <summary>
    /// The abstract base class from which all implementations of concrete
    /// data-forms must derive.
    /// </summary>
    public abstract class DataForm
    {
        private const string xmlns = "jabber:x:data";

        /// <summary>
        /// The fields contained in the data-form.
        /// </summary>
        private FieldList fields;

        /// <summary>
        /// The underlying XML element representing the data-form.
        /// </summary>
        protected XmlElement Element
        {
            get;
            set;
        }

        /// <summary>
        /// The title of the data-form.
        /// </summary>
        public string Title
        {
            get
            {
                if (Element["title"] != null)
                    return Element["title"].InnerText;
                return null;
            }

            set
            {
                var e = Element["title"];
                if (e != null)
                {
                    if (value == null)
                        Element.RemoveChild(e);
                    else
                        e.InnerText = value;
                }
                else
                {
                    if (value != null)
                        Element.Child(Xml.Element("title").Text(value));
                }
            }
        }

        /// <summary>
        /// The natural-language instructions to be followed by the
        /// form-submitting entity.
        /// </summary>
        public string Instructions
        {
            get
            {
                if (Element["instructions"] != null)
                    return Element["instructions"].InnerText;
                return null;
            }

            set
            {
                var e = Element["instructions"];
                if (e != null)
                {
                    if (value == null)
                        Element.RemoveChild(e);
                    else
                        e.InnerText = value;
                }
                else
                {
                    if (value != null)
                        Element.Child(Xml.Element("instructions").Text(value));
                }
            }
        }

        /// <summary>
        /// The type of the data-form.
        /// </summary>
        /// <exception cref="XmlException">The 'type' attribute of the underlying
        /// XML element is invalid.</exception>
        public DataFormType Type
        {
            get
            {
                return GetDataFormType();
            }

            protected set
            {
                Element.SetAttribute("type", value.ToString().ToLower());
            }
        }

        /// <summary>
        /// A list of fields contained in the data-form.
        /// </summary>
        public FieldList Fields
        {
            get
            {
                return fields;
            }
        }

        /// <summary>
        /// Returns a textual XML representation of the data-form.
        /// </summary>
        /// <returns>A string containing the XML representation of the
        /// data-form.</returns>
        public override string ToString()
        {
            return Element.ToXmlString();
        }

        /// <summary>
        /// Returns an XML element representing the data-form.
        /// </summary>
        /// <returns>An XML element representing the data-form.</returns>
        public XmlElement ToXmlElement()
        {
            return Element;
        }

        /// <summary>
        /// Initializes a new instance of the DataForm class.
        /// </summary>
        /// <param name="title">The title of the data-form.</param>
        /// <param name="instructions">The natural-language instructions to be
        /// followed by the form-submitting entity.</param>
        /// <param name="readOnly">Set to true to create a read-only form to
        /// which no fields may be added, otherwise false.</param>
        /// <param name="fields">One or several data-fields to add to the
        /// form.</param>
        internal DataForm(string title = null, string instructions = null,
            bool readOnly = false, params DataField[] fields)
        {
            Element = Xml.Element("x", xmlns);
            Title = title;
            Instructions = instructions;
            this.fields = new FieldList(Element, readOnly);
            if (fields != null)
            {
                foreach (var f in fields)
                    if (f != null)
                        this.fields.Add(f);
            }
        }

        /// <summary>
        /// Initializes a new instance of the DataForm class from the specified
        /// XML element.
        /// </summary>
        /// <param name="element">The XML 'field' element to initialize the instance
        /// with.</param>
        /// <param name="readOnly">Set to true to create a read-only form to
        /// which no fields may be added, otherwise false.</param>
        /// <exception cref="ArgumentNullException">The element parameter is
        /// null.</exception>
        /// <exception cref="ArgumentException">The specified XML element is not a
        /// valid data-form element.</exception>
        internal DataForm(XmlElement element, bool readOnly = false)
        {
            element.ThrowIfNull("element");
            this.Element = element;
            try
            {
                fields = new FieldList(element, readOnly);
                // Call GetDataFormType method to verify the 'type' attribute.
                GetDataFormType();
            }
            catch (XmlException e)
            {
                throw new ArgumentException("The element parameter is not a valid " +
                    "data-form.", e);
            }
        }

        /// <summary>
        /// Asserts the data-form is of the specified type.
        /// </summary>
        /// <param name="expected">The type to assert.</param>
        /// <exception cref="ArgumentException">The data-form is not of the
        /// expected type.</exception>
        protected void AssertType(DataFormType expected)
        {
            if (Type != expected)
            {
                throw new ArgumentException("The specified XML element is not a " +
                    "data-form of type '" + expected.ToString() + "'.");
            }
        }

        /// <summary>
        /// Returns the type of the data-form.
        /// </summary>
        /// <returns>The type of the data-form.</returns>
        /// <exception cref="XmlException">The 'type' attribute of the underlying
        /// XML element is invalid.</exception>
        private DataFormType GetDataFormType()
        {
            try
            {
                string t = Element.GetAttribute("type");
                return Util.ParseEnum<DataFormType>(t);
            }
            catch (Exception e)
            {
                throw new XmlException("The 'type' attribute of the underlying " +
                    "XML element is invalid.", e);
            }
        }

        /// <summary>
        /// Add a boolean value to this form
        /// </summary>
        /// <param name="name">The name of the value</param>
        /// <param name="value">The value to add</param>
        /// <returns>This data form</returns>
        public DataForm AddValue(string name, bool value)
        {
            Element.Child(new BooleanField(name, value).ToXmlElement());

            return this;
        }

        /// <summary>
        /// Add a Jid value to this form
        /// </summary>
        /// <param name="name">The name of the value</param>
        /// <param name="value">The value to add</param>
        /// <returns>This data form</returns>
        public DataForm AddValue(string name, Jid value)
        {
            value.ThrowIfNull();

            Element.Child(new JidField(name, value).ToXmlElement());

            return this;
        }

        /// <summary>
        /// Add a text value to this form
        /// </summary>
        /// <param name="name">The name of the value</param>
        /// <param name="value">The value to add</param>
        /// <returns>This data form</returns>
        public DataForm AddValue(string name, string value)
        {
            value.ThrowIfNull();

            Element.Child(new TextField(name, value).ToXmlElement());

            return this;
        }

        /// <summary>
        /// Add an untyped value to this form
        /// </summary>
        /// <param name="name">The name of the value</param>
        /// <param name="value">The value to add</param>
        /// <returns>This data form</returns>
        public DataForm AddUntypedValue(string name, object value)
        {
            var valueNode = Xml.Element("value", xmlns);
            if (value != null)
            {
                valueNode.Text(value.ToString());
            }

            Element.Child(Xml.Element("field", xmlns).Attr("var", name).Child(valueNode));

            return this;
        }

        /// <summary>
        /// Add a typed value to this form
        /// </summary>
        /// <param name="name">The name of the value</param>
        /// <param name="type">The type of the value to add</param>
        /// <param name="value">The value to add</param>
        /// <returns>This data form</returns>
        public DataForm AddValue(string name, DataFieldType type, object value)
        {
            var valueNode = Xml.Element("value", xmlns);
            if (value != null)
            {
                valueNode.Text(value.ToString());
            }

            Element.Child(Xml.Element("field", xmlns).Attr("var", name).Attr("type", type.ToString()).Child(valueNode));

            return this;
        }
    }
}