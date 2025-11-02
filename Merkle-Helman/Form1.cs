using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Merkle_Helman
{
    public partial class Form1 : Form
    {
        private ComboBox cmbBitLength;
        private Button btnGenerateKeys;
        private TextBox txtPrivateKey;
        private TextBox txtPublicKey;
        private TextBox txtMessage;
        private TextBox txtCiphertext;
        private TextBox txtDecrypted;
        private Button btnEncrypt;
        private Button btnDecrypt;
        private Label lblBitLength;
        private Label lblPrivateKey;
        private Label lblPublicKey;
        private Label lblMessage;
        private Label lblCiphertext;
        private Label lblDecrypted;

        private List<BigInteger> privateWeights; // Sir cu crestere mare (W)
        private BigInteger modulusQ;
        private BigInteger multiplierR;
        private BigInteger inverseR;
        private List<BigInteger> publicKey; // Cheia publica (B)

        public Form1()
        {
            InitializeComponentFromCode();
        }

        private void InitializeComponentFromCode()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Criptarea Merkle-Hellman";
            this.Size = new Size(1000, 600);

            lblBitLength = new Label { Text = "Lungime biți:", Location = new Point(10, 10), AutoSize = true };
            cmbBitLength = new ComboBox { Location = new Point(100, 10), Width = 100 };
            cmbBitLength.Items.AddRange(new object[] { 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096 });
            cmbBitLength.SelectedIndex = 0;

            btnGenerateKeys = new Button { Text = "Generează Chei", Location = new Point(210, 10), Width = 120 };
            btnGenerateKeys.Click += BtnGenerateKeys_Click;

            lblPrivateKey = new Label { Text = "Cheie Privată (W, q, r):", Location = new Point(10, 40), AutoSize = true };
            txtPrivateKey = new TextBox { Location = new Point(10, 60), Width = 960, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };

            lblPublicKey = new Label { Text = "Cheie Publică (B):", Location = new Point(10, 130), AutoSize = true };
            txtPublicKey = new TextBox { Location = new Point(10, 150), Width = 960, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };

            lblMessage = new Label { Text = "Mesaj (text):", Location = new Point(10, 220), AutoSize = true };
            txtMessage = new TextBox { Location = new Point(10, 240), Width = 960, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };

            btnEncrypt = new Button { Text = "Criptează", Location = new Point(10, 310), Width = 120 };
            btnEncrypt.Click += BtnEncrypt_Click;

            lblCiphertext = new Label { Text = "Text Criptat:", Location = new Point(10, 340), AutoSize = true };
            txtCiphertext = new TextBox { Location = new Point(10, 360), Width = 960, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };

            btnDecrypt = new Button { Text = "Decriptează", Location = new Point(10, 430), Width = 120 };
            btnDecrypt.Click += BtnDecrypt_Click;

            lblDecrypted = new Label { Text = "Mesaj Decriptat:", Location = new Point(10, 460), AutoSize = true };
            txtDecrypted = new TextBox { Location = new Point(10, 480), Width = 960, Height = 60, Multiline = true, ScrollBars = ScrollBars.Vertical };

            this.Controls.Add(lblBitLength);
            this.Controls.Add(cmbBitLength);
            this.Controls.Add(btnGenerateKeys);
            this.Controls.Add(lblPrivateKey);
            this.Controls.Add(txtPrivateKey);
            this.Controls.Add(lblPublicKey);
            this.Controls.Add(txtPublicKey);
            this.Controls.Add(lblMessage);
            this.Controls.Add(txtMessage);
            this.Controls.Add(btnEncrypt);
            this.Controls.Add(lblCiphertext);
            this.Controls.Add(txtCiphertext);
            this.Controls.Add(btnDecrypt);
            this.Controls.Add(lblDecrypted);
            this.Controls.Add(txtDecrypted);
        }

        private void BtnGenerateKeys_Click(object sender, EventArgs e)
        {
            int n = (int)cmbBitLength.SelectedItem;
            GenerateKeys(n);
            DisplayKeys();
        }

        private void GenerateKeys(int n)
        {
            Random rand = new Random();
            privateWeights = new List<BigInteger>(n);
            BigInteger sum = BigInteger.Zero;

            // Generare W supercrescator
            privateWeights.Add(new BigInteger(rand.Next(1, 11)));
            sum = privateWeights[0];
            for (int i = 2; i <= n; i++)
            {
                BigInteger next = sum * 2 + i;
                privateWeights.Add(next);
                sum += next;
            }

            // q > sum
            modulusQ = sum + BigInteger.One;

            // r coprim cu q
            do
            {
                multiplierR = new BigInteger(rand.Next(2, 10000));
            } while (BigInteger.GreatestCommonDivisor(multiplierR, modulusQ) != BigInteger.One);

            // invers modular
            inverseR = ModInverse(multiplierR, modulusQ);

            // cheie publica
            publicKey = new List<BigInteger>(n);
            foreach (var w in privateWeights)
            {
                publicKey.Add((w * multiplierR) % modulusQ);
            }
        }

        private void DisplayKeys()
        {
            txtPrivateKey.Text = $"W = {{ {string.Join(", ", privateWeights)} }}, q = {modulusQ}, r = {multiplierR}";
            txtPublicKey.Text = $"B = {{ {string.Join(", ", publicKey)} }}";
        }

        private void BtnEncrypt_Click(object sender, EventArgs e)
        {
            if (publicKey == null || publicKey.Count == 0)
            {
                MessageBox.Show("Vă rugăm să generați mai întâi cheile.");
                return;
            }

            string message = txtMessage.Text;
            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Vă rugăm să introduceți un mesaj pentru criptare.");
                return;
            }

            int n = (int)cmbBitLength.SelectedItem;
            int charsPerBlock = n / 8;
            List<BigInteger> ciphertexts = new List<BigInteger>();

            while (message.Length % charsPerBlock != 0)
            {
                message += " ";
            }

            for (int i = 0; i < message.Length; i += charsPerBlock)
            {
                BigInteger blockValue = BigInteger.Zero;
                for (int j = 0; j < charsPerBlock; j++)
                {
                    if (i + j < message.Length)
                    {
                        blockValue = (blockValue << 8) + (int)message[i + j];
                    }
                }

                string binary = ConvertToBinary(blockValue, n);
                BigInteger ciphertext = BigInteger.Zero;
                for (int k = 0; k < n; k++)
                {
                    if (binary[k] == '1')
                    {
                        ciphertext += publicKey[k];
                    }
                }
                ciphertexts.Add(ciphertext);
            }

            txtCiphertext.Text = string.Join(" ", ciphertexts);
        }

        private void BtnDecrypt_Click(object sender, EventArgs e)
        {
            if (privateWeights == null || privateWeights.Count == 0 || txtPrivateKey.Text == null)
            {
                MessageBox.Show("Vă rugăm să generați mai întâi cheile.");
                return;
            }
            
            string[] ciphertexts = txtCiphertext.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (ciphertexts.Length == 0)
            {
                MessageBox.Show("Vă rugăm să introduceți un text criptat valid.");
                return;
            }

            int n = (int)cmbBitLength.SelectedItem;
            int charsPerBlock = n / 8; 
            StringBuilder decryptedMessage = new StringBuilder();

            foreach (string cText in ciphertexts)
            {
                if (!BigInteger.TryParse(cText, out BigInteger c))
                {
                    MessageBox.Show("Format invalid al textului criptat.");
                    return;
                }

                // Calcul c' = c * r' mod q
                BigInteger cPrime = (c * inverseR) % modulusQ;
                if (cPrime < 0) cPrime += modulusQ;

                StringBuilder binary = new StringBuilder(new string('0', n));
                BigInteger remaining = cPrime;
                for (int i = privateWeights.Count - 1; i >= 0; i--)
                {
                    if (remaining >= privateWeights[i])
                    {
                        binary[i] = '1';
                        remaining -= privateWeights[i];
                    }
                }

                if (remaining != BigInteger.Zero)
                {
                    MessageBox.Show("Decriptare eșuată: sumă de submulțime invalidă.");
                    return;
                }

                BigInteger blockValue = BigInteger.Zero;
                BigInteger p = BigInteger.One;
                for (int i = n - 1; i >= 0; i--)
                {
                    if (binary[i] == '1')
                    {
                        blockValue += p;
                    }
                    p *= 2;
                }

                string blockChars = "";
                for (int j = 0; j < charsPerBlock; j++)
                {
                    BigInteger charValue = blockValue & 0xFF;
                    if (charValue > 0) 
                    {
                        blockChars = (char)charValue + blockChars;
                    }
                    blockValue >>= 8; 
                }
                decryptedMessage.Append(blockChars);
            }

            txtDecrypted.Text = decryptedMessage.ToString().TrimEnd();
        }

        private string ConvertToBinary(BigInteger value, int n)
        {
            StringBuilder binary = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                binary.Insert(0, (value & 1) == 0 ? '0' : '1');
                value >>= 1;
            }
            return binary.ToString();
        }

        private BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            BigInteger m0 = m;
            BigInteger y = BigInteger.Zero, x = BigInteger.One;

            if (m == BigInteger.One)
                return BigInteger.Zero;

            while (a > BigInteger.One)
            {
                BigInteger q = a / m;
                BigInteger t = m;
                m = a % m;
                a = t;
                t = y;
                y = x - q * y;
                x = t;
            }

            if (x < BigInteger.Zero)
                x += m0;

            return x;
        }

       
    }
}