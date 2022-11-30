using System.Linq.Expressions;
using DGWF.dgvf.filter;
using LinqKit;

namespace DGWF.impl;

public class RegisterFilter : FilterModel<string>
{
    [SourseName("Number")] 
    public FilterField<string> NumberField { get; set; } = new("", Comparators.None);

    [SourseName("ValidDate")]
    public FilterField<DateTime> ValidDateField { get; set; } = new(DateTime.Now, Comparators.None);

    [SourseName("Year")] 
    public FilterField<int> YearField { get; set; } = new(0, Comparators.None);

    [SourseName("Price")] 
    public FilterField<double> PriceField { get; set; } = new(0, Comparators.None);

    public Expression<Func<string, bool>> GetExpression()
    {
        var predicate = PredicateBuilder.True<string>()
            .And(NumberField.GetPredicate<string>("Number"))
            .And(ValidDateField.GetPredicate<string>("ValidDate"))
            .And(YearField.GetPredicate<string>("Year"))
            .And(PriceField.GetPredicate<string>("Price"));
        return predicate;
    }

    public void Reset()
    {
        NumberField = new FilterField<string>("", Comparators.None);
        ValidDateField = new FilterField<DateTime>(DateTime.Now, Comparators.None);
        YearField = new FilterField<int>(0, Comparators.None);
        PriceField = new FilterField<double>(0, Comparators.None);
    }
}