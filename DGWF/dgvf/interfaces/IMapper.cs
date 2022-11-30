namespace DGWF.dgvf;

public interface IMapper
{
    TOut Map<TIn, TOut>(TIn sourse);

    void CreateMap<TIn, TOut>(ITypeConvertor<TIn, TOut> configuration);
}