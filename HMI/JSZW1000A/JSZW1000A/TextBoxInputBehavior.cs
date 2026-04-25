namespace JSZW1000A
{
    internal static class TextBoxInputBehavior
    {
        public static void AttachSelectAllOnFocus(TextBox textBox)
        {
            textBox.Tag = false;
            textBox.GotFocus += SelectAllTextBox_GotFocus;
            textBox.MouseUp += SelectAllTextBox_MouseUp;
        }

        private static void SelectAllTextBox_GotFocus(object? sender, EventArgs e)
        {
            if (sender is TextBox textBox)
                textBox.Tag = true;
        }

        private static void SelectAllTextBox_MouseUp(object? sender, MouseEventArgs e)
        {
            if (sender is not TextBox textBox)
                return;

            if (e.Button == MouseButtons.Left && textBox.Tag is bool shouldSelectAll && shouldSelectAll)
                textBox.SelectAll();

            textBox.Tag = false;
        }
    }
}
