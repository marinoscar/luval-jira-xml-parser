using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace luval.jira.core
{
    public class IssueLink : XmlItem
    {
        public IssueLink(XElement el) : base(el) { }

        public string Id { get { return GetAttributeOrDefault<string>("id"); } }
        public string IssueKey { get { return GetElementValueOrDefault<string>("issuekey"); } }
    }
}
