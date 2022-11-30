namespace DGWF.dgvf;

public interface ITypeConvertor { }

public interface ITypeConvertor<in TIn, out TOut> : ITypeConvertor
{
    TOut Convert(TIn source);
}