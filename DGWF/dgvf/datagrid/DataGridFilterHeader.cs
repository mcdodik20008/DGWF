using System.Windows.Forms.VisualStyles;

namespace DGWF.dgvf
{
    public class DataGridFilterHeader : DataGridViewColumnHeaderCell
    {
        private ComboBoxState _currentState = ComboBoxState.Normal;

        private Point _cellLocation;

        private Rectangle _buttonRect;

        public event EventHandler<ColumnFilterClickedEventArg> FilterButtonClicked;

        protected override void OnDataGridViewChanged()
        {
            try
            {
                Padding dropDownPadding = new Padding(0, 0, 20, 0);
                Style.Padding = Padding.Add(InheritedStyle.Padding, dropDownPadding);
            }
            catch { }
            base.OnDataGridViewChanged();
        }

        protected override void Paint(Graphics graphics,
                                      Rectangle clipBounds,
                                      Rectangle cellBounds,
                                      int rowIndex,
                                      DataGridViewElementStates dataGridViewElementState,
                                      object value,
                                      object formattedValue,
                                      string errorText,
                                      DataGridViewCellStyle cellStyle,
                                      DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {

            base.Paint(graphics, clipBounds,
                       cellBounds, rowIndex,
                       dataGridViewElementState, value,
                       formattedValue, errorText,
                       cellStyle, advancedBorderStyle, paintParts);

            int width = 20; // 20 px
            _buttonRect = new Rectangle(cellBounds.X + cellBounds.Width - width, cellBounds.Y, width, cellBounds.Height);
            _cellLocation = cellBounds.Location;
            ComboBoxRenderer.DrawDropDownButton(graphics, _buttonRect, _currentState);
        }
        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            if (IsMouseOverButton(e.Location))
                _currentState = ComboBoxState.Pressed;
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(DataGridViewCellMouseEventArgs e)
        {
            if (IsMouseOverButton(e.Location))
            {
                _currentState = ComboBoxState.Normal;
                OnFilterButtonClicked();
            }
            base.OnMouseUp(e);
        }
        
        private bool IsMouseOverButton(Point e)
        {
            Point p = new Point(e.X + _cellLocation.X, e.Y + _cellLocation.Y);
            if (p.X >= _buttonRect.X && p.X <= _buttonRect.X + _buttonRect.Width &&
                p.Y >= _buttonRect.Y && p.Y <= _buttonRect.Y + _buttonRect.Height)
            {
                return true;
            }
            return false;
        }
        protected virtual void OnFilterButtonClicked()
        {
            if (FilterButtonClicked != null)
            {
                FilterButtonClicked(this, new ColumnFilterClickedEventArg(ColumnIndex, _buttonRect));
            }
        }
    }
}