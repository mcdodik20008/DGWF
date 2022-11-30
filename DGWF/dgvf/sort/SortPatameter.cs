using System.Reflection;

namespace DGWF.dgvf.sort;

public class SortParameter
{
    public SortParameter(PropertyInfo property, SortDirection direction = SortDirection.None)
    {
        Property = property;
        Direction = direction;
    }

    public PropertyInfo Property { get; }
    public SortDirection Direction { get; }
}