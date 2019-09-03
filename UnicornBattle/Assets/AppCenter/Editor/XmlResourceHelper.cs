// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.IO;
using System.Xml;

public static class XmlResourceHelper
{
    public static void WriteXmlResource(string path, IDictionary<string, string> resourceValues)
    {
        var xws = new XmlWriterSettings
        {
            Indent = true
        };
        using (var sw = File.Create(path))
        using (var xw = XmlWriter.Create(sw, xws))
        {
            xw.WriteStartDocument();
            xw.WriteStartElement("resources");

            foreach (var kvp in resourceValues)
            {
                if (!string.IsNullOrEmpty(kvp.Value))
                {
                    xw.WriteStartElement("string");
                    xw.WriteAttributeString("name", kvp.Key);
                    xw.WriteAttributeString("translatable", "false");
                    xw.WriteString(kvp.Value);
                    xw.WriteEndElement();
                }
            }

            xw.WriteEndElement();
            xw.WriteEndDocument();
            xw.Flush();
            xw.Close();
        }
    }
}
