using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace FileClealer
{
    public partial class FormDesktop : Form
    {
        private List<string> itemsToClear = new List<string>();
        private List<string> selectedFiles = new List<string>();
        private string settingsFilePath = "settings.json";

        public FormDesktop()
        {
            InitializeComponent();
            StartSettings();
        }

        private void StartSettings()
        {
            LoadSettings(); // Wczytaj ustawienia przy starcie aplikacji
            cbListToDelete.Items.AddRange(itemsToClear.ToArray());
        }

        private void LoadSettings()
        {
            // Sprawdź, czy plik z ustawieniami istnieje
            if (File.Exists(settingsFilePath))
            {
                // Odczytaj dane z pliku JSON
                string json = File.ReadAllText(settingsFilePath);
                itemsToClear = JsonConvert.DeserializeObject<List<string>>(json);
            }
            else
            {
                // Jeśli plik nie istnieje, dodaj domyślne elementy do listy
                itemsToClear.Add("<i>");
                itemsToClear.Add("</i>");
                itemsToClear.Add(@"{\an1}");
                itemsToClear.Add(@"{\an2}");
                itemsToClear.Add(@"{\an3}");
                itemsToClear.Add(@"{\an4}");
                itemsToClear.Add(@"{\an5}");
                itemsToClear.Add(@"{\an6}");
                itemsToClear.Add(@"{\an7}");
                itemsToClear.Add(@"{\an8}");
                itemsToClear.Add(@"{\an9}");
            }
        }

        private void SaveSettings()
        {
            // Serializuj listę do formatu JSON
            string json = JsonConvert.SerializeObject(itemsToClear);

            // Zapisz dane do pliku
            File.WriteAllText(settingsFilePath, json);
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            FormAddItem aI = new FormAddItem();
            aI.FormClosed += onAdI_Close;
            aI.ShowDialog();
        }

        private void onAdI_Close(object sender, FormClosedEventArgs e)
        {
            FormAddItem aI = (FormAddItem)sender;
            if (string.IsNullOrEmpty(aI.itemText)) return;
            if (itemsToClear.Contains(aI.itemText))
            {
                MessageBox.Show("Istnieje już taki element! Nie zapisano!");
                return;
            }

            itemsToClear.Add(aI.itemText);
            cbListToDelete.Items.Add(aI.itemText);

            SaveSettings();
            aI.Dispose();
        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            if (cbListToDelete.SelectedItem != null)
            {
                string selectedItem = cbListToDelete.SelectedItem.ToString();

                // Usuń z listy i z ComboBox
                itemsToClear.Remove(selectedItem);
                cbListToDelete.Items.Remove(selectedItem);

                // Zapisz zmienione ustawienia
                SaveSettings();
            }
        }


        private void btnChooseFiles_Click(object sender, EventArgs e)
        {
            oFD1.InitialDirectory = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Desktop");
            oFD1.Filter = "Pliki .srt|*.srt";

            if (oFD1.ShowDialog() == DialogResult.OK)
            {
                selectedFiles.AddRange(oFD1.FileNames);
                label1.Text = $"Liczba wybranych plików: {selectedFiles.Count}";
            }
        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (string filePath in selectedFiles)
                {
                    if (File.Exists(filePath))
                    {
                        string content = File.ReadAllText(filePath);
                        foreach (string iTC in itemsToClear)
                        {
                            content = content.Replace(iTC, "");
                        }
                        File.WriteAllText(filePath, content);
                    }
                }
                selectedFiles.Clear();
                MessageBox.Show("Nadpisano!");

                label1.Text = $"Liczba wybranych plików: {selectedFiles.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Coś nie tak. Podeślij mię to info: \n {ex}", "Coś nie działa!");
            } 
        }
    }
}
