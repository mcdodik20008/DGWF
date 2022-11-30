using DGWF.dgvf.filter;

namespace DGWF.impl;

public class FilterFactory : IFilterFactory
{
    private List<FilterModel> Collection { get; } = new List<FilterModel>();

    public TFilter Find<TFilter>() where TFilter : FilterModel, new()
    {
        if (Collection.FirstOrDefault(f => f is TFilter) is TFilter filter)
        {
            return filter;
        }
        AddNew<TFilter>();
        return Find<TFilter>();
    }

    private void AddNew<TFilter>() where TFilter : FilterModel, new()
    {
        var filter = new TFilter();
        filter.Reset();
        Collection.Add(filter);
    }
}