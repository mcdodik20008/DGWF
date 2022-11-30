using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using DGWF.dgvf.filter;
using DGWF.dgvf.sort;

namespace DGWF.dgvf;

public class DataGridViewWithFilter<TValue,TFilter> : DataGridView where TFilter : FilterModel, new()
{
    private int _sortOrder;
    private int _columnIndex;
    private readonly TFilter _filter;
    private List<TValue> _values = new();
    private readonly IMapper _mapper;
    private readonly List<FilterSorterColumn> _filterColumns = new();

    #region controls

    private readonly TextBox _textBoxCtrl = new();
    private readonly ComboBox _comboBoxSort = new();
    private readonly Button _saveFilterCtrl = new();
    private readonly Button _clearButtonCtrl = new();
    private readonly ToolStripDropDown _popup = new();
    private readonly ComboBox _comboBoxFilter = new();
    private readonly DateTimePicker _dateTimeCtrl = new();

    #endregion

    public DataGridViewWithFilter(TFilter filter, IMapper mapper)
    {
        _filter = filter;
        _mapper = mapper;
    }
    
    public DataGridViewWithFilter(IFilterFactory factory, IMapper mapper)
    {
        _filter = factory.Find<TFilter>();
        _mapper = mapper;
    }

    public void Reset()
    {
        _filter.Reset();
        _filterColumns.ForEach(x => x.Reset());
    }

    public TValue GetSelectedValue()
    {
        return _values[CurrentRow.Index];
    }
    
    public void FillDataGrid(IEnumerable<TValue> sourse, List<string>? skipPropertys = null)
    {
        _values.Clear();
        _values.AddRange(sourse);
        var dt = new DataTable();
        Columns.Clear();
        var propertys = typeof(TValue).GetProperties();

        // создаю столбцы
        foreach (var prop in propertys)
        {
            if (prop.Name.ToLower().Equals("id"))
                continue;
            dt.Columns.Add(prop.Name, prop.PropertyType);
            if (_filterColumns.Count < propertys.Length)
            {
                _filterColumns.Add(new FilterSorterColumn(prop, prop.Name, prop.PropertyType));
            }
        }

        // заполняю значениями
        foreach (var entity in sourse)
        {
            var values = GetEntityValues(entity, propertys, skipPropertys);
            dt.Rows.Add(values);
        }

        DataSource = dt;
    }

    private object[] GetEntityValues<T>(T entity, PropertyInfo[] propertys, List<string>? skipPropertys)
    {
        var values = new List<object>();
        foreach (var prop in propertys)
        {
            if (skipPropertys is not null && skipPropertys.Contains(prop.Name))
                continue;
            values.Add(prop.GetValue(entity));
        }

        return values.ToArray();
    }

    public List<SortParameter> GetSortParameters<T>()
    {
        var parameters = new List<SortParameter>();
        foreach (var column in _filterColumns.OrderBy(x => x.Order))
        {
            var prop = typeof(T).GetProperty(column.Property.Name);
            var val = _mapper.Map<string, SortDirection>(column.ValueSorter);
            parameters.Add(new SortParameter(prop, val));
        }
        return parameters;
    }
    
    public Expression<Func<TObject, bool>> GetFilter<TObject>()
    {
        var filter = _filter as FilterModel<TObject>;
        filter = FillFilter(filter, _filterColumns);
        return filter.GetExpression();
    }
    
    private FilterModel<T> FillFilter<T>(FilterModel<T> filter, List<FilterSorterColumn> filterColumns)
    {
        var properties = filter.GetType().GetProperties();
        foreach (var property in properties)
        {
            var value = property.GetValue(filter);
            MethodInfo updateFilterFieldMethod = value.GetType().GetMethod("UpdateFilter");
            var popName = property.GetCustomAttribute<SourseNameAttribute>()?.Name;
            var filterColumn = filterColumns.FirstOrDefault(x => x.Name.Equals(popName));
            var parameters = Equals(filterColumn, null) || Equals(filterColumn.Value, "")
                ? new object[] { _mapper, "", "" }
                : new object[] { _mapper, filterColumn.Value, filterColumn.ValueFilter };
            updateFilterFieldMethod.Invoke(value, parameters);
        }

        return filter;
    }

    // Устанавливаем доступные функции филтрации в зависимости от типа столбца
    private void FillFilterCombobox(ComboBox comboBox, string columnType)
    {
        var values = columnType switch
        {
            "System.DateTime" => new[] { "Без фильтрации", "До", "После" },
            "System.Int32" or "System.Int64" or "System.Double"
                => new[] { "Без фильтрации", "Меньше", "Больше", "Равно" },
            _ => new[]{ "Содержит" }
        };
        var val = _filterColumns[_columnIndex].ValueFilter;
        comboBox.Text = val is null || val.Equals("") ? values[0] : val;
        comboBox.Items.Clear();
        comboBox.Items.AddRange(values);
    }

