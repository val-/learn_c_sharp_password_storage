using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace PasswordStorage
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();

            XmlTextReader reader = new XmlTextReader("../../storage.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);

            XmlNodeList siteNodes = doc.DocumentElement.SelectNodes("/storage/site");

            foreach (XmlNode siteNode in siteNodes)
            {
                string url = siteNode.Attributes["url"].Value;
                string login = siteNode.Attributes["login"].Value;
                string password = siteNode.Attributes["password"].Value;
                comboBox1.Items.Add(new SiteItem(url, login, password));
            }

        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;
            SiteItem item = (SiteItem)senderComboBox.SelectedItem;
            label1.Text = item.GetLogin();
            label2.Text = item.GetPassword();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, (Object)label1.Text);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetData(DataFormats.Text, (Object)label2.Text);
        }
    }

    public class SiteItem
    {
        public SiteItem(string url, string login, string password)
        {
            this.url = url;
            this.login = login;
            this.password = password;
        }

        public override String ToString()
        {
            return this.url;
        }
        public String GetLogin()
        {
            return this.login;
        }

        public String GetPassword()
        {
            return this.password;
        }

        private string url;
        private string login;
        private string password;

    }

}
