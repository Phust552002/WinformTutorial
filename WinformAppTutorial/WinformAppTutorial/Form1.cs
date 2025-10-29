using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace WinformAppTutorial
{
    public partial class WinFormApp : Form
    {
        Font defaultFont;
        private char[] splitCharacters = "\n ,,:;\"'?!".ToArray();
        bool isEditingInProgress = false;
        string fileToOpen;
        string fileToSave;
        string retrievedText = "";

        public WinFormApp()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this,"Text Submitted","Message to User",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Sample", "About the program", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void btnDisplay_Click(object sender, EventArgs e)
        {
            MessageBox.Show(txtMessage.Text);
        }


        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void WinFormApp_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            if (txtMessage.WordWrap)
            {
                wordWrapOffToolStripMenuItem.Text = "Word Wrap (Off) ";

            }
            else
            {
                wordWrapOffToolStripMenuItem.Text = "Word Wrap (On) ";

            }
            defaultFont = txtMessage.Font;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
               if (isEditingInProgress)
            {
                DialogResult drDiscard = MessageBox.Show(this, "You are currently edit a file", "Open Txt Document", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (drDiscard != DialogResult.Yes)
                {
                    return;
                }
            }

            DialogResult dr = dlgOpenFile.ShowDialog();
            if ( dr == DialogResult.OK)
            {
                fileToOpen = dlgOpenFile.FileName;

                try
                {
                    txtMessage.Text = File.ReadAllText(@fileToOpen);
                    lblStatus.Text = "Opened File: " + @fileToOpen;
                    isEditingInProgress = true;
                    this.Text = "Text Editor Demo App - " + fileToOpen;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, "error message", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                return;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = dlgSaveFile.ShowDialog();
            if ( dr == DialogResult.OK)
            {
                string fileToSave = @dlgSaveFile.FileName;
                try
                {
                    File.WriteAllText(@fileToSave, txtMessage.Text);
                    lblStatus.Text = "File saved as: " + @fileToSave;
                    isEditingInProgress = false;
                    this.Text = "Text Editor DemoApp - " + @fileToSave;
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, "error message", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else {
                return;
            }
        }

        private void wordWrapOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(txtMessage.WordWrap)
            {
                txtMessage.WordWrap = false;
                wordWrapOffToolStripMenuItem.Text = "Word Wrap (Off) ";
            }
            else
            {
                txtMessage.WordWrap = true;
                wordWrapOffToolStripMenuItem.Text = "Word Wrap (On)";
            }
        }

        private void decreseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (txtMessage.Font.Size > 8)
            {
                txtMessage.Font = new Font("Microsoft Sans Serif", txtMessage.Font.Size - 2);
            }
            else
            {
                MessageBox.Show(this, "Min Size Reached (" + Convert.ToInt32(txtMessage.Font.Size).ToString() + ")");
            }
        }

        private void increaseFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (txtMessage.Font.Size <18)
            {
                txtMessage.Font = new Font("Microsoft Sans Serif", txtMessage.Font.Size + 2);
            }
            else
            {
                MessageBox.Show(this, "Max Size Reached ("+ Convert.ToInt32(txtMessage.Font.Size).ToString()+")");
            }
        }

        private void resetFontSizeToDefaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtMessage.Font = defaultFont;
        }

        private async void btnCountWords_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Counting words...";
            btnCountWords.Enabled = false;

            string textToCount = txtMessage.Text;

            int wordCount = await Task.Run(() => CountWords(textToCount));

            lblStatus.Text = $"Done! Total words: {wordCount}";
            btnCountWords.Enabled = true;
        }

        private int CountWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return 0;

            Thread.Sleep(2000);

            char[] splitChars = { ' ', '\n', '\r', '\t', ',', '.', ';', ':', '!', '?' };
            var words = text.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

    }
}