    // Устанавливаем доступные функции сортировки в зависимости от типа столбца
    private void FillSortCombobox(ComboBox comboBox)
    {
        var values = new[] { "Без соритровки", "По возрастанию", "По убыванию" };
        var val = _filterColumns[_columnIndex].ValueSorter;
        comboBox.Text = val is null || val.Equals("") ? values[0] : val;
        comboBox.Items.Clear();
        comboBox.Items.AddRange(values);
    }
    
    private void Header_FilterButtonClicked(object sender, ColumnFilterClickedEventArg e)
    {
        _columnIndex = e.ColumnIndex;
        InitializeCtrls(_columnIndex);
        var valueTextBox = GetControlHost(_textBoxCtrl);
        var actionBox = GetControlHost(_comboBoxFilter);
        var saveButton = GetControlHost(_saveFilterCtrl);
        var clearButton = GetControlHost(_clearButtonCtrl);
        var dateTimePicker = GetControlHost(_dateTimeCtrl);
        var sortBox = GetControlHost(_comboBoxSort);
        _popup.Items.Clear();
        _popup.AutoSize = true;
        _popup.Margin = Padding.Empty;
        _popup.Padding = Padding.Empty;
        var colType = Columns[_columnIndex].ValueType.ToString();
        FillFilterCombobox(_comboBoxFilter, colType);
        FillSortCombobox(_comboBoxSort);
        switch (colType)
        {
            case "System.DateTime":
                _popup.Items.Add(actionBox);
                _popup.Items.Add(dateTimePicker);
                break;
            case "System.Int64":
            case "System.Int32":
            case "System.Double":
                _popup.Items.Add(actionBox);
                _popup.Items.Add(valueTextBox);
                break;
            case "System.String":
                _popup.Items.Add(actionBox);
                _popup.Items.Add(valueTextBox);
                break;
            default:
                _popup.Items.Add(valueTextBox);
                break;
        }

        _popup.Items.Add(sortBox);
        _popup.Items.Add(saveButton);
        _popup.Items.Add(clearButton);
        _popup.Show(this, e.ButtonRectangle.X, e.ButtonRectangle.Bottom);
    }
    
    private void InitializeCtrls(int colIndex)
    {
        var widthTool = Columns[colIndex].Width + 50;
        if (widthTool < 130) widthTool = 130;
        
        _comboBoxSort.Size = new Size(widthTool, 30);
        _comboBoxFilter.Size = new Size(widthTool, 30);

        // Старое значение фильтрации для выбранного столбца
        _textBoxCtrl.Text = _filterColumns[_columnIndex].Value;
        _textBoxCtrl.Size = new Size(widthTool, 30);

        _dateTimeCtrl.Size = new Size(widthTool, 30);
        _dateTimeCtrl.Format = DateTimePickerFormat.Custom;
        _dateTimeCtrl.CustomFormat = "dd.MM.yyyy";
        _dateTimeCtrl.TextChanged -= DatePicker_TextChanged!;
        _dateTimeCtrl.TextChanged += DatePicker_TextChanged!;

        _saveFilterCtrl.Text = "Save";
        _saveFilterCtrl.Size = new Size(widthTool, 30);
        _saveFilterCtrl.Click -= SaveFilterSorter_Click!;
        _saveFilterCtrl.Click += SaveFilterSorter_Click!;

        _clearButtonCtrl.Text = "Clear";
        _clearButtonCtrl.Size = new Size(widthTool, 30);
        _clearButtonCtrl.Click -= ClearFilter_Click!;
        _clearButtonCtrl.Click += ClearFilter_Click!;
    }

    private void ClearFilter_Click(object? sender, EventArgs e)
    {
        _textBoxCtrl.Text = "";
        _comboBoxFilter.Text = "Без фильтрации";
        _comboBoxSort.Text = "Без соритровки";
        SaveFilterSorter_Click(sender, e);
    }

    private void DatePicker_TextChanged(object sender, EventArgs e)
    {
        _textBoxCtrl.Text = _dateTimeCtrl.Text;
        SaveFilterSorter_Click(sender, e);
    }

    private void SaveFilterSorter_Click(object sender, EventArgs e)
    {
        _filterColumns[_columnIndex].Value = _textBoxCtrl.Text;
        _filterColumns[_columnIndex].Order = _sortOrder++;
        _filterColumns[_columnIndex].ValueFilter = _comboBoxFilter.Text;
        _filterColumns[_columnIndex].ValueSorter = _comboBoxSort.Text;
        _popup.Close();
    }
    
    protected override void OnColumnAdded(DataGridViewColumnEventArgs e)
    {
        var header = new DataGridFilterHeader();
        header.FilterButtonClicked += Header_FilterButtonClicked!;
        e.Column.HeaderCell = header;
        e.Column.SortMode = DataGridViewColumnSortMode.NotSortable;
        base.OnColumnAdded(e);
    }
    
    private ToolStripControlHost GetControlHost(Control control)
    {
        var host = new ToolStripControlHost(control);
        host.Margin = Padding.Empty;
        host.Padding = Padding.Empty;
        host.AutoSize = false;
        host.Size = _dateTimeCtrl.Size;
        return host;
    }
}