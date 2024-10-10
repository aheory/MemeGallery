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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.IO;
using WpfAnimatedGif;
using static System.Net.Mime.MediaTypeNames;


namespace MemeGallery
{
    public partial class MainWindow : Window
    {
        public List<Meme> _allMemes;
        private readonly FilterClass filterClass;
        private readonly DataMemes dataMemes;
         
        public MainWindow()
        {
            InitializeComponent();
            filterClass = new FilterClass();
            dataMemes = new DataMemes();
            _allMemes = dataMemes.LoadMemes();
            DataContext = this;
            var categories = new List<string> { "Все", "Мемы", "Стикеры", "Гифки" };
            CategoryComboBox.ItemsSource = categories;
            CategoryComboBox.SelectedIndex = 0;
            FilterMemes();
        }

        private void AddMemeButton_Click(object sender, RoutedEventArgs e)
        {
            var addMemeWindow = new AddMemeWindow();
            if (addMemeWindow.ShowDialog() == true)
            {
                var newMeme = new Meme
                {
                    Name = addMemeWindow.MemeName,
                    Category = addMemeWindow.MemeCategory,
                    ImagePath = addMemeWindow.MemeImagePath
                };
                _allMemes.Add(newMeme);
                FilterMemes();
            }
        }

        private void DeleteMemeButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemeListBox.SelectedItem is Meme selectedMeme)
            {
                _allMemes.Remove(selectedMeme);
                FilterMemes();
                MemeImage.Source = null;
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите мем для удаления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddTagButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemeListBox.SelectedItem is Meme selectedMeme)
            {
                var newTag = TagTextBox.Text.Trim();
                if (!string.IsNullOrWhiteSpace(newTag) && !selectedMeme.Tags.Contains(newTag))
                {
                    selectedMeme.Tags.Add(newTag);
                    TagsListBox.Items.Refresh();
                    TagTextBox.Clear();
                    FilterMemes();
                }
            }
        }

        private void RemoveTagButton_Click(object sender, RoutedEventArgs e)
        {
            if (MemeListBox.SelectedItem is Meme selectedMeme && TagsListBox.SelectedItem is string selectedTag)
            {
                selectedMeme.Tags.Remove(selectedTag);
                TagsListBox.Items.Refresh();
            }
        }

        private void MemeListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MemeListBox == null || MemeListBox.SelectedItem == null)
                return;
            if (MemeListBox.SelectedItem is Meme selectedMeme)
            {
                var imageUri = new Uri(selectedMeme.ImagePath, UriKind.RelativeOrAbsolute);
                var image = new BitmapImage(imageUri);
                ImageBehavior.SetAnimatedSource(MemeImage, image);
                MemeName.Text = selectedMeme.Name;
                MemeCategory.Text = selectedMeme.Category;
                TagsListBox.ItemsSource = selectedMeme.Tags;
            }
            else
            {
                MemeImage.Source = null;
                MemeName.Text = string.Empty;
                MemeCategory.Text = string.Empty;
                TagsListBox.ItemsSource = null;
            }
        }

        private void CategoryComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            FilterMemes();
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            FilterMemes();
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchTextBox.Text == "Поиск по названию...")
            {
                SearchTextBox.Text = "";
                SearchTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }


        private void SearchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                SearchTextBox.Text = "Поиск по названию...";
                SearchTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }

        private void TagTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TagTextBox.Text == "Добавить тег")
            {
                TagTextBox.Text = "";
                TagTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void TagTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TagTextBox.Text))
            {
                TagTextBox.Text = "Добавить тег";
                TagTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }
        
        private void FilterMemes()                                              
        {
            if (MemeListBox == null)
                return;

            var selectedCategory = CategoryComboBox.SelectedItem as string;
            var searchQuery = SearchTextBox.Text;

            var filteredMemes = filterClass.FilterMemes(_allMemes, selectedCategory, searchQuery);

            AllTagsListBox.ItemsSource = filteredMemes?.SelectMany(meme => meme.Tags).Distinct() ?? [];
            MemeListBox.ItemsSource = filteredMemes.ToList();
        }

        private void SaveMemeListButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dataMemes.SaveMemes(_allMemes);
                MessageBox.Show("Список мемов успешно сохранен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении списка мемов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadMemeListButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _allMemes = dataMemes.LoadMemes();
                FilterMemes();
                MessageBox.Show("Список мемов успешно загружен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка мемов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }

    public class Meme
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string ImagePath { get; set; }
        public List<string> Tags { get; set; } = [];

        public override string ToString()
        {
            return Name;
        }
    }
}

