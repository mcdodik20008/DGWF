namespace DGWF.dgvf.filter;

public interface IFilterFactory
{
    T Find<T>() where T : FilterModel, new();
}