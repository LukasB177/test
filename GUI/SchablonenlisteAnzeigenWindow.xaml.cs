using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MySql.Data.MySqlClient;

namespace GUI
{
    public partial class SchablonenlisteAnzeigenWindow : Window
    {
        private DataTable _dataTable;

        public SchablonenlisteAnzeigenWindow()
        {
            InitializeComponent();
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                _dataTable = await GetDataAsync();
                SchablonenlisteDataGrid.ItemsSource = _dataTable.DefaultView;
                
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    MessageBox.Show($"Fehler beim Laden der Daten: {innerEx.Message}\n{innerEx.StackTrace}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Daten: {ex.Message}\n{ex.StackTrace}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private Task<DataTable> GetDataAsync()
        {
            return Task.Run(() =>
            {
                DataTable dataTable = new DataTable();

                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT * FROM sliste LIMIT 1000";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                    

                }

                return dataTable;
            });
        }

        //private void CreateColumnsWithContextMenu()
        //{
        //    foreach (DataColumn column in _dataTable.Columns)
        //    {

        //        MessageBox.Show(column.ColumnName);

        //        var dataGridColumn = new DataGridTextColumn
        //        {
        //            Header = column.ColumnName,
        //            Binding = new System.Windows.Data.Binding(column.ColumnName)
        //        };

              
        //        var contextMenu = new ContextMenu();
        //        var uniqueValues = _dataTable.AsEnumerable()
        //                                     .Select(row => row[column].ToString())
        //                                     .Distinct()
        //                                     .OrderBy(val => val);

        //        foreach (var value in uniqueValues)
        //        {
        //            var menuItem = new MenuItem { Header = value };
        //            menuItem.Click += (s, e) => ApplyFilter(column.ColumnName, value);
        //            contextMenu.Items.Add(menuItem);
        //        }

        //        dataGridColumn.HeaderStyle = new Style(typeof(DataGridColumnHeader))
        //        {
        //            Setters = { new Setter(DataGridColumnHeader.ContextMenuProperty, contextMenu) }
        //        };

        //        SchablonenlisteDataGrid.Columns.Add(dataGridColumn);
        //    }
        //}

        private void ApplyFilter(string columnName, string filterValue)
        {
            DataView dv = new DataView(_dataTable);
            dv.RowFilter = $"[{columnName}] = '{filterValue}'";
            SchablonenlisteDataGrid.ItemsSource = dv;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (_dataTable == null)
            {
                MessageBox.Show("Daten sind noch nicht geladen. Bitte warten Sie einen Moment und versuchen Sie es erneut.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            string searchText = SearchTextBox.Text;
            if (string.IsNullOrEmpty(searchText))
            {
                SchablonenlisteDataGrid.ItemsSource = _dataTable.DefaultView;
                return;
            }

            DataView dv = new DataView(_dataTable);
            string filter = string.Join(" OR ", _dataTable.Columns.Cast<DataColumn>().Select(col =>
            {
                if (col.DataType == typeof(string))
                {
                    return $"[{col.ColumnName}] LIKE '%{searchText}%'";
                }
                else if (col.DataType == typeof(int) || col.DataType == typeof(decimal) || col.DataType == typeof(double))
                {
                    if (int.TryParse(searchText, out _))
                    {
                        return $"[{col.ColumnName}] = {searchText}";
                    }
                    return "1=0"; // Ungültige Zahlensuche, also keinen Treffer
                }
                return "1=0"; // Für andere Typen nicht suchen
            }));

            dv.RowFilter = filter;
            SchablonenlisteDataGrid.ItemsSource = dv;
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double zoomFactor = 0.1;

                if (e.Delta > 0)
                {
                    gridScale.ScaleX += zoomFactor;
                    gridScale.ScaleY += zoomFactor;
                }
                else if (e.Delta < 0)
                {
                    gridScale.ScaleX = Math.Max(0.1, gridScale.ScaleX - zoomFactor);
                    gridScale.ScaleY = Math.Max(0.1, gridScale.ScaleY - zoomFactor);
                }

                e.Handled = true;
            }
        }

        


        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
