using System.Linq.Expressions;

namespace DGWF.dgvf.filter;

public interface FilterModel
{
    void Reset();
}

public interface FilterModel<T> : FilterModel
{
    Expression<Func<T, bool>> GetExpression();
}