namespace DGWF.dgvf.typeconvertors;

public class StringIntConvertor : ITypeConvertor<string, int>
{
    public int Convert(string source)
    {
        return int.TryParse(source, out var xx) ? xx : 0;
    }
}