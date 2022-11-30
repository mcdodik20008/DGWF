using System.Configuration;
using DGWF.dgvf.sort;
using DGWF.dgvf.typeconvertors;

namespace DGWF.dgvf.filter;

public sealed class Mapper : IMapper
{
    private Dictionary<Tuple<Type, Type>, ITypeConvertor> _convertors = new();

    public TOut Map<TIn, TOut>(TIn sourse)
    {
        var key = Tuple.Create(typeof(TIn), typeof(TOut));
        if (_convertors.TryGetValue(key, out var value))
        {
            return (value as ITypeConvertor<TIn, TOut>).Convert(sourse);
        }
        throw new ConfigurationException($"CreateMap sourse:{typeof(TIn)}, destination:{typeof(TOut)}");
    }

    public void SetBasicMaps()
    {
        _convertors.Add(Tuple.Create(typeof(string), typeof(Comparators)), new StringComparatorConvertor());
        _convertors.Add(Tuple.Create(typeof(string), typeof(DateTime)), new StringDateTimeConvertor());
        _convertors.Add(Tuple.Create(typeof(string), typeof(double)), new StringDoubleConvertor());
        _convertors.Add(Tuple.Create(typeof(string), typeof(int)), new StringIntConvertor());
        _convertors.Add(Tuple.Create(typeof(string), typeof(SortDirection)), new StringSortDirectionConvertor());
        _convertors.Add(Tuple.Create(typeof(string), typeof(string)), new StringStringConvertor());
    }
    
    public void CreateMap<TIn, TOut>(ITypeConvertor<TIn, TOut> configuration)
    {
        var key = Tuple.Create(typeof(TIn), typeof(TOut));
        var value = configuration as ITypeConvertor<object, object>;
        if (_convertors.TryGetValue(key, out var xx))
        {
            _convertors[key] = value;
            return;
        }
        _convertors.Add(key, value);
    }
}