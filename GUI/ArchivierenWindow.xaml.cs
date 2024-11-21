using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
    /// Interaktionslogik für ArchivierenWindow.xaml
    /// </summary>
    public partial class ArchivierenWindow : Window
    {
        public ArchivierenWindow()
        {
            InitializeComponent();
        }

        private void ButtonBeenden_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void ButtonSchabloneArchivieren_Click(object sender, RoutedEventArgs e)
        {
            string gitterbox = "Gitterbox";
            string ckNummer = CKNummerTextBox.Text;
            string benutzer = Environment.UserName;
            DateTime datum = DateTime.Now;

            if (string.IsNullOrWhiteSpace(ckNummer))
            {
                MessageBox.Show("Bitte CK-Nummer eingeben!");
                CKNummerTextBox.Focus();
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();

                    string query = $"SELECT * FROM sliste WHERE CK = @ckNummer";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ckNummer", ckNummer);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.HasRows)
                            {
                                MessageBox.Show($"Die CK-Nummer {ckNummer} wurde auf keinem Lagerort gefunden!", "Fehler", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                                return;
                            }

                            while (reader.Read())
                            {
                                string typSchab = reader["Typ"].ToString();
                                string lager = reader["Lagerplatz"].ToString();
                                string schablonennummer = reader["Schablonennummer"].ToString();
                                string seite = reader["TOPBOTTOM"].ToString();
                                string nutzengroesse = reader["Nutzengröße"].ToString();
                                string maxZyklen = reader["maxZyklen"].ToString();
                                string aktZyklen = reader["aktZyklen"].ToString();
                                string warnungZyklen = reader["WarnungZyklen"].ToString();
                                                                

                                // Überprüfen des Typs
                                if ((TypRadioButton601.IsChecked == true && typSchab != "601-") ||
                                    (TypRadioButtonVS.IsChecked == true && typSchab != "VS-"))
                                {
                                    MessageBox.Show($"Fehler: Falscher Schablonentyp ausgewählt. Die Schablone {schablonennummer} ist vom Typ {typSchab}.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                                // Überprüfen der Seite
                                if ((SeiteRadioButtonTOP.IsChecked == true && seite != "TOP") ||
                                    (SeiteRadioButtonBOTTOM.IsChecked == true && seite != "BOTTOM"))
                                {
                                    MessageBox.Show($"Fehler: Falsche Seite ausgewählt. Die Schablone {schablonennummer} befindet sich auf der Seite {seite}.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                                // Schließen des Readers nach dem Lesen der Daten
                                reader.Close();

                                // Neuen Eintrag mit Lagerplatz 0 erstellen
                                string insertNewEntryQuery = @"INSERT INTO sliste (Lagerplatz, Typ, Schablonennummer, `TOPBOTTOM`, Nutzengröße, CK, maxZyklen, aktZyklen, WarnungZyklen) 
                                                       VALUES (@Lagerplatz, @typ, @schablonennummer, @seite, @nutzengroesse, @ckNummer, @maxZyklen, @aktZyklen, @warnungZyklen)";
                                using (MySqlCommand insertCmd = new MySqlCommand(insertNewEntryQuery, conn))
                                {
                                    insertCmd.Parameters.AddWithValue("@Lagerplatz", gitterbox);
                                    insertCmd.Parameters.AddWithValue("@typ", typSchab);
                                    insertCmd.Parameters.AddWithValue("@schablonennummer", schablonennummer);
                                    insertCmd.Parameters.AddWithValue("@seite", seite);
                                    insertCmd.Parameters.AddWithValue("@nutzengroesse", nutzengroesse);
                                    insertCmd.Parameters.AddWithValue("@ckNummer", ckNummer);
                                    insertCmd.Parameters.AddWithValue("@maxZyklen", maxZyklen);
                                    insertCmd.Parameters.AddWithValue("@aktZyklen", aktZyklen);
                                    insertCmd.Parameters.AddWithValue("@warnungZyklen", warnungZyklen);
                                    

                                    insertCmd.ExecuteNonQuery();
                                }

                                // Alten Eintrag in der Schablonenliste leeren
                                string updateOldEntryQuery = @"UPDATE sliste SET 
                                                        Typ = NULL, 
                                                        Schablonennummer = NULL,
                                                        `TOPBOTTOM` = NULL,
                                                        Nutzengröße = NULL,
                                                        CK = NULL,
                                                        maxZyklen = NULL,
                                                        aktZyklen = NULL,
                                                        WarnungZyklen = NULL,
                                                        ausgelagert = 'Gitterbox'
                                                      WHERE CK = @ckNummer AND Lagerplatz = @lagerplatz";
                                using (MySqlCommand updateCmd = new MySqlCommand(updateOldEntryQuery, conn))
                                {
                                    updateCmd.Parameters.AddWithValue("@ckNummer", ckNummer);
                                    updateCmd.Parameters.AddWithValue("@lagerplatz", lager);

                                    updateCmd.ExecuteNonQuery();
                                }

                                // Eintrag in die 'lagerung'-Tabelle für Archivierung
                                string insertLagerungQuery = "INSERT INTO lagerung (datum, Lagerplatz, Typ, Schablonennummer, `TOP/BOT`, `CK#`, Benutzer, Meldungstext) VALUES " +
                                                             "(@datum, @lagerplatz, @typ, @Schablonennummer, @seite, @ckNummer, @benutzer, @vorgang)";
                                using (MySqlCommand insertLagerungCmd = new MySqlCommand(insertLagerungQuery, conn))
                                {
                                    string datumFormatted = datum.ToString("yyyy-MM-dd HH:mm:ss");
                                    insertLagerungCmd.Parameters.AddWithValue("@datum", datumFormatted);
                                    insertLagerungCmd.Parameters.AddWithValue("@lagerplatz", lager); // Neuer Lagerplatz 0
                                    insertLagerungCmd.Parameters.AddWithValue("@typ", typSchab);
                                    insertLagerungCmd.Parameters.AddWithValue("@Schablonennummer", schablonennummer);
                                    insertLagerungCmd.Parameters.AddWithValue("@seite", seite);
                                    insertLagerungCmd.Parameters.AddWithValue("@ckNummer", ckNummer);
                                    insertLagerungCmd.Parameters.AddWithValue("@benutzer", benutzer);
                                    insertLagerungCmd.Parameters.AddWithValue("@vorgang", "VORGANG: Schablone archiviert");
                                    
                                    insertLagerungCmd.ExecuteNonQuery();
                                }

                                MessageBox.Show($"Die Schablone mit der CK-Nummer {ckNummer} wurde erfolgreich archiviert. Lagerplatz wurde auf 'Gitterbox' gesetzt.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                CKNummerTextBox.Clear();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Verbindung zur Datenbank: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
