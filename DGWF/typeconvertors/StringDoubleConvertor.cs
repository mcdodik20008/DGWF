namespace DGWF.dgvf.typeconvertors;

public class StringDoubleConvertor : ITypeConvertor<string, double>
{
    public double Convert(string source)
    {
        return double.TryParse(source, out var xx) ? xx : 0;
    }
}