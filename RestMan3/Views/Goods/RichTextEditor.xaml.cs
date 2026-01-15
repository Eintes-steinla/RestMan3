using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace RestMan3.Views.Goods
{
    public partial class RichTextEditor : UserControl
    {
        public RichTextEditor()
        {
            InitializeComponent();
        }

        // Logic cũ
        public string GetContent()
        {
            var range = new TextRange(RichTextContentBox.Document.ContentStart, RichTextContentBox.Document.ContentEnd);
            return range.Text;
        }

        public void SetContent(string content)
        {
            RichTextContentBox.Document.Blocks.Clear();
            if (!string.IsNullOrEmpty(content))
            {
                var range = new TextRange(RichTextContentBox.Document.ContentStart, RichTextContentBox.Document.ContentEnd);
                range.Text = content;
            }
            else
            {
                RichTextContentBox.Document.Blocks.Add(new Paragraph());
            }
        }

        // --- Event Handlers ---

        private void BoldButton_Click(object sender, RoutedEventArgs e) => ExecuteCommand(EditingCommands.ToggleBold);
        private void ItalicButton_Click(object sender, RoutedEventArgs e) => ExecuteCommand(EditingCommands.ToggleItalic);
        private void UnderlineButton_Click(object sender, RoutedEventArgs e) => ExecuteCommand(EditingCommands.ToggleUnderline);

        private void AlignLeftButton_Click(object sender, RoutedEventArgs e) => ExecuteCommand(EditingCommands.AlignLeft);
        private void AlignCenterButton_Click(object sender, RoutedEventArgs e) => ExecuteCommand(EditingCommands.AlignCenter);
        private void AlignRightButton_Click(object sender, RoutedEventArgs e) => ExecuteCommand(EditingCommands.AlignRight);

        // Chức năng Indent/Outdent
        private void IndentButton_Click(object sender, RoutedEventArgs e) => ExecuteCommand(EditingCommands.IncreaseIndentation);
        private void OutdentButton_Click(object sender, RoutedEventArgs e) => ExecuteCommand(EditingCommands.DecreaseIndentation);

        // Chức năng Lists (Đã sửa tên theo yêu cầu)
        private void UnorderedListButton_Click(object sender, RoutedEventArgs e) => ExecuteCommand(EditingCommands.ToggleBullets);
        private void OrderedListButton_Click(object sender, RoutedEventArgs e) => ExecuteCommand(EditingCommands.ToggleNumbering);

        // Chức năng Hyperlink (Đã sửa tên)
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            RichTextContentBox.Focus();
            var selection = RichTextContentBox.Selection;
            if (selection.IsEmpty) return;

            // Trong thực tế, bạn nên hiện một Dialog để nhập URL ở đây
            string url = "https://";
            var hyperlink = new Hyperlink(selection.Start, selection.End)
            {
                NavigateUri = new System.Uri(url)
            };
        }

        // Hàm bổ trợ để thực thi lệnh chuẩn WPF
        private void ExecuteCommand(RoutedUICommand command)
        {
            RichTextContentBox.Focus();
            command.Execute(null, RichTextContentBox);
        }
    }
}