using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace luval.jira.core
{
    public class Comment : XmlItem
    {
        public Comment(XElement element) : base(element) { }

        public string Id { get { return GetAttributeOrDefault<string>("id"); } }
        public string Author { get { return GetAttributeOrDefault<string>("author"); } }
        public string Value { get { return GetElementValueOrDefault<string>(); } }

    }
}
