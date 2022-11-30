namespace DGWF.dgvf.typeconvertors;

public class StringDateTimeConvertor : ITypeConvertor<string, DateTime>
{
    public DateTime Convert(string source)
    {
        return DateTime.TryParse(source, out var xx) ? xx : DateTime.Now;
    }
}