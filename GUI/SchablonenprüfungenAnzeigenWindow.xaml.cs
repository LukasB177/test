using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GUI
{
    /// <summary>
    /// Interaktionslogik für SchablonenprüfungenAnzeigenWindow.xaml
    /// </summary>
    public partial class SchablonenprüfungenAnzeigenWindow : Window
    {
        public SchablonenprüfungenAnzeigenWindow()
        {
            InitializeComponent();

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();

                    string query = "SELECT Datum, `CK-Nummer`, Bearbeiter FROM prüfungen ORDER BY Datum DESC LIMIT 100000; ";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Format the Datum column to display in 24-hour format
                    foreach (DataRow row in dataTable.Rows)
                    {
                        if (row["Datum"] != DBNull.Value)
                        {
                            DateTime datum = (DateTime)row["Datum"];
                            row["Datum"] = datum.ToString("yyyy-MM-dd HH:mm:ss");
                        }
                    }

                    VorgangeDataGrid.ItemsSource = dataTable.DefaultView;

                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Laden der Daten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                double zoomFactor = 0.1;

                if (e.Delta > 0)
                {
                    windowScale.ScaleX += zoomFactor;
                    windowScale.ScaleY += zoomFactor;
                }
                else if (e.Delta < 0)
                {
                    windowScale.ScaleX = Math.Max(0.1, windowScale.ScaleX - zoomFactor);
                    windowScale.ScaleY = Math.Max(0.1, windowScale.ScaleY - zoomFactor);
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
