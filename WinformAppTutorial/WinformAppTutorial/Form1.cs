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
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;


namespace WinformAppTutorial
{
    public partial class WinFormApp : Form
    {
        Font defaultFont;
        private char[] splitCharacters = "\n ,,:;\"'?!".ToArray();
        bool isEditingInProgress = false;
        string fileToOpen;
        //string fileToSave;
        //string retrievedText = "";
        private Image originalImage;

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
            // TODO: This line of code loads data into the 'testDataSet.Employee' table. You can move, or remove it, as needed.
            this.employeeTableAdapter.Fill(this.testDataSet.Employee);
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
            picBox.SizeMode = PictureBoxSizeMode.Zoom;
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

        private async void lblCountWord_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Counting words...";
            lblCountWord.Enabled = false;

            string textToCount = txtMessage.Text;

            int wordCount = await Task.Run(() => CountWords(textToCount));

            lblStatus.Text = $"Done! Total words: {wordCount}";
            lblCountWord.Enabled = true;
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

        private void connectDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string connString = @"Data Source=ITD-IS-PHUCHT;User Id=sa;Password=1234;Database=test;Trusted_Connection=True;";
            // Nếu bạn dùng SQL Authentication (có user/pass):
            // string connString = @"Server=.\SQLEXPRESS;Database=YourDatabaseName;User Id=sa;Password=123456;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {

                    string query = @"SELECT * FROM EMPLOYEE";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open(); // Mở kết nối
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        while(dr.Read())
                        {
                            MessageBox.Show(dr.GetInt32(0).ToString() + " - " + dr.GetString(1));
                        }
                    }
                    else
                    {
                        MessageBox.Show("No Data Found");
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ Kết nối thất bại: " + ex.Message);
            }
        }

        private void employeeBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.employeeBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.testDataSet);

        }

        private void bindingNavigatorDeleteItem_Click(object sender, EventArgs e)
        {

        }



        private void lnkOpen_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            dlgOpenFile.FileName = "";
            dlgOpenFile.Filter = "Image files(*.gif; *.jpg; *.jpeg; *.bmp; *.wmf; *.png)| *.gif; *.jpg; *.jpeg; *.bmp; *.wmf; *.png"; 


            dlgOpenFile.ShowDialog();

            if (!string.IsNullOrEmpty(dlgOpenFile.FileName))
                picBox.Image = Image.FromFile(@dlgOpenFile.FileName);
                picBox.SizeMode = PictureBoxSizeMode.CenterImage;
                originalImage = picBox.Image;
        }

        private void lnkAuto_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            picBox.Image = originalImage;
            picBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void lnkZoomIn_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            picBox.SizeMode = PictureBoxSizeMode.AutoSize;

            Image img = picBox.Image;

            Size size = new Size((int)(img.Width * 1.2), (int)(img.Height * 1.2));

            Bitmap bm = new Bitmap(img, size);
            Graphics g = Graphics.FromImage(bm);

            g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            picBox.Image = bm;
            picBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void lnkZoomOut_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            picBox.SizeMode = PictureBoxSizeMode.AutoSize;

            Image img = picBox.Image;

            Size size = new Size((int)(img.Width * 0.8), (int)(img.Height * 0.8));

            //check if new size width or height are below 40 in order not to allow further zooming out 
            if (size.Width < 40 || size.Height < 40)
                return;

            Bitmap bm = new Bitmap(img, size);
            Graphics g = Graphics.FromImage(bm);

            g.InterpolationMode = InterpolationMode.HighQualityBilinear;

            picBox.Image = bm;
            picBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void lnkPrint_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PrintDialog dg = new PrintDialog();
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += DocPrintPage;
            dg.Document = doc;

            if (dg.ShowDialog() == DialogResult.OK)
                doc.Print();
        }

        private void DocPrintPage(object sender, PrintPageEventArgs e)
        {
            Bitmap bm = new Bitmap(picBox.Width, picBox.Height);
            picBox.DrawToBitmap(bm, new Rectangle(0, 0, picBox.Width, picBox.Height));
            e.Graphics.DrawImage(bm, 0, 0);
            bm.Dispose();

        }
    }
}
