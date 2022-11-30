namespace DGWF.dgvf.sort;

public class UltimateComparer<T> : IComparer<T>
{
    private IEnumerable<SortParameter> _sortParameters;

    public UltimateComparer(IEnumerable<SortParameter> sortParameters)
    {
        _sortParameters = sortParameters;
    }

    public int Compare(T x, T y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;

        foreach (var sortParameter in _sortParameters)
        {
            if (sortParameter.Direction.Equals(SortDirection.None))
                continue;
            var property = sortParameter.Property;
            try
            {
                var valueX = property.GetValue(x) as IComparable;
                var valueY = property.GetValue(y) as IComparable;

                var result = valueX.CompareTo(valueY);
                if (result != 0)
                    return sortParameter.Direction.Equals(SortDirection.Up) ? result : -result;
            }
            catch (Exception e)
            {
                throw new TypeAccessException(
                    $"Недопустимое свойство для сортировки {property.Name}, Объект соритовки {typeof(T)}");
            }
        }

        return 0;
    }
}