using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MemeGallery
{
    internal class DataMemes
    {
        private const string FileName = "memes.json";
        public List<Meme> _allMemes;
        
        public List<Meme> LoadMemes()
        {
            try
            {
                if (File.Exists(FileName))
                {
                    var jsonString = File.ReadAllText(FileName);
                    return JsonSerializer.Deserialize<List<Meme>>(jsonString) ?? [];
                }
                else
                {
                    return [];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке списка мемов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return [];
            }
        }

        public void SaveMemes(List<Meme> memes)
        {
            try
            {
                var jsonString = JsonSerializer.Serialize(memes);
                File.WriteAllText(FileName, jsonString);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении списка мемов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

    }
}
