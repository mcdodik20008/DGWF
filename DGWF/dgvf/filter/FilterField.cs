using System.Linq.Expressions;
using LinqKit;

namespace DGWF.dgvf.filter;

public class FilterField<T> where T : IComparable
{
    public T Value;

    public Comparators Action;

    public FilterField(T value, Comparators action)
    {
        Value = value;
        Action = action;
    }

    public FilterField<T> UpdateFilter(IMapper mapper, string value, string action)
    {
        Action = mapper.Map<string, Comparators>(action);
        Value = mapper.Map<string, T>(value);
        return this;
    }

    public Expression<Func<TEnt, bool>> GetPredicate<TEnt>(string propName)
    {
        var predicate = PredicateBuilder.True<TEnt>();
        var type = typeof(TEnt).GetProperty(propName);
        if (Comparators.None != Action)
        {
            predicate = Action switch
            {
                Comparators.Equals => predicate.And(l => type.GetValue(l).Equals(Value)),
                Comparators.Less => predicate.And(l => Value.CompareTo(type.GetValue(l) as IComparable) == 1),
                Comparators.More => predicate.And(l => Value.CompareTo(type.GetValue(l) as IComparable) == -1),
                Comparators.Like => predicate.And(l => type.GetValue(l).ToString().Contains(Value.ToString()))
            };
        }

        return predicate;
    }
}