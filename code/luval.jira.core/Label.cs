using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace luval.jira.core
{
    public class Label : XmlItem
    {
        public Label(XElement element) : base(element) { }

        public string Name { get { return GetElementValueOrDefault<string>(); } }

    }
}
