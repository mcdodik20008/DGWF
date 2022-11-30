namespace DGWF.dgvf
{
    public class ColumnFilterClickedEventArg : EventArgs
    {
        public int ColumnIndex { get; }
        
        public Rectangle ButtonRectangle { get; }
        
        public ColumnFilterClickedEventArg(int colIndex, Rectangle btnRect)
        {
            ColumnIndex = colIndex;
            ButtonRectangle = btnRect;
        }
    }
}
