using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeOpenXml;
using PALMS.Reports.Common;
using PALMS.Reports.Epplus.Builders;
using PALMS.Reports.Epplus.Model;

namespace PALMS.Reports.Epplus.Tests
{
    [TestClass]
    public class ExcelDataTest
    {
        [TestMethod]
        public void File_Load_Loaded()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "template.xlsx");

            // Act
            var excelPackage = FileFactory.LoadPackage(path);

            // Assert
            Assert.IsNotNull(excelPackage);

            excelPackage.Dispose();
        }

        [TestMethod]
        public void File_Save_Saved()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "template.xlsx");
            var savedFile = Path.Combine(Directory.GetCurrentDirectory(), "saved.xlsx");

            // Act
            using (var excelPackage = FileFactory.LoadPackage(path))
            {
                excelPackage.SaveAs(new FileInfo(savedFile));
            }

            // Assert
            Assert.IsTrue(File.Exists(savedFile));
        }

        [TestMethod]
        public void Template_InitHeader_Saved()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "template.xlsx");
            var savedFile = Path.Combine(Directory.GetCurrentDirectory(), "savedTemplate.xlsx");
            var sheetName = "Sheet1";
            var data = FakeData.GetData();

            // Act
            using (var excelPackage = FileFactory.LoadPackage(path))
            {
                var sheet = excelPackage.Workbook.Worksheets[sheetName];

                new ReportBuilder(sheet, data).SetHeader();
                //sheet.SetHeader(data);

                excelPackage.SaveAs(new FileInfo(savedFile));
            }

            // Assert

            using (var package = FileFactory.LoadPackage(savedFile))
            {
                var sheet = package.Workbook.Worksheets[sheetName];

                Assert.AreEqual(data.Name, sheet.Cells[1, 3].Value);
                Assert.AreEqual(data.Description, sheet.Cells[2, 3].Value);
                Assert.AreEqual(data.Month, sheet.Cells[3, 3].Value);
                Assert.AreEqual(data.NoteType, sheet.Cells[4, 3].Value);
            }
        }

        [TestMethod]
        public void Template_OneColumn_Saved()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "template.xlsx");
            var savedFile = Path.Combine(Directory.GetCurrentDirectory(), "savedTemplate.xlsx");
            var sheetName = "Sheet1";
            var data = FakeData.GetData(29);

            using (var excelPackage = FileFactory.LoadPackage(path))
            {
                var sheet = excelPackage.Workbook.Worksheets[sheetName];

                // Act
                new ReportBuilder(sheet, data).Build();

                excelPackage.SaveAs(new FileInfo(savedFile));
            }

            // Assert

            using (var package = FileFactory.LoadPackage(savedFile))
            {
                var sheet = package.Workbook.Worksheets[sheetName];

                Assert.AreEqual(data.Name, sheet.Cells[1, 3].Value);
                Assert.AreEqual(data.Description, sheet.Cells[2, 3].Value);
            }
        }

        [TestMethod]
        public void Template_ManyColumns_Saved()
        {
            // Arrange
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "template3.xlsx");
            var savedFile = Path.Combine(Directory.GetCurrentDirectory(), "savedTemplate3.xlsx");
            var sheetName = "Sheet1";
            var data = FakeData.GetData2();

            using (var excelPackage = FileFactory.LoadPackage(path))
            {
                var sheet = excelPackage.Workbook.Worksheets[sheetName];

                // Act
                new ReportBuilder3(sheet, data).Build();

                sheet.Calculate();

                excelPackage.SaveAs(new FileInfo(savedFile));
            }

            // Assert

            using (var package = FileFactory.LoadPackage(savedFile))
            {
                var sheet = package.Workbook.Worksheets[sheetName];

                Assert.AreEqual(data.Name, sheet.Cells[1, 3].Value);
                Assert.AreEqual(data.Description, sheet.Cells[2, 3].Value);
            }
        }
    }

}
