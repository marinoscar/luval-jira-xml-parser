using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace luval.jira.core
{
    public class Search : XmlItem
    {
        public Search(XElement element) : base(element) 
        {
            Element = GetElement("channel");
            Issues = new List<Issue>();
            var iss = Element.Elements().Where(i => i.Name == "item").ToList();
            if (iss == null) return;
            foreach (var issue in iss)
            {
                Issues.Add(new Issue(issue));
            }
        }

        public string Title { get { return GetElementValueOrDefault<string>("title"); } }
        public string Link { get { return GetElementValueOrDefault<string>("link"); } }
        public List<Issue> Issues { get; private set; }

        public List<Dictionary<string, object>> ToList()
        {
            var res = new List<Dictionary<string, object>>();
            foreach(var issue in Issues)
            {
                var d = new Dictionary<string, object>();
                d["SearchTitle"] = Title;
                d["SearchLink"] = Link;
                d["ProjectId"] = issue.ProjectId;
                d["ProjectKey"] = issue.ProjectKey;
                d["ProjectName"] = issue.ProjectName;
                d["ProjectName"] = issue.ProjectName;
                d["Title"] = issue.Title;
                d["Link"] = issue.Link;
                d["IssueId"] = issue.IssueId;
                d["IssueKey"] = issue.IssueKey;
                d["Summary"] = issue.Summary;
                d["IssueType"] = issue.IssueType;
                d["Priority"] = issue.Priority;
                d["Status"] = issue.Status;
                d["Resolution"] = issue.Resolution;
                d["Assignee"] = issue.Assignee;
                d["Reporter"] = issue.Reporter;
                d["Created"] = issue.Created;
                d["Updated"] = issue.Updated;
                d["Due"] = issue.Due;
                d["OriginalEstimateInSeconds"] = issue.OriginalEstimateInSeconds;
                d["EstimateInSeconds"] = issue.EstimateInSeconds;
                d["EpicLink"] = issue.EpicLink;
                d["Sprint"] = issue.Sprint;
                d["Labels"] = string.Join(";", issue.Labels);

                res.Add(d);
            }
            return res;
        }


    }
}
