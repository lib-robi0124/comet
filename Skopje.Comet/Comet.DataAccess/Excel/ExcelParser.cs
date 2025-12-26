using ClosedXML.Excel;

namespace Comet.DataAccess.Excel
{
    public class ExcelParser : IExcelParser
    {
        public List<T> Parse<T>(Stream stream) where T : new()
        {
            var result = new List<T>();

            using var workbook = new XLWorkbook(stream);
            var sheet = workbook.Worksheet(1);
            var rows = sheet.RangeUsed().RowsUsed().Skip(1); // skip header

            foreach (var row in rows)
            {
                var item = new T();
                var props = typeof(T).GetProperties();

                for (int i = 0; i < props.Length; i++)
                {
                    var cellValue = row.Cell(i + 1).GetString();
                    props[i].SetValue(item, cellValue);
                }

                result.Add(item);
            }

            return result;
        }
    }
}
