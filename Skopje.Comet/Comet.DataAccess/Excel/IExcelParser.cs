namespace Comet.DataAccess.Excel
{
    public interface IExcelParser
    {
        List<T> Parse<T>(Stream stream) where T : new();
    }
}
