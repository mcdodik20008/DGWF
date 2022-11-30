using System.Reflection;

namespace DGWF.dgvf;

public class FilterSorterColumn
{
    public string Name { get; }
    
    public PropertyInfo Property { get; }
    
    public Type ValueType { get; }
    
    public string Value { get; set; }
    
    public string ValueFilter{ get; set; }
    
    public string ValueSorter { get; set; }
    
    public int Order { get; set; }

    public FilterSorterColumn(PropertyInfo property, string name, Type valueType)
    {
        Property = property;
        Name = name;
        ValueType = valueType;
    }

    public void Reset()
    {
        Value = "";
        ValueFilter = "";
        ValueSorter = "";
        Order = 0;
    }
}