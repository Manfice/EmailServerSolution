using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace PriceParse
{
    public static class PriceManager
    {
        public static List<List<double>> ReadWorkbook(List<string> files)
        {
            var temp = @"C:\test1.xlsx";
            int maxCell = 0;
            List<double> cellVallength = new List<double>();
            List<List<double>> result = new List<List<double>>();
            int rc = 0;
            IWorkbook workbook = null;
            foreach (var file in files)
            {
                var fs = File.OpenRead(file);
                try
                {
                    if (file.EndsWith("xls", StringComparison.CurrentCultureIgnoreCase))
                    {
                        workbook = new HSSFWorkbook(fs);
                    }
                    else
                    {
                        workbook = WorkbookFactory.Create(fs);

                    }
                }
                catch (Exception e)
                {
                    continue;
                }
               // ISheet wSheet = workbook.GetSheetAt(0);
                foreach (ISheet sheet in workbook)
                {
                    foreach (IRow row in sheet)
                    {
                        maxCell = row.Select(cell => cell.ColumnIndex).Concat(new[] { maxCell }).Max();
                    }
                    rc = sheet.LastRowNum;
                    cellVallength = new List<double>();
                    for (var i = 0; i <= maxCell; i++)
                    {
                        double cvl = 0f;
                        var r = 0;
                        foreach (IRow row in sheet)
                        {
                            var s = string.Empty;
                            var c = row.GetCell(i);
                            if (c != null)
                                switch (c.CellType)
                                {
                                    case CellType.String:
                                        s = c.StringCellValue; break;
                                    case CellType.Numeric:
                                        s = c.NumericCellValue.ToString(CultureInfo.InvariantCulture); break;
                                    case CellType.Unknown:
                                        break;
                                    case CellType.Formula:
                                        break;
                                    case CellType.Blank:
                                        break;
                                    case CellType.Boolean:
                                        break;
                                    case CellType.Error:
                                        break;
                                    default:
                                        s = ""; break;
                                }
                            //cvl += s.Split(' ').Average(s1 => s1.Length);
                            cvl += s.Split(' ').Count();
                            r++;
                        }
                        cellVallength.Add(item: Math.Round(cvl / rc, 3));
                    }
                    result.Add(cellVallength);
                }
            }

            //using (var wc = new WebClient())
            //{
            //    wc.DownloadFile(new Uri("http://localhost:57025/mailapi/ApiFileMedia/GetFile"), temp);
            //}


            return result;
        }
    }
}
