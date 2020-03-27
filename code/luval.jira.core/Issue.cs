using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Xml.Linq;

namespace luval.jira.core
{
    public class Issue : XmlItem
    {
        public Issue(XElement element) : base(element)
        {
            Labels = new List<Label>();
            CustomFiels = new List<CustomField>();
            var lbs = GetElement("labels");
            var cfs = GetElement("customfields");
            if(lbs != null)
                foreach (var label in lbs.Elements())
                {
                    Labels.Add(new Label(label));
                }
            if(cfs != null)
                foreach (var cf in cfs.Elements())
                {
                    CustomFiels.Add(new CustomField(cf));
                }
        }


        public string Title { get { return GetElementValueOrDefault<string>("title"); } }
        public string Link { get { return GetElementValueOrDefault<string>("link"); } }
        public string ProjectId { get { return GetAttributeOrDefault<string>(GetElement("project"), "id"); } }
        public string ProjectKey { get { return GetAttributeOrDefault<string>(GetElement("project"), "key"); } }
        public string ProjectName { get { return GetElementValueOrDefault<string>("project"); } }
        public string Description { get { return GetElementValueOrDefault<string>(GetElement("description"), "p"); } }
        public long IssueId { get { return GetAttributeOrDefault<long>(GetElement("key"), "id"); } }
        public string IssueKey { get { return GetElementValueOrDefault<string>("key"); } }
        public string Summary { get { return GetElementValueOrDefault<string>("summary"); } }
        public string IssueType { get { return GetElementValueOrDefault<string>("type"); } }
        public string Priority { get { return GetElementValueOrDefault<string>("priority"); } }
        public string Status { get { return GetElementValueOrDefault<string>("status"); } }
        public string Resolution { get { return GetElementValueOrDefault<string>("resolution"); } }
        public string Assignee { get { return GetElementValueOrDefault<string>("assignee"); } }
        public string Reporter { get { return GetElementValueOrDefault<string>("reporter"); } }
        public DateTime? Created { get { return GetElementValueOrDefault<DateTime?>("created"); } }
        public DateTime? Updated { get { return GetElementValueOrDefault<DateTime?>("updated"); } }
        public DateTime? Due { get { return GetElementValueOrDefault<DateTime?>("due"); } }
        public long OriginalEstimateInSeconds { get { return GetAttributeOrDefault<long>(GetElement("timeoriginalestimate"), "seconds"); } }
        public long EstimateInSeconds { get { return GetAttributeOrDefault<long>(GetElement("timeestimate"), "seconds"); } }
        public string EpicLink { get {  var res = CustomFiels.FirstOrDefault(i => i.Name == "Epic Link"); return res != null ? res.Values.First() : string.Empty; } }
        public string Sprint { get { var res = CustomFiels.FirstOrDefault(i => i.Name == "Sprint"); return res != null ? res.Values.First() : string.Empty; } }


        public List<Label> Labels { get; private set; }
        public List<CustomField> CustomFiels { get; private set; }




    }
}
