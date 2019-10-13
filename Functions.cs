using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieRentalStore
{
    class Functions
    {
        public static void AddMoviesGenre(ComboBox combo)
        {
            // Clear all the existing items to make sure it's adding items from start whenever calling the function
            combo.Items.Clear();

            // Add list of all genre list
            string[] genre = { "Action", "Adventure", "Comedy", "Drama", "Fantasy", "Historical", "Horror", "Magic", "Martial Arts", "Mecha", "Military", "Music", "Mystery", "Romance" };
            combo.Items.AddRange(genre);

            // Set default selected item as the first available index
            combo.SelectedIndex = 0;
        }

        public static void DisplayMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
