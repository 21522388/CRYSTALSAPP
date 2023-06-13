using Org.BouncyCastle.Pqc.Crypto.Crystals.Dilithium;
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

        const int DEFAULT_PORT = 8080;
        const int BUFFER_SIZE = 4096;
        TcpClient client;
        NetworkStream ns;

        int NONCE_LENGTH = AesGcm.NonceByteSizes.MaxSize;
        int TAG_LENGTH = AesGcm.TagByteSizes.MaxSize;
        const int SIGNATURE_LENGTH = 3293; // Dilithium3

        byte[] sessionKey;
        DilithiumPublicKeyParameters partnerKey;
        AsymmetricCipherKeyPair keyPair;

        bool kyber_exchanged, dilithium_exchanged = false;

        void exchangeKyber(SecureRandom rng)
        {
            client.ReceiveBufferSize = BUFFER_SIZE;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesReceived;

            devConsole.Print("Waiting for Server's Public key...");
            bytesReceived = ns.Read(buffer, 0, buffer.Length);

            byte[] keyReceived = buffer[..bytesReceived];
            devConsole.Print("Server's Public key received: " + PrettyPrint(keyReceived));

            KyberPublicKeyParameters alicePublic = new KyberPublicKeyParameters(KyberParameters.kyber768, keyReceived);

            devConsole.Print("Generating session key...");
            KyberKemGenerator bobKyberKemGenerator = new KyberKemGenerator(rng);
            ISecretWithEncapsulation encapsulatedSecret = bobKyberKemGenerator.GenerateEncapsulated(alicePublic);
            sessionKey = encapsulatedSecret.GetSecret();
            devConsole.Print("Session key: " + PrettyPrint(sessionKey));

            byte[] cipherText = encapsulatedSecret.GetEncapsulation();
            ns.Write(cipherText, 0, cipherText.Length);
            devConsole.Print("Sent Encapsulated Secret key to Server!");

            devConsole.Print("Process complete!");
        }

        void exchangeDilithium(SecureRandom rng)
        {
            client.ReceiveBufferSize = BUFFER_SIZE;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesReceived;

            devConsole.Print("Waiting for Server's Public key...");
            bytesReceived = ns.Read(buffer, 0, buffer.Length);

            byte[] keyReceived = buffer[..bytesReceived];
            devConsole.Print("Server's Public key received: " + PrettyPrint(keyReceived));
            partnerKey = new DilithiumPublicKeyParameters(DilithiumParameters.Dilithium3, keyReceived);

            DilithiumKeyGenerationParameters kgp = new DilithiumKeyGenerationParameters(rng, DilithiumParameters.Dilithium3);
            DilithiumKeyPairGenerator dilithiumKeyPairGenerator = new DilithiumKeyPairGenerator();

            dilithiumKeyPairGenerator.Init(kgp);
            keyPair = dilithiumKeyPairGenerator.GenerateKeyPair();

            DilithiumPublicKeyParameters myPublic = (DilithiumPublicKeyParameters)keyPair.Public;
            DilithiumPrivateKeyParameters myPrivate = (DilithiumPrivateKeyParameters)keyPair.Private;

            byte[] publicEncoded = myPublic.GetEncoded();
            byte[] privateEncoded = myPrivate.GetEncoded();

            devConsole.Print("Generated DILITHIUM Key Pair!");
            devConsole.Print("Public Key: " + PrettyPrint(publicEncoded));
            devConsole.Print("Private Key: " + PrettyPrint(privateEncoded));

            ns.Write(publicEncoded, 0, publicEncoded.Length);
            devConsole.Print("Sent Public key to Server!");

            devConsole.Print("Process complete!");
        }

        private byte[] encryptMessage(byte[] plaintext)
        {
            AesGcm aes = new AesGcm(sessionKey);

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
            AesGcm aes = new AesGcm(sessionKey);

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

        private byte[] generateSignature(byte[] message)
        {
            DilithiumSigner signer = new DilithiumSigner();
            signer.Init(true, keyPair.Private);
            return signer.GenerateSignature(message);
        }

        private bool verifyMessage(byte[] message, byte[] signature)
        {
            DilithiumSigner signer = new DilithiumSigner();
            signer.Init(false, partnerKey);
            return signer.VerifySignature(message, signature);
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
            byte[] encrypted = encryptMessage(message);

            byte[] signature = generateSignature(encrypted);
            devConsole.Print("Generated Signature: " + PrettyPrint(signature));

            byte[] payload = new byte[encrypted.Length + signature.Length];
            Buffer.BlockCopy(encrypted, 0, payload, 0, encrypted.Length);
            Buffer.BlockCopy(signature, 0, payload, encrypted.Length, signature.Length);

            ns.Write(payload, 0, payload.Length);
        }

        void processMessage(byte[] payload)
        {
            devConsole.Print(">> Message received: " + PrettyPrint(payload));
            try
            {
                // Extract encrypted data and signature
                byte[] encrypted = new byte[payload.Length - SIGNATURE_LENGTH];
                byte[] signature = new byte[SIGNATURE_LENGTH];

                Buffer.BlockCopy(payload, 0, encrypted, 0, encrypted.Length);
                Buffer.BlockCopy(payload, encrypted.Length, signature, 0, signature.Length);

                // RandomNumberGenerator.Fill(signature); SIMULATE TAMPERED SIGNATURE
                devConsole.Print("Received Signature: " + PrettyPrint(signature));

                // Verify signature
                bool result = verifyMessage(encrypted, signature);
                if (!result)
                {
                    devConsole.Print("Failed! >> Invalid Signature");
                    return;
                }
                devConsole.Print("Signature Verified!");

                // Decrypt using AES 256 GCM
                byte[] plaintext = decryptMessage(encrypted);
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
            // Perform CRYSTALS-Dilithium
            devConsole.Print(">> Exchanging CRYSTALS-Dilithium keys:");
            exchangeDilithium(rng);
            // Perform CRYSTALS-Kyber
            devConsole.Print(">> Creating session key with CRYSTALS-Kyber:");
            exchangeKyber(rng);

            devConsole.Print("=== SESSION CREATION COMPLETE! ===");
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
            List<FoodItem> foodItems = new List<FoodItem>();
            foreach (ListViewItem item in OrderView.Items)
            {
                foodItems.Add(new FoodItem
                {
                    Name = item.SubItems[0].Text,
                    Amount = item.SubItems[1].Text,
                });
            }

            string jsonString = JsonSerializer.Serialize(foodItems);

            return Encoding.UTF8.GetBytes(NameBox.Text + ';' + jsonString);
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

        private void addItem(string itemName)
        {
            foreach (ListViewItem item in OrderView.Items)
            {
                if (item.SubItems[0].Text == itemName)
                {
                    item.SubItems[1].Text = (int.Parse(item.SubItems[1].Text) + 1).ToString();
                    return;
                }
            }
            OrderView.Items.Add(new ListViewItem(new[] { itemName, "1" }));
        }

        private void AddChickenButton_Click(object sender, EventArgs e)
        {
            addItem("Chicken");
        }

        private void AddFriesButton_Click(object sender, EventArgs e)
        {
            addItem("French Fries");
        }

        private void AddPepsiButton_Click(object sender, EventArgs e)
        {
            addItem("Pepsi");
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            OrderView.Items.Clear();
        }
    }
}