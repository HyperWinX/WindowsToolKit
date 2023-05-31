using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace WindowsToolKit
{
    internal static class Plist
    {
        internal static Dictionary<string, object> Parse(Stream stream)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;

            XmlReaderSettings settings = new XmlReaderSettings();
            // DTD processing is disabled on anything but .NET 2.0, so this must be set to
            // Ignore.
            // See https://msdn.microsoft.com/en-us/magazine/ee335713.aspx for additional information.
            settings.DtdProcessing = DtdProcessing.Ignore;

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                xmlDoc.Load(reader);
            }

            XmlElement root = xmlDoc.DocumentElement;
            if (root.Name != "plist")
            {
                throw new InvalidDataException("XML document is not a plist");
            }

            return ParseDictionary(root.FirstChild);
        }

        internal static void Write(Stream stream, Dictionary<string, object> plist)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;

            XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlDoc.AppendChild(xmlDecl);

            XmlDocumentType xmlDocType = xmlDoc.CreateDocumentType("plist", "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
            xmlDoc.AppendChild(xmlDocType);

            XmlElement rootElement = xmlDoc.CreateElement("plist");
            rootElement.SetAttribute("Version", "1.0");
            xmlDoc.AppendChild(rootElement);

            xmlDoc.DocumentElement.SetAttribute("Version", "1.0");

            rootElement.AppendChild(CreateNode(xmlDoc, plist));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;

            using (XmlWriter xw = XmlWriter.Create(stream, settings))
            {
                xmlDoc.Save(xw);
            }
        }

        private static object ParseNode(XmlNode xmlNode)
        {
            switch (xmlNode.Name)
            {
                case "dict":
                    return ParseDictionary(xmlNode);
                case "array":
                    return ParseArray(xmlNode);
                case "string":
                    return ParseString(xmlNode);
                case "data":
                    return ParseData(xmlNode);
                case "integer":
                    return ParseInteger(xmlNode);
                case "true":
                    return true;
                case "false":
                    return false;
                default:
                    throw new NotImplementedException();
            }
        }

        private static XmlNode CreateNode(XmlDocument xmlDoc, object obj)
        {
            if (obj is Dictionary<string, object>)
            {
                return CreateDictionary(xmlDoc, (Dictionary<string, object>)obj);
            }
            if (obj is string)
            {
                XmlText text = xmlDoc.CreateTextNode((string)obj);
                XmlElement node = xmlDoc.CreateElement("string");
                node.AppendChild(text);
                return node;
            }
            throw new NotImplementedException();
        }

        private static XmlNode CreateDictionary(XmlDocument xmlDoc, Dictionary<string, object> dict)
        {
            XmlElement dictNode = xmlDoc.CreateElement("dict");

            foreach (KeyValuePair<string, object> entry in dict)
            {
                XmlText text = xmlDoc.CreateTextNode(entry.Key);
                XmlElement keyNode = xmlDoc.CreateElement("key");
                keyNode.AppendChild(text);

                dictNode.AppendChild(keyNode);

                XmlNode valueNode = CreateNode(xmlDoc, entry.Value);
                dictNode.AppendChild(valueNode);
            }

            return dictNode;
        }

        private static Dictionary<string, object> ParseDictionary(XmlNode xmlNode)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            XmlNode focusNode = xmlNode.FirstChild;
            while (focusNode != null)
            {
                if (focusNode.Name != "key")
                {
                    throw new InvalidDataException("Invalid plist, expected dictionary key");
                }

                string key = focusNode.InnerText;

                focusNode = focusNode.NextSibling;

                result.Add(key, ParseNode(focusNode));

                focusNode = focusNode.NextSibling;
            }

            return result;
        }

        private static object ParseArray(XmlNode xmlNode)
        {
            List<object> result = new List<object>();

            XmlNode focusNode = xmlNode.FirstChild;
            while (focusNode != null)
            {
                result.Add(ParseNode(focusNode));
                focusNode = focusNode.NextSibling;
            }

            return result;
        }

        private static object ParseString(XmlNode xmlNode)
        {
            return xmlNode.InnerText;
        }

        private static object ParseData(XmlNode xmlNode)
        {
            string base64 = xmlNode.InnerText;
            return Convert.FromBase64String(base64);
        }

        private static object ParseInteger(XmlNode xmlNode)
        {
            return int.Parse(xmlNode.InnerText, CultureInfo.InvariantCulture);
        }
    }
}