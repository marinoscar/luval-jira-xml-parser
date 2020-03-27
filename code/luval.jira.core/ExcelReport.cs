using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
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
                using (var sheet = package.Workbook.Worksheets.Add("Use-Cases"))
                {
                    sheet.Cells[1, 1].Value = "Use Case Name";
                    sheet.Cells[1, 2].Value = "Total User Stories";
                    sheet.Cells[1, 3].Value = "Total Open";
                    sheet.Cells[1, 4].Value = "Total In Progress";
                    sheet.Cells[1, 5].Value = "Total In Completed";
                    sheet.Cells[1, 6].Value = "Total Progress";
                    sheet.Cells[1, 7].Value = "Original Due Date";
                    sheet.Cells[1, 8].Value = "Opp Intake - Count";
                    sheet.Cells[1, 9].Value = "Opp Intake - Open";
                    sheet.Cells[1, 10].Value = "Opp Intake - In Progress";
                    sheet.Cells[1, 11].Value = "Opp Intake - Completed";
                    sheet.Cells[1, 12].Value = "Opp Intake - Progress";
                    sheet.Cells[1, 13].Value = "Design - Count";
                    sheet.Cells[1, 14].Value = "Design - Open";
                    sheet.Cells[1, 15].Value = "Design - In Progress";
                    sheet.Cells[1, 16].Value = "Design - Completed";
                    sheet.Cells[1, 17].Value = "Design - Progress";
                    sheet.Cells[1, 18].Value = "Build - Count";
                    sheet.Cells[1, 19].Value = "Build - Open";
                    sheet.Cells[1, 20].Value = "Build - In Progress";
                    sheet.Cells[1, 21].Value = "Build - Completed";
                    sheet.Cells[1, 22].Value = "Build - Progress";
                }
            }
        }
    }
}
