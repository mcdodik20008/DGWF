namespace DGWF.dgvf.filter;

public class SourseNameAttribute : Attribute
{
    public string Name { get; }

    public SourseNameAttribute(string name)
    {
        Name = name;
    }
}