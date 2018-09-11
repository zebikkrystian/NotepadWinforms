using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EdytorTekstu
{
    public partial class MainForm : Form
    {
        private static string ProgramName = "Edytor kodu C# - ";
        private static string _filePath;

        private Color _syntax;
        private Color _strings;
        private Color _numbers;
        private Color _base;


        private bool _isHiglight = false;

        public MainForm()
        {
            InitializeComponent();
            SetTitle("nowy plik");

            _syntax = Color.FromArgb(57, 135, 186);
            _strings = Color.FromArgb(0, 155, 0);
            _numbers = Color.FromArgb(241, 7, 0);
            _base = Color.Black;
        }

        private void SetTitle(string fileName)
        {
            Text = $"{ProgramName}{fileName}";
        }

        private void newMenuItem_Click(object sender, System.EventArgs e)
        {
            editor.Clear();
        }

        private void openMenuItem_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.Title = "Wybierz plik do edycji";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.StreamReader sr = new
                    System.IO.StreamReader(openFileDialog1.FileName);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                editor.Text = sr.ReadToEnd();
                watch.Stop();

                MessageBox.Show("Time: " + watch.ElapsedMilliseconds);
                SetTitle(openFileDialog1.SafeFileName);
                sr.Close();
                _filePath = openFileDialog1.FileName;
            }


        }

        private void saveAsMenuItem_Click(object sender, System.EventArgs e)
        {
            saveFileDialog1.InitialDirectory = @"C:\";
            saveFileDialog1.Title = "Wybierz miejsce zapisu";
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, editor.Text);

            }
        }

        private void newBtn_Click(object sender, System.EventArgs e)
        {
            newMenuItem_Click(sender, e);
        }

        private void openBtn_Click(object sender, System.EventArgs e)
        {
            openMenuItem_Click(sender, e);
        }
        private void saveMenuItem_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(_filePath))
            {
                saveAsMenuItem_Click(sender, e);
                return;
            }

            System.IO.File.WriteAllText(_filePath, editor.Text);
        }


        private void saveBtn_Click(object sender, System.EventArgs e)
        {
            saveMenuItem_Click(sender, e);
        }

        private void closeMenuItem_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void infoMenuItem_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show("Program do edycji tekstu.\nAutor: Krystian Zębik");
        }

        private void prevBtn_Click(object sender, System.EventArgs e)
        {
            editor.Undo();
        }

        private void nextBtn_Click(object sender, System.EventArgs e)
        {
            editor.Redo();
        }

        private void cutBtn_Click(object sender, System.EventArgs e)
        {
            editor.Cut();
        }

        private void copyBtn_Click(object sender, System.EventArgs e)
        {
            editor.Copy();
        }

        private void pasteBtn_Click(object sender, System.EventArgs e)
        {
            editor.Paste();
        }

        private void prevMenuItem_Click(object sender, System.EventArgs e)
        {
            prevBtn_Click(sender, e);
        }

        private void nextMenuItem_Click(object sender, System.EventArgs e)
        {
            nextBtn_Click(sender, e);
        }

        private void cutMenuItem_Click(object sender, System.EventArgs e)
        {
            cutBtn_Click(sender, e);
        }

        private void copyMenuItem_Click(object sender, System.EventArgs e)
        {
            copyBtn_Click(sender, e);
        }

        private void copyAllMenuItem_Click(object sender, System.EventArgs e)
        {
            editor.SelectAll();
            editor.Copy();
        }

        private void pasteMenuItem_Click(object sender, System.EventArgs e)
        {
            pasteBtn_Click(sender, e);
        }

        private void selectAllMenuItem_Click(object sender, System.EventArgs e)
        {
            editor.SelectAll();
        }


        private void CheckKeyword(string word)
        {

            if (this.editor.Text.Contains(word))
            {
                int index = -1;
                int selectStart = this.editor.SelectionStart;

                while ((index = this.editor.Text.IndexOf(word, (index + 1))) != -1)
                {
                    this.editor.Select(index, word.Length);
                    this.editor.SelectionColor = _syntax;
                    this.editor.Select(selectStart, 0);
                    this.editor.SelectionColor = _base;
                }
            }
        }

        private void CheckStrings()
        {
            int selectStart = this.editor.SelectionStart;
            var selection = new List<KeyValuePair<int, int>>();

            var text = editor.Text.ToCharArray();
            int index = 0;
            for (int i = 0; i < text.Length; i++)
            {
                var letter = text[i];
                if (letter == '"')
                {
                    if (index == 0)
                    {
                        index = i;
                        continue;
                    }

                    selection.Add(new KeyValuePair<int, int>(index, i));
                    index = 0;
                }

            }

            if (index != 0)
            {
                selection.Add(new KeyValuePair<int, int>(index, text.Length));
            }

            foreach (var s in selection)
            {
                var length = s.Value - s.Key + 1;
                this.editor.Select(s.Key, length);
                this.editor.SelectionColor = _strings;
                this.editor.Select(selectStart, 0);
                this.editor.SelectionColor = _base;
            }
        }

        private void CheckNumbers()
        {
            int selectStart = this.editor.SelectionStart;
            var selection = new List<KeyValuePair<int, int>>();

            var text = editor.Text.ToCharArray();
            int index = 0;
            for (int i = 0; i < text.Length; i++)
            {
                var letter = text[i];
                if (letter >= 48 && letter <= 57)
                {
                    if (index == 0)
                    {
                        index = i;
                    }


                    continue;
                }

                if (index != 0)
                {
                    selection.Add(new KeyValuePair<int, int>(index, i));
                    index = 0;
                }

            }

            foreach (var s in selection)
            {
                var length = s.Value - s.Key;
                this.editor.Select(s.Key, length);
                this.editor.SelectionColor = _numbers;
                this.editor.Select(selectStart, 0);
                this.editor.SelectionColor = _base;
            }
        }

        private void editor_TextChanged(object sender, EventArgs e)
        {
        }

        private void SetHiglights()
        {
            var syntaxWords = GetSyntaxWords();
            foreach (var word in syntaxWords)
            {
                CheckKeyword($"{word}");
            }

            CheckStrings();
            CheckNumbers();

        }

        private List<string> GetSyntaxWords()
        {
            return new List<string>
            {
                "abstract",
                "base",
                "bool",
                "break",
                "byte",
                "case",
                "catch",
                "char",
                "class",
                "const",
                "continue",
                "decimal",
                "default",
                "double",
                "else",
                "enum",
                "false",
                "finally",
                "float",
                "for",
                "foreach",
                "goto",
                "if",
                "int",
                "internal",
                "interface",
                "long",
                "namespace",
                "new",
                "null",
                "object",
                "out",
                "override",
                "params",
                "private",
                "protected",
                "public",
                "readonly",
                "return",
                "short",
                "sizeof",
                "static",
                "string",
                "switch",
                "this",
                "throw",
                "true",
                "try",
                "typeof",
                "var",
                "using",
                "virtual",
                "void",
                "while"
            };
        }


        private void higlightsBtn_Click(object sender, EventArgs e)
        {
            SetHiglights();
        }
    }
}
