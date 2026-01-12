using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace RestMan3.Views.Goods
{
    public partial class RichTextEditor : UserControl
    {
        public RichTextEditor()
        {
            InitializeComponent();
        }

        public string GetContent()
        {
            var range = new TextRange(RichTextContentBox.Document.ContentStart, RichTextContentBox.Document.ContentEnd);
            return range.Text;
        }

        public void SetContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                RichTextContentBox.Document.Blocks.Clear();
                RichTextContentBox.Document.Blocks.Add(new Paragraph());
            }
            else
            {
                var range = new TextRange(RichTextContentBox.Document.ContentStart, RichTextContentBox.Document.ContentEnd);
                range.Text = content;
            }
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            RichTextContentBox.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            RichTextContentBox.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, FontStyles.Italic);
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            if (RichTextContentBox.Selection.GetPropertyValue(TextBlock.TextDecorationsProperty) == DependencyProperty.UnsetValue)
            {
                RichTextContentBox.Selection.ApplyPropertyValue(TextBlock.TextDecorationsProperty, TextDecorations.Underline);
            }
            else
            {
                RichTextContentBox.Selection.ApplyPropertyValue(TextBlock.TextDecorationsProperty, null);
            }
        }

        private void AlignLeftButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            RichTextContentBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Left);
        }

        private void AlignCenterButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            RichTextContentBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Center);
        }

        private void AlignRightButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            RichTextContentBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Right);
        }

        private void JustifyButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            RichTextContentBox.Selection.ApplyPropertyValue(Paragraph.TextAlignmentProperty, TextAlignment.Justify);
        }

        private void BulletListButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            var list = new List();
            list.MarkerStyle = TextMarkerStyle.Disc;
            var listItem = new ListItem(new Paragraph(new Run("Item")));
            list.ListItems.Add(listItem);

            RichTextContentBox.Document.Blocks.Add(list);
        }

        private void NumberedListButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            var list = new List();
            list.MarkerStyle = TextMarkerStyle.Decimal;
            var listItem = new ListItem(new Paragraph(new Run("Item")));
            list.ListItems.Add(listItem);

            RichTextContentBox.Document.Blocks.Add(list);
        }

        private void LinkButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            var selectedText = RichTextContentBox.Selection.Text;
            if (!string.IsNullOrEmpty(selectedText))
            {
                var hyperlink = new Hyperlink(new Run(selectedText))
                {
                    NavigateUri = new System.Uri("https://example.com"),
                    Foreground = System.Windows.Media.Brushes.Blue,
                    TextDecorations = TextDecorations.Underline
                };
                RichTextContentBox.Selection.Text = string.Empty;
            }
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            MessageBox.Show("Image insertion feature will be improved in the next version.", "Information");
        }
    }
}
