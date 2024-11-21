//using MySql.Data.MySqlClient;
//using System;
//using System.Data;
//using System.Windows;
//using System.Windows.Input;
//using System.Windows.Media;

//namespace GUI
//{
//    public partial class lagervorgängeAnzeigenWindow : Window
//    {
//        public lagervorgängeAnzeigenWindow()
//        {



//            InitializeComponent();

//            try
//            {
//                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
//                {
//                    conn.Open();

//                    string query = "SELECT datum, Lagerplatz, Typ, num1, num2, num3, Art, `TOP/BOT` AS TOPBOT, `CK#`, Benutzer, Meldungstext FROM lagerung ORDER BY datum DESC LIMIT 100000;";
//                    MySqlCommand cmd = new MySqlCommand(query, conn);
//                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
//                    DataTable dataTable = new DataTable();
//                    adapter.Fill(dataTable);

//                    LagerungDataGrid.ItemsSource = dataTable.DefaultView;

//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show($"Fehler beim Laden der Daten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
//            }
//        }






//        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
//        {
//            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
//            {
//                double zoomFactor = 0.1;

//                if (e.Delta > 0)
//                {
//                    gridScale.ScaleX += zoomFactor;
//                    gridScale.ScaleY += zoomFactor;
//                }
//                else if (e.Delta < 0)
//                {
//                    gridScale.ScaleX = Math.Max(0.1, gridScale.ScaleX - zoomFactor);
//                    gridScale.ScaleY = Math.Max(0.1, gridScale.ScaleY - zoomFactor);
//                }

//                e.Handled = true;
//            }
//        }

//        private void Button_Click(object sender, RoutedEventArgs e)
//        {
//            this.Close();
//        }
//    }
//}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace GUI
{
    public partial class lagervorgängeAnzeigenWindow : Window
    {
        public lagervorgängeAnzeigenWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT datum, Lagerplatz, Typ, Schablonennummer, `TOPBOT`, `CK`, Benutzer, Meldungstext " +
                                   "FROM lagerung ORDER BY Schablonennummer ASC, datum ASC;";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Neue Spalten hinzufügen
                    if(!dataTable.Columns.Contains("ZuletztBenutztVor"))
                    {
                        dataTable.Columns.Add("ZuletztBenutztVor", typeof(int));
                    }

                    

                    Dictionary<string, DataRow> letzteEinträge = new Dictionary<string, DataRow>();

                    // Schleife über alle Einträge, um den letzten Eintrag für jede Schablonennummer zu finden
                    foreach (DataRow row in dataTable.Rows)
                    {
                        string schablonennummer = row["Schablonennummer"].ToString();

                        // Speichere den aktuellsten Eintrag für jede Schablonennummer
                        letzteEinträge[schablonennummer] = row;
                    }

                    // Schleife über die gespeicherten letzten Einträge, um die Differenz in Tagen zu berechnen
                    foreach (var entry in letzteEinträge)
                    {
                        DataRow row = entry.Value;
                        DateTime datum = Convert.ToDateTime(row["datum"]).Date; // Nur das Datum
                        int daysSinceLastUse = (DateTime.Today - datum).Days;
                        row["ZuletztBenutztVor"] = daysSinceLastUse; // Schreibe nur bei den letzten Einträgen den Wert
                    }

                    RecalculateZuletztBenutztVor(dataTable);

                    LagerungDataGrid.ItemsSource = dataTable.DefaultView;
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

        private void OpenDateSelectionWindow_Click(object sender, RoutedEventArgs e)
        {
            DateSelectionWindow dateSelectionWindow = new DateSelectionWindow();
            if (dateSelectionWindow.ShowDialog() == true)
            {
                DateTime? selectedDate = dateSelectionWindow.SelectedDate;
                if (selectedDate.HasValue)
                {
                    FilterDataByDate(selectedDate.Value);
                }
            }
        }

        private void FilterDataByDate(DateTime? startDate)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();
                    string query = "SELECT datum, Lagerplatz, Typ, Schablonennummer, `TOPBOT` , `CK`, Benutzer, Meldungstext " +
                                   "FROM lagerung ";

                    if (startDate.HasValue)
                    {
                        query += "WHERE datum >= @StartDate ";
                    }

                    query += "ORDER BY datum DESC LIMIT 100000;";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    if (startDate.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
                    }

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Hier sicherstellen, dass die Spalte hinzugefügt und berechnet wird.
                    if (!dataTable.Columns.Contains("ZuletztBenutztVor"))
                    {
                        dataTable.Columns.Add("ZuletztBenutztVor", typeof(int));
                    }

                    // Recalculate ZuletztBenutztVor nach dem Filtern der Daten
                    RecalculateZuletztBenutztVor(dataTable);

                    LagerungDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Filtern der Daten: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void FilterSchablonenButton_Click(object sender, RoutedEventArgs e)
        {
            DataView dataView = LagerungDataGrid.ItemsSource as DataView;

            if (dataView != null)
            {
                // Zuerst sicherstellen, dass die Spalte 'ZuletztBenutztVor' existiert und korrekt berechnet ist.
                RecalculateZuletztBenutztVor(dataView.Table);

                // Filter anwenden, nur die Zeilen anzeigen, bei denen ZuletztBenutztVor > 365 ist
                dataView.RowFilter = "ZuletztBenutztVor > 365";

                // Anzahl der Zeilen zählen
                int count = dataView.Count;

                // MessageBox anzeigen
                MessageBox.Show($"Schablonen die länger als 1 Jahr nicht genutzt wurden: {count}", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RecalculateZuletztBenutztVor (DataTable dataTable)
        {
            // Dictionary, um das letzte Datum für jede Schablonennummer zu speichern
            Dictionary<string, DateTime> letzteBenutzung = new Dictionary<string, DateTime>();

            // Iteriere durch die Zeilen, um den letzten Eintrag für jede Schablonennummer zu finden
            foreach (DataRow row in dataTable.Rows)
            {
                string schablonennummer = row["Schablonennummer"].ToString();
                DateTime datum = Convert.ToDateTime(row["datum"]);

                // Wenn die Schablonennummer bereits existiert, aktualisieren wir das Datum der letzten Benutzung, ansonsten fügen wir sie hinzu.
                if (letzteBenutzung.ContainsKey(schablonennummer))
                {
                    // Aktualisiere nur, wenn das Datum neuer ist als das bereits gespeicherte
                    if (datum > letzteBenutzung[schablonennummer])
                    {
                        letzteBenutzung[schablonennummer] = datum;
                    }
                }
                else
                {
                    letzteBenutzung[schablonennummer] = datum;
                }
            }

            // Setze die "ZuletztBenutztVor"-Spalte nur für den letzten Eintrag jeder Schablonennummer
            foreach (DataRow row in dataTable.Rows)
            {
                string schablonennummer = row["Schablonennummer"].ToString();
                DateTime datum = Convert.ToDateTime(row["datum"]);

                if (letzteBenutzung.ContainsKey(schablonennummer) && datum == letzteBenutzung[schablonennummer])
                {
                    // Berechne die Differenz in Tagen vom heutigen Datum
                    int daysSinceLastUse = (DateTime.Today - datum).Days;
                    row["ZuletztBenutztVor"] = daysSinceLastUse;
                }
            }
        }
    }
}

