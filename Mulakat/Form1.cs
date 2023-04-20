using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Mulakat
{
    public partial class Form1 : Form
    {
        SQLiteConnection con;
        SQLiteDataAdapter da;
        DataSet ds;
        int counter;
        List<string> barkodlar = new List<string>();
        List<string> fav = new List<string>();
        string html;
        public Form1()
        {
            InitializeComponent();
            con = new SQLiteConnection("Data Source=\"C:\\Users\\abugr\\source\\repos\\Mulakat\\Mulakat\\rxsample.db\";Version=3;");
            ds = new DataSet();
            da = new SQLiteDataAdapter("Select * FROM ILACLAR LEFT JOIN ILAC_FORM ON ILACLAR.ID = ILAC_FORM.ILAC_ID LEFT JOIN ILAC_AMBALAJ ON ILAC_FORM.ID = ILAC_AMBALAJ.ILAC_FORM_ID LEFT JOIN ILAC_ETKIN_MADDELER ON ILAC_FORM.ID = ILAC_ETKIN_MADDELER.ILAC_FORM_ID LEFT JOIN ETKIN_MADDELER ON ILAC_ETKIN_MADDELER.ETKIN_MADDE = ETKIN_MADDELER.ID", con);
            con.Open();
            da.Fill(ds);
            DataView view = new DataView(ds.Tables[0]);
            DataTable dataTable = view.ToTable(true, "BARKOD", "ILAC_ADI", "OLCU", "AMBALAJ");
            foreach (DataRow item in ds.Tables[0].Rows)
            {
                Button btn = new Button();
                btn.Size = new Size(240, 60);
                btn.Text = String.Format("{0} {1} {2}", item["ILAC_ADI"].ToString(), item["OLCU"].ToString(), item["AMBALAJ"].ToString());
                btn.Tag = item["BARKOD"].ToString();
                btn.Name = btn.Text + " " + btn.Tag;
                btn.Click += Btn_Click;
                flowLayoutPanel1.Controls.Add(btn);
            }
        }

        private void Search(string barkodno)
        {
            flowLayoutPanel2.Controls.Clear();
            var test = (from rows in ds.Tables[0].AsEnumerable() where rows["BARKOD"].ToString() == barkodno.ToString() select rows).ToList();
            isim.Text = String.Format("{0} {1}", test[0]["ILAC_ADI"].ToString(), test[0]["OLCU"].ToString());
            kutu.Text = test[0]["AMBALAJ"].ToString();
            barkod.Text = test[0]["BARKOD"].ToString();
            firma.Text = test[0]["FIRMA"].ToString();
            fiyat.Text = test[0]["FIYAT"].ToString();
            kamuFiyati.Text = test[0]["KAMUFIYATI"].ToString();
            kamuOdenen.Text = test[0]["KAMUODENEN"].ToString();
            depoFiyati.Text = test[0]["DEPOCU"].ToString();
            imalatFiyati.Text = test[0]["IMALATCI"].ToString();
            kdv.Text = "%8";
            jo.Text = test[0]["JENERIKORIJINAL"].ToString();
            sgk.Text = test[0]["SGKETKINKODU"].ToString();
            atc.Text = test[0]["ATCKODU"].ToString();
            recete.Text = test[0]["RECETE"].ToString();
            html = test[0]["KUB"].ToString();
            pictureBox1.Image = (test[0]["AMBALAJRESIM"] != System.DBNull.Value) ? (Bitmap)(new ImageConverter()).ConvertFrom(test[0]["AMBALAJRESIM"]) : null;
            foreach (var item in test)
            {
                Label label = new Label();
                label.AutoSize = true;
                label.Text = item["ETKINMADDE"].ToString() + " " + item["MIKTAR"].ToString() + item["BIRIM"].ToString();
                flowLayoutPanel2.Controls.Add(label);
            }
            button3.BackColor = fav.Contains(barkod.Text) ? Color.Red : Color.White;
        }
        private void Btn_Click(object sender, EventArgs e)
        {
            Search((sender as Button).Tag.ToString());
            barkodlar.Add((sender as Button).Tag.ToString());
            counter = barkodlar.Count - 1;
            button1.Enabled = barkodlar.Count > 1 ? true : false;
            button3.Enabled = true;

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                foreach (Control item in flowLayoutPanel1.Controls)
                {
                    item.Visible = (item.Name.IndexOf(textBox1.Text, StringComparison.OrdinalIgnoreCase) >= 0) ? true : false;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (counter - 1 != -1)
            {
                counter--;
                Search(barkodlar[counter]);
                button2.Enabled = true;
                if (counter == 0)
                {
                    button1.Enabled = false;
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (counter + 1 != barkodlar.Count)
            {
                counter++;
                Search(barkodlar[counter]);
                button1.Enabled = true;
                if (counter + 1 == barkodlar.Count)
                {
                    button2.Enabled = false;

                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (button3.BackColor == Color.Red)
            {
                button3.BackColor = Color.White;
                fav.Remove(barkod.Text);
            }
            else
            {
                button3.BackColor = Color.Red;
                fav.Add(barkod.Text);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var formPopup = new Form();
            WebBrowser wb = new WebBrowser();
            wb.DocumentText = html;
            formPopup.Controls.Add(wb);
            formPopup.Show(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(barkod.Text);
        }
    }
}
