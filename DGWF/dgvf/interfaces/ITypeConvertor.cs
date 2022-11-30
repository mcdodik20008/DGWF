namespace DGWF.dgvf;

public interface ITypeConvertor<in TIn, out TOut>
{
    TOut Convert(TIn source);
}