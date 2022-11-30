using DGWF.dgvf.filter;

namespace DGWF.dgvf.typeconvertors;

public class StringComparatorConvertor : ITypeConvertor<string, Comparators>
{
    public Comparators Convert(string source)
    {
        return source switch
        {
            "Равно" => Comparators.Equals,
            "Меньше" or "До" => Comparators.Less,
            "Больше" or "После" => Comparators.More,
            "Содержит" => Comparators.Like,
            _ => Comparators.None
        };
    }
}