using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemeGallery
{
    internal class FilterClass
    {
        
        public IEnumerable<Meme> FilterMemes(IEnumerable<Meme> allMemes, string selectedCategory, string searchQuery)
        {
            IEnumerable<Meme> filteredMemes = allMemes ?? [];

            if (!string.IsNullOrWhiteSpace(selectedCategory) && selectedCategory != "Все")
            {
                filteredMemes = filteredMemes.Where(m => m.Category.Equals(selectedCategory, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(searchQuery) && searchQuery != "Поиск по названию...")
            {
                filteredMemes = filteredMemes.Where(
                    meme => meme.Name.ToLower().Contains(searchQuery.ToLower()) ||
                            meme.Tags.Any(tag => tag.ToLower().Contains(searchQuery.ToLower())));
            }

            return filteredMemes;
        }

    }
}
