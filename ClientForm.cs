using Org.BouncyCastle.Pqc.Crypto.Crystals.Kyber;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Org.BouncyCastle.Utilities.Net;
using IPAddress = System.Net.IPAddress;
using Org.BouncyCastle.Crypto;
using System.CodeDom;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CRYSTALSAPP
{
    public partial class ClientForm : Form
    {
        DevConsole devConsole;

        public ClientForm()
        {
            InitializeComponent();
            SendButton.Enabled = false; // Default Behaviour
            devConsole = new DevConsole();
            devConsole.Show();
        }

        static string PrettyPrint(byte[] bytes)
        {
            var base64 = Convert.ToBase64String(bytes);
            if (base64.Length > 50)
                return $"{base64[..25]}...{base64[^25..]}";

            return base64;
        }

        const string CURRENCY_SUFFIX = "VND";
        const int DEFAULT_PORT = 8080;
        const int BUFFER_SIZE = 4096;
        TcpClient client;
        NetworkStream ns;

        int NONCE_LENGTH = AesGcm.NonceByteSizes.MaxSize;
        int TAG_LENGTH = AesGcm.TagByteSizes.MaxSize;

        byte[] sharedSecret;
        void exchangeKyber(SecureRandom rng)
        {
            client.ReceiveBufferSize = BUFFER_SIZE;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesReceived;

            devConsole.Print("Waiting for Server's Public key...");
            bytesReceived = ns.Read(buffer, 0, buffer.Length);

            byte[] keyReceived = buffer[..bytesReceived];
            // Add Key Received verification here!
            devConsole.Print("Server's Public key received: " + PrettyPrint(keyReceived));
            KyberPublicKeyParameters alicePublic = new KyberPublicKeyParameters(KyberParameters.kyber768, keyReceived);

            devConsole.Print("Generating session key...");
            KyberKemGenerator bobKyberKemGenerator = new KyberKemGenerator(rng);
            ISecretWithEncapsulation encapsulatedSecret = bobKyberKemGenerator.GenerateEncapsulated(alicePublic);
            sharedSecret = encapsulatedSecret.GetSecret();
            devConsole.Print("Session key: " + PrettyPrint(sharedSecret));

            byte[] cipherText = encapsulatedSecret.GetEncapsulation();
            ns.Write(cipherText, 0, cipherText.Length);
            devConsole.Print("Sent Encapsulated Secret key to Server!");

            devConsole.Print("Process complete!");
        }

        private byte[] encryptMessage(byte[] plaintext)
        {
            AesGcm aes = new AesGcm(sharedSecret);

            byte[] nonce = new byte[NONCE_LENGTH];
            byte[] tag = new byte[TAG_LENGTH];
            RandomNumberGenerator.Fill(nonce);

            byte[] ciphertext = new byte[plaintext.Length];
            aes.Encrypt(nonce, plaintext, ciphertext, tag);

            byte[] payload = new byte[nonce.Length + ciphertext.Length + tag.Length];
            Buffer.BlockCopy(nonce, 0, payload, 0, nonce.Length);
            Buffer.BlockCopy(ciphertext, 0, payload, nonce.Length, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, payload, nonce.Length + ciphertext.Length, tag.Length);

            return payload;
        }

        private byte[] decryptMessage(byte[] encrypted)
        {
            AesGcm aes = new AesGcm(sharedSecret);

            byte[] recv_nonce = new byte[NONCE_LENGTH];
            Buffer.BlockCopy(encrypted, 0, recv_nonce, 0, NONCE_LENGTH);

            byte[] recv_tag = new byte[TAG_LENGTH];
            Buffer.BlockCopy(encrypted, encrypted.Length - TAG_LENGTH, recv_tag, 0, TAG_LENGTH);

            byte[] ciphertext = new byte[encrypted.Length - NONCE_LENGTH - TAG_LENGTH];
            Buffer.BlockCopy(encrypted, NONCE_LENGTH, ciphertext, 0, ciphertext.Length);

            byte[] plaintext = new byte[ciphertext.Length];

            try
            {
                aes.Decrypt(recv_nonce, encrypted[NONCE_LENGTH..^TAG_LENGTH], recv_tag, plaintext);
                return plaintext; // Decryption Success
            }
            catch (CryptographicException) // Decryption Failed
            {
                return new byte[0];
            }
        }

        private void initConnection(IPAddress ipAddress)
        {
            client = new TcpClient();

            IPEndPoint iPEndPoint = new IPEndPoint(ipAddress, DEFAULT_PORT);
            client.Connect(iPEndPoint);

            ns = client.GetStream();

            Thread thread = new Thread(new ThreadStart(receiveMessage));
            thread.Start();
        }

        void sendMessage(byte[] message)
        {
            byte[] payload = encryptMessage(message);
            ns.Write(payload, 0, payload.Length);
        }

        void processMessage(byte[] payload)
        {
            devConsole.Print(">> Message received: " + PrettyPrint(payload));
            try
            {
                // Decrypt using AES 256 GCM
                byte[] plaintext = decryptMessage(payload);
                devConsole.Print(Encoding.UTF8.GetString(plaintext));
            }
            catch (Exception ex)
            {
                devConsole.Print("Failed! >> " + ex.Message);
            }
        }

        void receiveMessage()
        {
            devConsole.Print("=== CREATING SESSION ===");

            SecureRandom rng = new SecureRandom();
            // Perform CRYSTALS-Kyber
            exchangeKyber(rng);

            devConsole.Print("=== SESSION CREATION COMPLETE! ===");

            // Wait for Menu
            List<MenuItem> menuItems = waitForMenu();
            // Create Menu on List
            createMenu(menuItems);
            SendButton.Enabled = true;

            client.ReceiveBufferSize = BUFFER_SIZE;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesReceived;

            while (true)
            {
                try
                {
                    bytesReceived = ns.Read(buffer, 0, buffer.Length);

                    if (bytesReceived > 0)
                    {
                        processMessage(buffer[..bytesReceived]);
                    }
                }
                catch (Exception ex)
                {
                    devConsole.Print("Failed >> Exception: " + ex.Message);
                    break;
                }
            }
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            IPAddress ipAddress;
            if (!IPAddress.TryParse(AddressBox.Text, out ipAddress))
            {
                MessageBox.Show("Invalid IP Address!", "Action failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                devConsole.Print("Initializing TCP Connection...");
                initConnection(ipAddress);
                devConsole.Print("Connected to server successfully!");

                SecureRandom rng = new SecureRandom();
                devConsole.Print("Exchanging CRYSTALS-Kyber keys...");
            }
            catch (Exception ex)
            {
                devConsole.Print("Failed >> Exception: " + ex.Message);
            }
        }

        private byte[] createMessage()
        {
            List<OrderItem> orderItems = new List<OrderItem>();
            foreach (ListViewItem item in OrderView.Items)
            {
                if (Int32.Parse(item.SubItems[2].Text) <= 0) continue;
                orderItems.Add(new OrderItem
                {
                    ID = Int32.Parse(item.SubItems[0].Text),
                    Amount = Int32.Parse(item.SubItems[2].Text),
                });
            }

            OrderRequest orderRequest = new OrderRequest();
            orderRequest.CustomerName = NameBox.Text;
            orderRequest.DeliveryAddress = DeliveryAddressBox.Text;
            orderRequest.OrderItems = orderItems;

            return JsonSerializer.SerializeToUtf8Bytes(orderRequest);
        }

        int TOTAL_PAYMENT = 0;

        private void updateTotal()
        {
            TotalLabel.Text = TOTAL_PAYMENT.ToString() + ' ' + CURRENCY_SUFFIX;
        }

        private List<MenuItem> waitForMenu()
        {
            client.ReceiveBufferSize = BUFFER_SIZE;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesReceived;

            devConsole.Print("Waiting for Server to send Menu...");
            bytesReceived = ns.Read(buffer, 0, buffer.Length);

            devConsole.Print("Menu Received: " + PrettyPrint(buffer[..bytesReceived]));
            
            byte[] plaintext = decryptMessage(buffer[..bytesReceived]);
            devConsole.Print(Encoding.UTF8.GetString(plaintext));
            return JsonSerializer.Deserialize<List<MenuItem>>(plaintext);
        }

        private void createMenu(List<MenuItem> menuItems)
        {
            foreach (var menuItem in menuItems)
            {
                var item = new ListViewItem(menuItem.ID.ToString());
                item.SubItems.Add(menuItem.ItemName);
                item.SubItems.Add(menuItem.Price.ToString());
                item.SubItems.Add(menuItem.Available.ToString());
                item.SubItems.Add(menuItem.Amount.ToString());

                var panel = new Panel();
                panel.Size = new Size(50, 20);

                var plusButton = new Button();
                plusButton.Text = "+";
                plusButton.Location = new Point(0, 0);
                plusButton.Size = new Size(20, 20);
                plusButton.Click += (sender, e) =>
                {
                    menuItem.Amount++;
                    item.SubItems[4].Text = menuItem.Amount.ToString();
                    TOTAL_PAYMENT += menuItem.Price;
                    updateTotal();
                };
                panel.Controls.Add(plusButton);
                plusButton.Enabled = menuItem.Available;

                var minusButton = new Button();
                minusButton.Text = "-";
                minusButton.Location = new Point(30, 0);
                minusButton.Size = new Size(20, 20);
                minusButton.Click += (sender, e) =>
                {
                    if (menuItem.Amount > 0)
                    {
                        menuItem.Amount--;
                        item.SubItems[4].Text = menuItem.Amount.ToString();
                        TOTAL_PAYMENT -= menuItem.Price;
                        updateTotal();
                    }
                };
                panel.Controls.Add(minusButton);
                minusButton.Enabled = menuItem.Available;

                item.Tag = panel;

                OrderView.Items.Add(item);
            }

            OrderView.ItemSelectionChanged += (sender, e) =>
            {
                if (e.IsSelected)
                {
                    var panel = (Panel)e.Item.Tag;
                    panel.Visible = true;
                    panel.Location = new Point(e.Item.Bounds.Right - panel.Width + 5, e.Item.Bounds.Top + (e.Item.Bounds.Height - panel.Height) / 2);
                    OrderView.Controls.Add(panel);
                    panel.BringToFront();
                    OrderView.Focus();
                }
                else
                {
                    foreach (ListViewItem item in OrderView.Items)
                    {
                        var panel = (Panel)item.Tag;
                        panel.Visible = false;
                        OrderView.Controls.Remove(panel);
                    }
                }
            };
            Controls.Add(OrderView);
        }

        bool debounce = false;
        private void SendButton_Click(object sender, EventArgs e)
        {
            if (debounce) return;
            debounce = true;

            byte[] message = createMessage();
            sendMessage(message);

            debounce = false;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            OrderView.Items.Clear();
        }
    }
}