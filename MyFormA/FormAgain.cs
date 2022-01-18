
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MyFormA
{
    public partial class FormAgain : Form
    {
        Label messiah = new Label();
        Button[] btn = new Button[2];
        Button btn_tbl;
        TableLayoutPanel tlp = new TableLayoutPanel();
        static List<Pilet> piletid;
        string film;
        static string[] read_kohad;
        TableLayoutPanel tbl = new TableLayoutPanel();
        string[] texts = new string[2];
        
        public FormAgain() 
        { }
      
        int btn_w, btn_h;
        public FormAgain(string title, string body, string button1, string button2)
        {
            texts[0] = button1;
            texts[1] = button2;
            
            
            
            this.ClientSize = new System.Drawing.Size(500, 500);
            this.Text = title;
            int x = 10;
            for (int i = 0; i < 3; i++)
            {
                btn[i] = new Button
                {
                    Location = new System.Drawing.Point(x, 20),
                    Size = new System.Drawing.Size(30, 30),
                    Text = texts[i]
                   
            };
                
                btn[i].Click += FormAgain_Click;
                x += 100;
                this.Controls.Add(btn[i]);
            }
            
            messiah.Location = new System.Drawing.Point(10, 10);
            messiah.Text = "Kas tahad saada e-mailile?";
            this.Controls.Add(messiah);
        }
        

            public FormAgain(int x,int y,string film)
        {
            this.tbl.ColumnCount = x;
            this.tbl.RowCount = y;
            this.tbl.ColumnStyles.Clear();
            this.tbl.RowStyles.Clear();
            this.Text = film;
            read_kohad = Ostetud_piletid();
            piletid = new List<Pilet> { };
            for (int i = 0; i < x; i++)
            {
                this.tbl.RowStyles.Add(new RowStyle(SizeType.Percent,
                    100/y));
            }
            for (int i = 0; i < y; i++)
            {
                this.tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100 / x));
            }
            this.Size = new System.Drawing.Size(y * 100, x * 100);
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    var btn_tbl = new Button
                    {
                        Text = string.Format("rida {0},koht {1}", i+ 1, j+ 1),
                        Name = string.Format("{1}{0}", j+ 1,  i+1),
                        Dock = DockStyle.Fill,
                       
                    };
                    btn_tbl.MouseClick += Btn_tbl_MouseClick;
                    this.tbl.Controls.Add(btn_tbl, i, j);
                    btn_tbl.BackColor = Color.LawnGreen;
                }
                btn_w = (int)(100 / y);
                btn_h = (int)(100 / x);
                this.tbl.Dock = DockStyle.Fill;
               // this.tbl.Size = new System.Drawing.Size(tbl.ColumnCount * btn_w*4, tbl.RowCount * btn_h*4);
                this.Controls.Add(tbl);
            }
        }

        private void Btn_tbl_MouseClick(object sender, MouseEventArgs e)
        {
            Button b = sender as Button;
            b.BackColor = Color.Red;

        }
        public void Saada_piletid(List<Pilet> piletid)
        {

            string text = "Sinu ost on \n";
            foreach (var item in piletid)
            {
                text += "Pilet:\n" + "Rida: " + item.Rida + "Koht: " + item.Koht + "\n";
            }

            //message.Attachments.Add(new Attachment("file.pdf"));
            string email = "programmeeriminetthk@gmail.com";
            string password = "2.kuursus";
            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.Port = 587;
            client.Credentials = new NetworkCredential(email, password);
            client.EnableSsl = true;
            //client.UseDefaultCredentials = true;

            try
            {

                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress("programmeeriminetthk@gmail.com"));//kellele saada vaja küsida
                message.From = new MailAddress("programmeeriminetthk@gmail.com");
                message.Subject = "Ostetud piletid";
                message.Body = text;
                message.IsBodyHtml = true;
                client.Send(message);
                piletid.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public string[] Ostetud_piletid()
        {
            try
            {
                StreamReader f = new StreamReader(@"..\..\piletid\piletid.txt");
                read_kohad = f.ReadToEnd().Split(';');
                f.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return read_kohad;
        }
        private void Pileti_valik(object sender, EventArgs e)
        {
            Button btn_click = (Button)sender;
            btn_click.BackColor = Color.Yellow;
            var rida = int.Parse(btn_click.Name[0].ToString());
            var koht = int.Parse(btn_click.Name[1].ToString());
            var vas = MessageBox.Show("Sinu pilet on: Rida: " + rida + " Koht: " + koht, "Kas ostad?", MessageBoxButtons.YesNo);
            if (vas == DialogResult.Yes)
            {
                btn_click.BackColor = Color.Red;
                btn_click.Enabled = false;
                try
                {
                    Pilet pilet = new Pilet(rida, koht);
                    piletid.Add(pilet);
                    StreamWriter ost = new StreamWriter(@"..\..\piletid\piletid.txt", true);
                    ost.Write(btn_click.Name.ToString() + ';');
                    ost.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (vas == DialogResult.No)
            {
                btn_click.BackColor = Color.Green;
            };

            if (MessageBox.Show("Sul on ostetud: " + piletid.Count() + "piletid", "Kas tahad saada neid e-mailile?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //SendMail("Text");
                Saada_piletid(piletid);
            }

        }

        private void FormAgain_Click(object sender, EventArgs e)
        {
            Button btn_click = (Button)sender;
            //MessageBox.Show(btnS.Text + " button was clicked");
            btn_click.BackColor = Color.Yellow;
            string[] rida_koht = btn_tbl.Name.Split('_');
            MyFormA.Pilet P= new Pilet(int.Parse(rida_koht[1]), int.Parse(rida_koht[0]));
            if (MessageBox.Show("Sinu pilet on : Rida:" + (rida_koht[1]) + "Koht:" + (rida_koht[0]),
                    "Kas ostad?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                btn_click.BackColor = Color.Red;
            }
            else
            {
                btn_click.BackColor = Color.Green;
            };
           


        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "Kino";
            this.ResumeLayout(false);
        }
    }
}
