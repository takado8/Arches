using System.Windows.Media;

namespace Arches.viewModel
{
    internal static class Constants
    {
        public const string textBoxPlaceholder = "Nowy zabieg...";

        public static Brush getUnselectedItemBrush()
        {
            return Brushes.Transparent;
        }

        public static Brush getSelectedItemBrush()
        {
            return Brushes.AliceBlue;
        }

        public static Brush getSelectedCategoryItemBrush()
        {
            return Brushes.LightBlue;
        }

        public static Brush getCursorFrameBrush()
        {
            return Brushes.DeepPink;
        }
    }
}
