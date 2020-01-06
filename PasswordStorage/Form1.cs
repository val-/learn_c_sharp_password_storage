using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;


namespace PasswordStorage
{
    public partial class Form1 : Form
    {
        public Form1(string masterPassword)
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
                string passwordEncrypted = siteNode.Attributes["password"].Value;
                using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider())
                {

                    byte[] key = GenerateKey(masterPassword, 32);
                    byte[] vi = GenerateKey(masterPassword, 16);
                    string password = DecryptString_Aes(passwordEncrypted, key, vi);
                    comboBox1.Items.Add(new SiteItem(url, login, password));
                }


                
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

        static byte[] GenerateKey(string masterPassword, int count) {
            byte[] masterPasswordBytes = System.Text.Encoding.UTF8.GetBytes(masterPassword);
            byte[] resultBytes = new byte[count]; 
            for (int i=0; i<count; i++) {
                if (i < masterPasswordBytes.Length)
                {
                    resultBytes[i] = masterPasswordBytes[i];
                }
                else {
                    resultBytes[i] = 1;
                }
            }
            return resultBytes;
        }
        static string EncryptString_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return String.Join("_", encrypted.Select(b => b.ToString("x2")).ToArray());

        }

        static string DecryptString_Aes(string encrypted, byte[] Key, byte[] IV)
        {

            string[] encryptedParts = encrypted.Split('_');
            byte[] cipherText = new byte[encryptedParts.Length];
            for (int i=0; i< encryptedParts.Length; i++) {
                cipherText[i] = Convert.ToByte(encryptedParts[i], 16);
            }

            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an AesCryptoServiceProvider object
            // with the specified key and IV.
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return plaintext;

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
