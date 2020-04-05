using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.Table.PivotTable;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace luval.jira.core
{
    public class ExcelReport
    {
        public void DoReport(Stream fileStream, Search search, IEnumerable<string> businessUnits)
        {
            DoReport(new ExcelPackage(fileStream), search, businessUnits);
        }

        public void DoReport(FileInfo file, Search search, IEnumerable<string> businessUnits)
        {
            DoReport(new ExcelPackage(file), search, businessUnits);
        }

        public void DoReport(ExcelPackage package, Search search, IEnumerable<string> businessUnits)
        {
            using (package)
            {
                using (var dataSheet = GetAllRecords(package, search, businessUnits))
                {
                    using (var pivotSheet = CreatePivotTable(package, dataSheet))
                    {
                        package.Save();
                    }
                }
            }
        }

        public ExcelWorksheet GetAllRecords(ExcelPackage package, Search search, IEnumerable<string> businessUnits)
        {
            var sheet = package.Workbook.Worksheets.Add("All-Records");
            var items = search.ToList();
            if (!items.Any()) return sheet;
            var epics = items.Where(i => (string)i["IssueType"] == "Epic").ToList();
            var keys = epics.Select(i => i["IssueKey"].ToString()).ToList();

            foreach (var item in items)
            {
                item["Phase-Name"] = GetPhaseName(Convert.ToString(item["Phase"]));
                item["Status-Name"] = GetStatusName(Convert.ToString(item["Status"]));

                var epic = epics.FirstOrDefault(i => (string)i["IssueKey"] == (string)item["EpicLink"]);
                item["ParentUseCase"] = epic != null ? epic["Summary"] : default(string);
                item["ParentUseCaseLink"] = epic != null ? epic["Link"] : default(string);
                if (epic != null && !string.IsNullOrWhiteSpace(Convert.ToString(epic["Labels"])))
                {
                    item["BusinessGroup"] = ((string)epic["Labels"]).Split(";".ToCharArray()).FirstOrDefault(i => businessUnits.Contains(i.ToUpper().Trim()));
                    epic["BusinessGroup"] = item["BusinessGroup"];
                }
                else
                    item["BusinessGroup"] = null;
                if (item["Resolved"] == null)
                    item["SecondsToResolution"] = DateTime.UtcNow.Subtract(((DateTimeOffset)item["Created"]).UtcDateTime).TotalSeconds;
                else
                    item["SecondsToResolution"] = DateTime.UtcNow.Subtract(((DateTimeOffset)item["Resolved"]).UtcDateTime).TotalSeconds;
            }
            var cols = items.First().Keys.ToArray();
            for (int i = 0; i < cols.Length; i++)
            {
                sheet.Cells[1, i + 1].Value = cols[i];
            }
            for (int r = 0; r < items.Count; r++)
            {
                for (int c = 0; c < cols.Length; c++)
                {
                    var val = items[r][cols[c]];
                    if (val is string && !string.IsNullOrWhiteSpace((string)val)) val = ((string)val).Trim();
                    if (val != null && val.GetType() == typeof(DateTimeOffset))
                    {
                        var dt = DateTimeOffset.Parse(Convert.ToString(val)).DateTime;
                        sheet.Cells[r + 2, c + 1].Style.Numberformat.Format = "MM/dd/yyyy HH:mm";
                        //sheet.Cells[r + 2, c + 1].Formula = string.Format("=DATE({0},{1},{2}) + TIME({3},{4},{5})", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
                        sheet.Cells[r + 2, c + 1].Value = dt;
                    }
                    else
                        sheet.Cells[r + 2, c + 1].Value = val;
                }
            }
            sheet.Tables.Add(sheet.SelectedRange[1, 1, items.Count + 1, cols.Length], "Issues");
            return sheet;
        }

        private string GetPhaseName(string phase)
        {
            var phases = new Dictionary<string, string>() {
                { "Phase-Opp-Intake", "1 - Opp Intake"},
                { "Phase-Design", "2 - Design"},
                { "Phase-Build", "3 - Build"},
                { "Phase-Test", "4 - Test"},
                { "Phase-Deploy", "5 - Deploy"}
            };
            return phases.ContainsKey(phase.Trim()) ? phases[phase.Trim()] : "99 - Other";
        }

        private string GetStatusName(string status)
        {
            var phases = new Dictionary<string, string>() {
                { "To Do", "1 - To Do"},
                { "In Progress", "2 - In Progress"},
                { "Done", "3 - Done"},
                { "Resolved", "4 - Resolved"},
                { "Open", "0 - Open"}
            };
            return phases.ContainsKey(status.Trim()) ? phases[status.Trim()] : "99 - Other";
        }

        public ExcelWorksheet CreatePivotTable(ExcelPackage package, ExcelWorksheet dataSheet)
        {
            var sheet = package.Workbook.Worksheets.Add("Pivot");
            var pivotTable = sheet.PivotTables.Add(sheet.Cells["A3"], dataSheet.Cells[dataSheet.Tables["Issues"].Address.Address], "PivotData");

            pivotTable.RowFields.Add(pivotTable.Fields["ProjectKey"]);
            pivotTable.RowFields.Add(pivotTable.Fields["BusinessGroup"]);
            pivotTable.RowFields.Add(pivotTable.Fields["ParentUseCase"]);
            pivotTable.ColumnFields.Add(pivotTable.Fields["Phase-Name"]);
            pivotTable.ColumnFields.Add(pivotTable.Fields["Status-Name"]);

            FormatValueField(ref pivotTable, "IssueKey", "Total Stories");
            //FormatValueField(ref pivotTable, "IsOppIntake", "Opp Intake");
            //FormatValueField(ref pivotTable, "IsDesign", "Design");
            //FormatValueField(ref pivotTable, "IsBuild", "Build");
            //FormatValueField(ref pivotTable, "IsTest", "Test");
            //FormatValueField(ref pivotTable, "IsDeploy", "Deploy");

            return sheet;
        }

        private void FormatValueField(ref ExcelPivotTable pivotTable, string fielName, string caption)
        {
            var totalCount = pivotTable.DataFields.Add(pivotTable.Fields[fielName]);
            totalCount.Name = caption;
            totalCount.Function = DataFieldFunctions.Count;
        }
    }
}
