using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace GUI
{
    public partial class cleanWindow : Window
    {
        private string bearbeiter;

        public cleanWindow()
        {
            InitializeComponent();
        }

        private void btn_Check_No_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ShowMessageWindow("Ungeprüfte Schablonen dürfen nicht eingelagert werden!");
        }

        private void btn_check_Yes_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            ShowCKInputWindow();
        }

        private void ShowMessageWindow(string message)
        {
            Window messageWindow = new Window
            {
                Title = "Warnung",
                Width = 400,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };

            StackPanel stackpanel = new StackPanel();

            TextBlock textBlock = new TextBlock
            {
                Text = message,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(10)
            };

            Button closeButton = new Button
            {
                Content = "Schließen",
                Width = 75,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10),
            };
            closeButton.Click += (s, e) => messageWindow.Close();

            stackpanel.Children.Add(textBlock);
            stackpanel.Children.Add(closeButton);

            messageWindow.Content = stackpanel;
            messageWindow.ShowDialog();
        }

        private void ShowCKInputWindow()
        {
            Window ckInputWindow = new Window
            {
                Title = "CK-Nummer eingeben",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };

            StackPanel stackPanel = new StackPanel();

            TextBox ckInputBox = new TextBox
            {
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center,
                Width = 200
            };

            Button submitButton = new Button
            {
                Content = "Bestätigen",
                Width = 75,
                Height = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10),
            };
            submitButton.Click += (s, e) =>
            {
                string ckNr = ckInputBox.Text;

                if (string.IsNullOrEmpty(ckNr))
                {
                    MessageBox.Show("Vorgang abgebrochen!", "Warnung", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (ckNr.Any(char.IsLetter)) //Überprüft auf Buchstaben in der CK-Nummer
                {
                    MessageBox.Show("Eingabe ungültig! CK-Nummer darf keine Buchstaben enthalten.", "Warnung", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (!CKnummerVorhanden(ckNr))
                {
                    MessageBox.Show("CK-Nummer nicht vorhanden!", "Warnung", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    MessageBox.Show("CK-Nummer vorhanden. Prozess wird fortgesetzt.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    ckInputWindow.Close();
                    Auftragsmenge(ckNr); // Hier wird die CK-Nummer weitergegeben
                }
            };

            stackPanel.Children.Add(new TextBlock
            {
                Text = "Bitte CK-Nummer eingeben:",
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10)
            });
            stackPanel.Children.Add(ckInputBox);
            stackPanel.Children.Add(submitButton);

            ckInputWindow.Content = stackPanel;
            ckInputWindow.ShowDialog();
        } //Auftragsmenge wird abgefragt, wenn CK-Nummer vorhanden

        private bool CKnummerVorhanden(string ckNr)
        {
            bool vorhanden = false;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM sliste WHERE `CK` = @ckNr";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ckNr", ckNr);
                    if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                    {
                        vorhanden = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }

            return vorhanden;
        }





        private void UpdateAktZyklen(string ckNr, int auftragsmenge)
        {
            DateTime datum = DateTime.Now; // Datum ohne Formatierung
            string benutzer = Environment.UserName;

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();

                    // Aktuelle aktZyklen, Nutzengröße, maxZyklen und warnZyklen abrufen
                    string selectQuery = "SELECT * FROM sliste WHERE `CK` = @ckNr";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, conn);
                    selectCmd.Parameters.AddWithValue("@ckNr", ckNr);

                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int aktZyklen = Convert.ToInt32(reader["aktZyklen"]);
                            int nutzengroesse = Convert.ToInt32(reader["Nutzengröße"]);
                            int maxZyklen = Convert.ToInt32(reader["maxZyklen"]);
                            int warnZyklen = Convert.ToInt32(reader["WarnungZyklen"]);
                            string lager = reader["Lagerplatz"].ToString();
                            string schablonennummer = reader["Schablonennummer"].ToString();
                            string topBottom = reader["TOPBOTTOM"].ToString();
                            string typSchab = reader["Typ"].ToString();

                            // Neue Zyklen berechnen: Auftragsmenge / Nutzengröße
                            int zusatzZyklen = (int)Math.Ceiling((double)auftragsmenge / nutzengroesse);

                            // Zyklen aktualisieren
                            int neuerZyklen = aktZyklen + zusatzZyklen;

                            reader.Close(); // Leser schließen, bevor ein anderes SQL-Kommando ausgeführt wird

                            // Datenbankupdate durchführen
                            string updateQuery = "UPDATE `sliste` SET `aktZyklen` = @neuerZyklen WHERE `CK` = @ckNr";
                            MySqlCommand updateCmd = new MySqlCommand(updateQuery, conn);
                            updateCmd.Parameters.AddWithValue("@neuerZyklen", neuerZyklen);
                            updateCmd.Parameters.AddWithValue("@ckNr", ckNr);
                            updateCmd.ExecuteNonQuery();

                            // Überprüfung der Zyklen und Erstellung von Warnungen
                            if (neuerZyklen > (maxZyklen - warnZyklen) && neuerZyklen <= maxZyklen)
                            {
                                EintragInWarnungen(conn, datum, lager, typSchab, schablonennummer, topBottom, ckNr, benutzer, "WARNUNG: Standzeit bald erreicht.");
                                MessageBox.Show("Standzeit bald erreicht", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                                //E-Mail an Harald schicken wenn Warnung ausgegebn wird!
                            }
                            else if (neuerZyklen > maxZyklen)
                            {
                                EintragInWarnungen(conn, datum, lager, typSchab, schablonennummer, topBottom, ckNr, benutzer, "KRITISCH: Standzeit erreicht!");
                                MessageBox.Show("KRITISCH: Standzeit erreicht!", "Kritisch", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Aktualisieren der Zyklen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EintragInWarnungen(MySqlConnection conn, DateTime datum, string lagerplatz, string typ, string schablonennummer, string topBottom, string ckNummer, string benutzer, string meldung)
        {
            string insertWarnungQuery = "INSERT INTO warnungen (`datum`, `Lagerplatz`, `Typ`, `Schablonennummer`, `TOPBOT`, `CK#`, `Benutzer`, `Meldungstext`) VALUES " +
                                        "(@datum, @lagerplatz, @typ, @schablonennummer, @topBottom, @ckNummer, @benutzer, @meldung)";
            using (MySqlCommand insertWarnungCmd = new MySqlCommand(insertWarnungQuery, conn))
            {
                insertWarnungCmd.Parameters.AddWithValue("@datum", datum);
                insertWarnungCmd.Parameters.AddWithValue("@lagerplatz", lagerplatz);
                insertWarnungCmd.Parameters.AddWithValue("@typ", typ);
                insertWarnungCmd.Parameters.AddWithValue("@schablonennummer", schablonennummer);
                insertWarnungCmd.Parameters.AddWithValue("@topBottom", topBottom);
                insertWarnungCmd.Parameters.AddWithValue("@ckNummer", ckNummer);
                insertWarnungCmd.Parameters.AddWithValue("@benutzer", benutzer);
                insertWarnungCmd.Parameters.AddWithValue("@meldung", meldung);
                insertWarnungCmd.ExecuteNonQuery();
            }
        }


        private void Auftragsmenge(string ckNr)
        {
            bool gültigAuftrag = false;
            int auftragsmenge = 0;

            while (!gültigAuftrag)
            {
                string eingabe = Microsoft.VisualBasic.Interaction.InputBox("Auftragsmenge eingeben: ", "Auftragsmenge");

                if (!int.TryParse(eingabe, out auftragsmenge) && !string.IsNullOrEmpty(eingabe))
                {
                    MessageBox.Show("Ungültige Eingabe!", "Warnung", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (string.IsNullOrEmpty(eingabe))
                {
                    MessageBox.Show("Nichts eingegeben", "Warnung", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }
                else if (auftragsmenge % Nutzengröße(ckNr) != 0)
                {
                    MessageBox.Show("Ungültige Eingabe! Auftragsmenge ist kein Vielfaches der Nutzengröße!", "Warnung", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    gültigAuftrag = true;

                    MessageBoxResult result = MessageBox.Show("Stimmt die Auftragszahl " + auftragsmenge + " mit Ihren Unterlagen überein?", "Sicherheitsfrage", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        gültigAuftrag = false;
                    }
                    else if (result == MessageBoxResult.Yes)
                    {
                        if (BearbeiterAbfragen())
                        {
                            // Zyklen aktualisieren
                            UpdateAktZyklen(ckNr, auftragsmenge); // Auftragsmenge weitergeben
                                                                  // Logbucheintrag erstellen
                            LogbuchEintragErstellen(ckNr, bearbeiter);
                        }
                    }
                }
            }
        }



        private int Nutzengröße(string ckNr)
        {
            int nutzengröße = 0; // Default value

            try
            {
                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();

                    string query = "SELECT `Nutzengröße` FROM sliste WHERE `CK` = @ckNr";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ckNr", ckNr);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        nutzengröße = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }

            return nutzengröße;
        }





        private bool BearbeiterAbfragen()
        {
            bool gültigerBearbeiter = false;
            string bearbeiter = string.Empty;

            while (gültigerBearbeiter == false)
            {
                bearbeiter = Microsoft.VisualBasic.Interaction.InputBox("Namenskürzel des Bearbeiters: ", "Bearbeiter erfassen");

                if (string.IsNullOrEmpty(bearbeiter))
                {
                    MessageBox.Show("Vorgang abgebrochen!", "Warnung", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return false;
                }
                else
                {
                    gültigerBearbeiter = true;

                }

            }
            this.bearbeiter = bearbeiter;
            return true;
        }

        
        private void LogbuchEintragErstellen(string ckNr, string bearbeiter)
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(MainWindow.GlobalSettings.Instance.GetConnectionString()))
                {
                    conn.Open();

                    // Logbucheintrag erstellen
                    string insertQuery = "INSERT INTO `prüfungen` (`Datum`, `CK-Nummer`, `Bearbeiter`) VALUES (@datum, @ckNr, @bearbeiter)";
                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
                    insertCmd.Parameters.AddWithValue("@datum", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    insertCmd.Parameters.AddWithValue("@ckNr", ckNr);
                    insertCmd.Parameters.AddWithValue("@bearbeiter", bearbeiter);

                    insertCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fehler: " + ex.Message);
            }
        }
    }
}
