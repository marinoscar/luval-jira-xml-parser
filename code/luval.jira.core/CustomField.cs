using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace luval.jira.core
{
    public class CustomField : XmlItem
    {
        public CustomField(XElement element) : base(element)
        {
            Values = new List<string>();
            var vals = GetElement("customfieldvalues");
            if (vals == null) return;
            foreach (var val in vals.Elements())
            {
                Values.Add(GetElementValueOrDefault<string>(val));
            }
        }

        public string Id { get { return GetAttributeOrDefault<string>("id"); } }
        public string Key { get { return GetAttributeOrDefault<string>("key"); } }
        public string Name { get { return GetElementValueOrDefault<string>("customfieldname"); } }
        public List<string> Values { get; private set; }
    }
}
