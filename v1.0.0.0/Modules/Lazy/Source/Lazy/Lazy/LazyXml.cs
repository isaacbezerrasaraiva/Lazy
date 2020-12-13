// LazyXml.cs
//
// This file is integrated part of Ark project
// Licensed under "Gnu General Public License Version 3"
//
// Created by Isaac Bezerra Saraiva
// Created on 2020, November 24

using System;
using System.Xml;
using System.Data;

namespace Lazy
{
    public class LazyXml
    {
        #region Variables

        private XmlDocument xmlDocument;

        #endregion Variables

        #region Constructors

        public LazyXml()
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Create new xml document
        /// </summary>
        public void New()
        {
            this.xmlDocument = new XmlDocument();
        }

        /// <summary>
        /// Open an existing xml document from file
        /// </summary>
        /// <param name="path">The xml document file path</param>
        public void Open(String path)
        {
            this.xmlDocument = new XmlDocument();
            this.xmlDocument.Load(path);
        }

        /// <summary>
        /// Open an existing xml document from memory
        /// </summary>
        /// <param name="xml">The xml document</param>
        public void OpenXml(String xml)
        {
            this.xmlDocument = new XmlDocument();
            this.xmlDocument.LoadXml(xml);
        }

        /// <summary>
        /// Save current xml document to file
        /// </summary>
        /// <param name="path">The xml document file path</param>
        public void Save(String path)
        {
            this.xmlDocument.Save(path);
        }
        
        /// <summary>
        /// Read a xml node
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <returns>The located xml node</returns>
        public XmlNode ReadNode(String xPath)
        {
            return this.xmlDocument.SelectSingleNode(xPath);
        }
        
        /// <summary>
        /// Read a xml node child
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <param name="childName">The xml node child name</param>
        /// <returns>The located xml node child</returns>
        public XmlNode ReadNodeChild(String xPath, String childName)
        {
            return this.xmlDocument.SelectSingleNode(xPath).SelectSingleNode(childName);
        }

        /// <summary>
        /// Read a xml node child
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <param name="childName">The xml node child name</param>
        /// <returns>The located xml node child</returns>
        public XmlNode ReadNodeChild(XmlNode node, String childName)
        {
            return node.SelectSingleNode(childName);
        }
        
        /// <summary>
        /// Read a xml node list
        /// </summary>
        /// <param name="xPath">The xPath of the xml node list</param>
        /// <returns>The located xml node list</returns>
        public XmlNodeList ReadNodeList(String xPath)
        {
            return this.xmlDocument.SelectNodes(xPath);
        }
        
        /// <summary>
        /// Read a xml node child list
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <returns>The located xml node child list</returns>
        public XmlNodeList ReadNodeChildList(String xPath)
        {
            return this.xmlDocument.SelectSingleNode(xPath).ChildNodes;
        }

        /// <summary>
        /// Read a xml node child list
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <param name="childName">The xml node child name</param>
        /// <returns>The located xml node child list</returns>
        public XmlNodeList ReadNodeChildList(String xPath, String childName)
        {
            return this.xmlDocument.SelectSingleNode(xPath).SelectNodes(childName);
        }

        /// <summary>
        /// Read a xml node child list
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <returns>The located xml node child list</returns>
        public XmlNodeList ReadNodeChildList(XmlNode node)
        {
            return node.ChildNodes;
        }

        /// <summary>
        /// Read a xml node child list
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <param name="childName">The xml node child name</param>
        /// <returns>The located xml node child list</returns>
        public XmlNodeList ReadNodeChildList(XmlNode node, String childName)
        {
            return node.SelectNodes(childName);
        }

        /// <summary>
        /// Read a xml node attribute
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <param name="attributeName">The xml node attribute name</param>
        /// <returns>The located xml node attribute</returns>
        public XmlAttribute ReadNodeAttribute(String xPath, String attributeName)
        {
            return this.xmlDocument.SelectSingleNode(xPath).Attributes[attributeName];
        }

        /// <summary>
        /// Read a xml node attribute
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <param name="attributeName">The xml node attribute name</param>
        /// <returns>The located xml node attribute</returns>
        public XmlAttribute ReadNodeAttribute(XmlNode node, String attributeName)
        {
            return node.Attributes[attributeName];
        }

        /// <summary>
        /// Read a xml node attribute collection
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <returns>The located xml node attribute collection</returns>
        public XmlAttributeCollection ReadNodeAttributeList(String xPath)
        {
            return this.xmlDocument.SelectSingleNode(xPath).Attributes;
        }

        /// <summary>
        /// Read a xml node attribute collection
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <returns>The located xml node attribute collection</returns>
        public XmlAttributeCollection ReadNodeAttributeList(XmlNode node)
        {
            return node.Attributes;
        }

        /// <summary>
        /// Read a xml node attribute value
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <param name="attributeName">The xml node attribute name</param>
        /// <returns>The located xml node attribute value</returns>
        public String ReadNodeAttributeValue(String xPath, String attributeName)
        {
            return this.xmlDocument.SelectSingleNode(xPath).Attributes[attributeName].Value;
        }

