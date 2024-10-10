using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;


namespace MemeGallery
{
    /// <summary>
    /// Логика взаимодействия для AddMemrWindow.xaml
    /// </summary>
    public partial class AddMemeWindow : Window
    {
        public string MemeName { get; private set; }
        public string MemeCategory { get; private set; }
        public string MemeImagePath { get; private set; }

        public AddMemeWindow()
        {
            InitializeComponent();
            var categories = new List<string> { "Мемы", "Стикеры", "Гифки" };
            CategoryComboBox.ItemsSource = categories;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpg;*.gif)|*.png;*.jpg;*.gif|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                ImagePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            MemeName = NameTextBox.Text;
            MemeCategory = CategoryComboBox.SelectedItem as string;
            MemeImagePath = ImagePathTextBox.Text;

            if (string.IsNullOrWhiteSpace(MemeName) || string.IsNullOrWhiteSpace(MemeCategory) || string.IsNullOrWhiteSpace(MemeImagePath))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string localImagePath = MemeImagePath;

            if (IsUrl(MemeImagePath))
            {
                try
                {
                    // Скачиваем изображение из URL и сохраняем его локально
                    localImagePath = DownloadImageFromUrl(MemeImagePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            MemeImagePath = localImagePath;
            }
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private bool IsUrl(string path)
        {
            return Uri.TryCreate(path, UriKind.Absolute, out Uri uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        private string DownloadImageFromUrl(string url)
        {
            using WebClient webClient = new();
            var uri = new Uri(url);
            var fileName = Path.GetFileName(uri.LocalPath);
            var localFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

            // Создаем папку Images, если ее нет
            if (!Directory.Exists(localFolderPath))
            {
                Directory.CreateDirectory(localFolderPath);
            }

            var localPath = Path.Combine(localFolderPath, fileName);
            webClient.DownloadFile(uri, localPath);
            return localPath;
        }
    }
}
