using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Compiler
{
    internal class CodeHandler
    {
        private RichTextBox richTextBox;
        private KeyWords keyWords;
        private Identifiers identifiers;

        public CodeHandler(RichTextBox textBox)
        {
            richTextBox = textBox;
            keyWords = new KeyWords();
            identifiers = new Identifiers();
        }

        public void HandleText()
        {
            richTextBox.Enabled = false;
            richTextBox.Visible = false;
            Control control = richTextBox.Parent;
            while (control.GetType().Name != "Form1")
            {
                control = control.Parent;
            }
            int selectionStart = richTextBox.SelectionStart;
            int selectionLength = richTextBox.SelectionLength;
            richTextBox.SelectAll();
            richTextBox.SelectionColor = Color.Black;

            MatchCollection matchCollection = keyWords.FindMatches(richTextBox.Text);
            foreach (Match match in matchCollection)
            {
                richTextBox.Select(match.Index, match.Length);
                richTextBox.SelectionColor = Color.Blue;
            }

            identifiers.LoadIdentifiers(richTextBox.Text);
            matchCollection = identifiers.FindMatches(richTextBox.Text);
            foreach (Match match in matchCollection)
            {
                richTextBox.Select(match.Index, match.Length);
                richTextBox.SelectionColor = Color.Red;
            }

            richTextBox.Select(selectionStart, selectionLength);
            richTextBox.Visible = true;
            richTextBox.Enabled = true;
            (control as Form).ActiveControl = richTextBox;
        }
    }
}