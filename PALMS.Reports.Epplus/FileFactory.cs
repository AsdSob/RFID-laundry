using System.IO;
using OfficeOpenXml;

namespace PALMS.Reports.Epplus
{
    public static class FileFactory
    {
        public static ExcelPackage LoadPackage(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);

            var fileInfo = new FileInfo(path);

            return new ExcelPackage(fileInfo);
        }
    }
}