        /// <summary>
        /// Read a xml node attribute value
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <param name="attributeName">The xml node attribute name</param>
        /// <returns>The located xml node attribute value</returns>
        public String ReadNodeAttributeValue(XmlNode node, String attributeName)
        {
            return node.Attributes[attributeName].Value;
        }

        /// <summary>
        /// Read a xml node inner text
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <returns>The located xml node inner text</returns>
        public String ReadNodeInnerText(String xPath)
        {
            return this.xmlDocument.SelectSingleNode(xPath).InnerText;
        }

        /// <summary>
        /// Read a xml node inner text
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <returns>The located xml node inner text</returns>
        public String ReadNodeInnerText(XmlNode node)
        {
            return node.InnerText;
        }

        /// <summary>
        /// Write the xml root node
        /// </summary>
        /// <param name="rootName">The xml root node name</param>
        /// <returns>The xml root node written</returns>
        public XmlNode WriteRoot(String rootName)
        {
            XmlNode xmlNodeRoot = this.xmlDocument.CreateNode(XmlNodeType.Element, rootName, null);
            this.xmlDocument.AppendChild(xmlNodeRoot);

            XmlDeclaration declaration = xmlDocument.CreateXmlDeclaration("1.0", "utf-8", String.Empty);
            xmlDocument.InsertBefore(declaration, xmlNodeRoot);

            return xmlNodeRoot;
        }
        
        /// <summary>
        /// Write a xml node
        /// </summary>
        /// <param name="xPathParent">The xPath of the xml parent node</param>
        /// <param name="nodeName">The xml node name</param>
        /// <returns>The xml node written</returns>
        public XmlNode WriteNode(String xPathParent, String nodeName)
        {
            return WriteNode(this.xmlDocument.SelectSingleNode(xPathParent), nodeName);
        }

        /// <summary>
        /// Write a xml node
        /// </summary>
        /// <param name="nodeParent">The xml parent node</param>
        /// <param name="nodeName">The xml node name</param>
        /// <returns>The xml node written</returns>
        public XmlNode WriteNode(XmlNode nodeParent, String nodeName)
        {
            XmlNode xmlNodeChild = xmlDocument.CreateNode(XmlNodeType.Element, nodeName, null);
            nodeParent.AppendChild(xmlNodeChild);

            return xmlNodeChild;
        }

        /// <summary>
        /// Write a xml node
        /// </summary>
        /// <param name="nodeParent">The xml parent node</param>
        /// <param name="node">The xml node</param>
        public void WriteNode(XmlNode nodeParent, XmlNode node)
        {
            nodeParent.AppendChild(node);
        }

        /// <summary>
        /// Write a xml node attribute
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <param name="attributeName">The xml node attribute name</param>
        /// <returns>The xml node attribute written</returns>
        public XmlAttribute WriteNodeAttribute(String xPath, String attributeName)
        {
            return WriteNodeAttribute(this.xmlDocument.SelectSingleNode(xPath), attributeName, String.Empty);
        }

        /// <summary>
        /// Write a xml node attribute
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <param name="attributeName">The xml node attribute name</param>
        /// <param name="attributeValue">The xml node attribute value</param>
        /// <returns>The xml node attribute written</returns>
        public XmlAttribute WriteNodeAttribute(String xPath, String attributeName, String attributeValue)
        {
            return WriteNodeAttribute(this.xmlDocument.SelectSingleNode(xPath), attributeName, attributeValue);
        }

        /// <summary>
        /// Write a xml node attribute
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <param name="attributeName">The xml node attribute name</param>
        /// <returns>The xml node attribute written</returns>
        public XmlAttribute WriteNodeAttribute(XmlNode node, String attributeName)
        {
            return WriteNodeAttribute(node, attributeName, String.Empty);
        }

        /// <summary>
        /// Write a xml node attribute
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <param name="attributeName">The xml node attribute name</param>
        /// <param name="attributeValue">The xml node attribute value</param>
        /// <returns>The xml node attribute written</returns>
        public XmlAttribute WriteNodeAttribute(XmlNode node, String attributeName, String attributeValue)
        {
            XmlAttribute xmlAttribute = this.xmlDocument.CreateAttribute(attributeName);
            xmlAttribute.Value = attributeValue;

            node.Attributes.Append(xmlAttribute);

            return xmlAttribute;
        }

        /// <summary>
        /// Write a xml node attribute
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <param name="attribute">The xml node attribute</param>
        public void WriteNodeAttribute(XmlNode node, XmlAttribute attribute)
        {
            node.Attributes.Append(attribute);
        }

        /// <summary>
        /// Write a xml node inner text
        /// </summary>
        /// <param name="xPath">The xPath of the xml node</param>
        /// <param name="innerText">The xml node inner text</param>
        public void WriteNodeInnerText(String xPath, String innerText)
        {
            this.xmlDocument.SelectSingleNode(xPath).InnerText = innerText;
        }

        /// <summary>
        /// Write a xml node inner text
        /// </summary>
        /// <param name="node">The xml node</param>
        /// <param name="innerText">The xml node inner text</param>
        public void WriteNodeInnerText(XmlNode node, String innerText)
        {
            node.InnerText = innerText;
        }

        #endregion Methods

        #region Properties

        public XmlDocument XmlDocument
        {
            get { return this.xmlDocument; }
        }

        #endregion Properties
    }
}