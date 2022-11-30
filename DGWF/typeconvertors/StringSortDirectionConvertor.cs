using DGWF.dgvf.sort;

namespace DGWF.dgvf.typeconvertors;

public class StringSortDirectionConvertor : ITypeConvertor<string, SortDirection>
{
    public SortDirection Convert(string source)
    {
        return source switch
        {
            "По возрастанию" => SortDirection.Up,
            "По убыванию" => SortDirection.Down,
            _ => SortDirection.None
        };
    }
}