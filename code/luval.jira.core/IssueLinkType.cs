using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace luval.jira.core
{
    public class IssueLinkType : XmlItem
    {
        public IssueLinkType(XElement el) : base(el) {
            Inwards = new List<IssueLink>();
            Outward = new List<IssueLink>();
            foreach(var inw in Element.Elements().Where(i => i.Name == "outwardlinks"))
            {
                foreach (var lnk in inw.Elements().Where(i => i.Name == "issuelink"))
                {
                    Outward.Add(new IssueLink(lnk));
                }
            }
            foreach (var outw in Element.Elements().Where(i => i.Name == "inwardlinks"))
            {
                foreach (var lnk in outw.Elements().Where(i => i.Name == "issuelink"))
                {
                    Inwards.Add(new IssueLink(lnk));
                }
            }
        }

        public string Id { get { return GetAttributeOrDefault<string>("id"); } }
        public string Name { get { return GetElementValueOrDefault<string>("name"); } }
        public List<IssueLink> Inwards { get; set; }
        public List<IssueLink> Outward { get; set; }

    }
}
