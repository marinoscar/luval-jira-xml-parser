using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace luval.jira.core
{
    public class CustomFieldValue : XmlItem
    {
        public CustomFieldValue(XElement element) : base(element)
        {
            Values = new List<CustomFieldValue>();
            var array = GetElement("customfields");
            if(array != null)
                foreach (var cf in (IEnumerable<XElement>)array)
                {
                    Values.Add(new CustomFieldValue(cf));
                }
        }

        public string Name { get { return GetElementValueOrDefault<string>("customfieldname"); } }
        public List<CustomFieldValue> Values { get; private set; }
    }
}
