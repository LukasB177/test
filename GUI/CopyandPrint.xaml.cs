using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using MySql.Data.MySqlClient;
using OfficeOpenXml;

namespace GUI
{
    /// <summary>
    /// Interaktionslogik für CopyandPrint.xaml
    /// </summary>
    public partial class CopyandPrint : Window
    {
        public CopyandPrint()
        {
            InitializeComponent();
        }

        private void CreateBackupButton_Click(object sender, RoutedEventArgs e)
        {
            // Aufruf der Methode zum Erstellen der Excel-Datei
            SaveToExcel();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            // Implementiere die Drucklogik hier oder rufe eine Methode auf, die den Druckprozess startet
            MessageBox.Show("Drucken wurde gestartet.", "Drucken", MessageBoxButton.OK, MessageBoxImage.Information);
            //StartPrintProcess();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void SaveToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            
            string downloadsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string excelFilePath = System.IO.Path.Combine(downloadsPath, $"Sicherungskopie.xlsx");


            using (ExcelPackage package = new ExcelPackage())
            {
                // Daten aus den Tabellen abrufen und in Excel speichern
                DataTable schablonenliste = await GetDataAsync("SELECT * FROM sliste"); // Deine Tabelle "schablonenliste"
                AddWorksheet(package, "Schablonenliste", schablonenliste);

                DataTable warnungen = await GetDataAsync("SELECT * FROM warnungen"); // Deine Tabelle "warnungen"
                AddWorksheet(package, "Warnungen", warnungen);

                DataTable schablonenvorgaenge = await GetDataAsync("SELECT * FROM vorgänge"); // Deine Tabelle "schablonenvorgaenge"
                AddWorksheet(package, "Schablonenvorgänge", schablonenvorgaenge);

                DataTable lagervorgaenge = await GetDataAsync("SELECT * FROM lagerung"); // Deine Tabelle "lagervorgaenge"
                AddWorksheet(package, "Lagervorgänge", lagervorgaenge);

                DataTable schablonenpruefungen = await GetDataAsync("SELECT * FROM prüfungen"); // Deine Tabelle "schablonenpruefungen"
                AddWorksheet(package, "Schablonenprüfungen", schablonenpruefungen);

                // Datei speichern
                File.WriteAllBytes(excelFilePath, package.GetAsByteArray());
                MessageBox.Show($"Die Sicherungskopie wurde erfolgreich unter {excelFilePath} gespeichert.", "Erfolg", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async Task<DataTable> GetDataAsync(string query)
        {
            return await Task.Run(() =>
            {
                DataTable dataTable = new DataTable();

                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    adapter.Fill(dataTable);
                }

                return dataTable;
            });
        }

        private void AddWorksheet(ExcelPackage package, string sheetName, DataTable dataTable)
        {
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);

            // Header hinzufügen
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                worksheet.Cells[1, i + 1].Value = dataTable.Columns[i].ColumnName;
            }

            // Daten hinzufügen
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    var cell = worksheet.Cells[i + 2, j + 1];
                    cell.Value = dataTable.Rows[i][j];

                    // Datumsspalten formatieren
                    if (dataTable.Columns[j].DataType == typeof(DateTime))
                    {
                        cell.Style.Numberformat.Format = "dd.MM.yyyy";  // Beispiel: deutsches Datumsformat
                    }

                    // Beispiel: Zellen in der Spalte "Meldungstext" grün färben
                    if (dataTable.Columns[j].ColumnName == "Meldungstext" && dataTable.TableName == "lagerung")
                    {
                        cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                    }

                    //Hier kann ich noch paar Desings anpassen
                    //und schauen dass ich das Backup genauso designe 
                    //wie der DataGrid wenn er ausgegeben wird
                    //
                    //
                    //
                    //
                }
            }
            // Spaltenbreite automatisch anpassen
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        }
    }
}