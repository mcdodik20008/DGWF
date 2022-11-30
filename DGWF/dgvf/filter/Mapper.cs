using System.Configuration;

namespace DGWF.dgvf.filter;

public sealed class Mapper : IMapper
{
    private Dictionary<Tuple<Type, Type>, ITypeConvertor<object, object>> _convertors;

    public TOut Map<TIn, TOut>(TIn sourse)
    {
        var key = Tuple.Create(typeof(TIn), typeof(TOut));
        if (_convertors.TryGetValue(key, out var value))
        {
            return (value as ITypeConvertor<TIn, TOut>).Convert(sourse);
        }
        throw new ConfigurationException($"CreateMap sourse:{typeof(TIn)}, destination:{typeof(TOut)}");
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