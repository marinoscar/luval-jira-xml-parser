using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                d["Labels"] = string.Join(";", issue.Labels.Select(i => i.Name));
                d["Phase"] = issue.Labels.Any(i => i.Name.ToLowerInvariant().StartsWith("phase")) ? issue.Labels.First(i => i.Name.ToLowerInvariant().StartsWith("phase")).Name : "";
                d["IsOppIntake"] = issue.Labels.Any(i => i.Name.ToLowerInvariant().Equals("phase-opp-intake")) ? 1 : 0;
                d["IsDesign"] = issue.Labels.Any(i => i.Name.ToLowerInvariant().Equals("phase-design")) ? 1 : 0;
                d["IsBuild"] = issue.Labels.Any(i => i.Name.ToLowerInvariant().Equals("phase-build")) ? 1 : 0;
                d["IsTest"] = issue.Labels.Any(i => i.Name.ToLowerInvariant().Equals("phase-test")) ? 1 : 0;
                d["IsDeploy"] = issue.Labels.Any(i => i.Name.ToLowerInvariant().Equals("phase-deploy")) ? 1 : 0;
                d["InwardsLinks"] = string.Join(",", issue.LinkTypes.SelectMany(i => i.Inwards).Select(i => i.IssueKey));
                d["OutwardsLinks"] = string.Join(",", issue.LinkTypes.SelectMany(i => i.Outward).Select(i => i.IssueKey));
                d["InwardsBlockers"] = string.Join(",", issue.LinkTypes.Where(i => i.Name == "Blocks").SelectMany(i => i.Inwards).Select(i => i.IssueKey));
                d["OutwardsBlockers"] = string.Join(",", issue.LinkTypes.Where(i => i.Name == "Blocks").SelectMany(i => i.Outward).Select(i => i.IssueKey));
                d["IsBlocked"] = string.IsNullOrWhiteSpace(Convert.ToString(d["OutwardsBlockers"])) && string.IsNullOrWhiteSpace(Convert.ToString(d["InwardsBlockers"])) ? 0 : 1;

                res.Add(d);
            }
            return res;
        }


    }
}
