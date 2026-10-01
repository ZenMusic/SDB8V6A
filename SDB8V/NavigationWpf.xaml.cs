using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SymbolDB
{
    public partial class NavigationWpf : Window
    {
        private readonly Action<int> _selectImage;

        public NavigationWpf(AnnotationsWpf owner, Action<int> selectImage, IReadOnlyList<AnnotationThumbnail> thumbnails)
        {
            InitializeComponent();
            Owner = owner;
            _selectImage = selectImage ?? throw new ArgumentNullException(nameof(selectImage));
            ThumbnailItemsControl.ItemsSource = thumbnails;
        }

        private void Picture_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border picture &&
                int.TryParse(picture.Tag?.ToString(), out int index))
            {
                _selectImage(index);
            }
        }
    }
}
