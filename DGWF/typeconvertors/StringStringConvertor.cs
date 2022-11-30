namespace DGWF.dgvf.typeconvertors;

public class StringStringConvertor : ITypeConvertor<string, string>
{
    public string Convert(string source)
    {
        return source;
    }
}