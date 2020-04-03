using OfficeOpenXml;
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
        public void DoReport(Stream fileStream, Search search)
        {
            DoReport(new ExcelPackage(fileStream), search);
        }

        public void DoReport(FileInfo file, Search search)
        {
            DoReport(new ExcelPackage(file), search);
        }

        public void DoReport(ExcelPackage package, Search search)
        {
            using (package)
            {
                using (var dataSheet = GetAllRecords(package, search))
                {
                    using (var pivotSheet = CreatePivotTable(package, dataSheet))
                    {
                        package.Save();
                    }
                }
            }
        }

        public ExcelWorksheet GetAllRecords(ExcelPackage package, Search search)
        {
            var sheet = package.Workbook.Worksheets.Add("All-Records");
            var items = search.ToList();
            if (!items.Any()) return sheet;
            var epics = items.Where(i => (string)i["IssueType"] == "Epic").ToList();
            var keys = epics.Select(i => i["IssueKey"].ToString()).ToList();
            var bus = new[] { "O2C", "P2P", "R2R", "TAX", "COE", "I&W", "I_amp;amp;W" };

            foreach (var item in items)
            {
                var epic = epics.FirstOrDefault(i => (string)i["IssueKey"] == (string)item["EpicLink"]);
                item["ParentUseCase"] = epic != null ? epic["Summary"] : default(string);
                item["ParentUseCaseLink"] = epic != null ? epic["Link"] : default(string);
                if (epic != null && !string.IsNullOrWhiteSpace(Convert.ToString(epic["Labels"])))
                {
                    item["BusinessGroup"] = ((string)epic["Labels"]).Split(";".ToCharArray()).FirstOrDefault(i => bus.Contains(i.ToUpper().Trim()));
                    epic["BusinessGroup"] = item["BusinessGroup"];
                }
                else
                    item["BusinessGroup"] = null;
            }
            var cols = items.First().Keys.ToArray();
            for (int i = 0; i < cols.Length; i++)
            {
                sheet.Cells[1, i + 1].Value = cols[i];
                if ((new[] { "Completed", "Updated", "Due" }).Contains(cols[i]))
                    sheet.Cells[1, i + 1].Style.Numberformat.Format = "MM/dd/yyyy hh:mm:ss";
            }
            for (int r = 0; r < items.Count; r++)
            {
                for (int c = 0; c < cols.Length; c++)
                {
                    var val = items[r][cols[c]];
                    if (val is string && !string.IsNullOrWhiteSpace((string)val)) val = ((string)val).Trim();
                    sheet.Cells[r + 2, c + 1].Value = val;
                }
            }
            sheet.Tables.Add(sheet.SelectedRange[1, 1, items.Count + 1, cols.Length], "Issues");
            return sheet;
        }

        public ExcelWorksheet CreatePivotTable(ExcelPackage package, ExcelWorksheet dataSheet)
        {
            var sheet = package.Workbook.Worksheets.Add("Pivot");
            var pivotTable = sheet.PivotTables.Add(sheet.Cells["A3"], dataSheet.Cells[dataSheet.Tables["Issues"].Address.Address], "PivotData");

            pivotTable.RowFields.Add(pivotTable.Fields["BusinessGroup"]);
            pivotTable.RowFields.Add(pivotTable.Fields["ParentUseCase"]);
            pivotTable.ColumnFields.Add(pivotTable.Fields["Status"]);

            FormatValueField(ref pivotTable, "IssueKey", "Total Stories");
            FormatValueField(ref pivotTable, "IsOppIntake", "Opp Intake");
            FormatValueField(ref pivotTable, "IsDesign", "Design");
            FormatValueField(ref pivotTable, "IsBuild", "Build");
            FormatValueField(ref pivotTable, "IsTest", "Test");
            FormatValueField(ref pivotTable, "IsDeploy", "Deploy");

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
