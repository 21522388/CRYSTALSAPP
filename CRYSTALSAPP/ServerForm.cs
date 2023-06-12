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
using Org.BouncyCastle.Crypto.Generators;

namespace CRYSTALSAPP
{
    public partial class ServerForm : Form
    {
        DevConsole devConsole;

        public ServerForm()
        {
            InitializeComponent();
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

        const string CLIENT_JOIN_MSG = "New client connected from: ";
        const string CLIENT_LEAVE_MSG = " has disconnected!";
        const string DEFAULT_MESSAGE = "Hello Client! This is server, How are you?";

        const int DEFAULT_PORT = 8080;
        const int BUFFER_SIZE = 4096;
        List<TcpClient> userClients = new List<TcpClient>();

        int NONCE_LENGTH = AesGcm.NonceByteSizes.MaxSize;
        int TAG_LENGTH = AesGcm.TagByteSizes.MaxSize;
        const int SIGNATURE_LENGTH = 3293; // Dilithium3

        byte[] sessionKey;
        DilithiumPublicKeyParameters partnerKey;
        AsymmetricCipherKeyPair keyPair;

        bool kyber_exchanged, dilithium_exchanged = false;

        void exchangeKyber(SecureRandom rng, TcpClient userClient)
        {
            userClient.ReceiveBufferSize = BUFFER_SIZE;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesReceived;

            NetworkStream ns = userClient.GetStream();

            KyberKeyGenerationParameters keyGenParameters = new KyberKeyGenerationParameters(rng, KyberParameters.kyber768);

            KyberKeyPairGenerator keyPairGenerator = new KyberKeyPairGenerator();
            keyPairGenerator.Init(keyGenParameters);

            AsymmetricCipherKeyPair aliceKeyPair = keyPairGenerator.GenerateKeyPair();

            KyberPublicKeyParameters alicePublic = (KyberPublicKeyParameters)aliceKeyPair.Public;
            KyberPrivateKeyParameters alicePrivate = (KyberPrivateKeyParameters)aliceKeyPair.Private;

            byte[] publicEncoded = alicePublic.GetEncoded();
            byte[] privateEncoded = alicePrivate.GetEncoded();

            devConsole.Print("Generated KYBER Key Pair!");
            devConsole.Print("Public Key: " + PrettyPrint(publicEncoded));
            devConsole.Print("Private Key: " + PrettyPrint(privateEncoded));

            ns.Write(publicEncoded, 0, publicEncoded.Length);
            devConsole.Print("Sent Public key to Client!");

            devConsole.Print("Waiting for Client's encapsulated secret...");
            bytesReceived = ns.Read(buffer, 0, buffer.Length);

            devConsole.Print("Encapsulated secret received! Extracting session key...");
            var aliceKemExtractor = new KyberKemExtractor(alicePrivate);
            sessionKey = aliceKemExtractor.ExtractSecret(buffer[..bytesReceived]);
            devConsole.Print("Session key: " + PrettyPrint(sessionKey));

            devConsole.Print("Process complete!");
        }

        void exchangeDilithium(SecureRandom rng, TcpClient userClient)
        {
            userClient.ReceiveBufferSize = BUFFER_SIZE;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesReceived;

            NetworkStream ns = userClient.GetStream();

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
            devConsole.Print("Sent Public key to Client!");

            devConsole.Print("Waiting for Client's Public key...");
            bytesReceived = ns.Read(buffer, 0, buffer.Length);

            byte[] keyReceived = buffer[..bytesReceived];
            devConsole.Print("Server's Public key received: " + PrettyPrint(keyReceived));
            partnerKey = new DilithiumPublicKeyParameters(DilithiumParameters.Dilithium3, keyReceived);

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

        private void startServer()
        {
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, DEFAULT_PORT);
            ServerSocket.Start();
            devConsole.Print("Server is now listening to port: " + DEFAULT_PORT);

            while (true)
            {
                TcpClient userClient = ServerSocket.AcceptTcpClient();
                userClients.Add(userClient);
                devConsole.Print(CLIENT_JOIN_MSG + userClient.Client.RemoteEndPoint.ToString());

                Thread t = new Thread(() => handleClient(userClient));
                t.Start();
            }
        }
        void sendMessage(NetworkStream ns)
        {
            byte[] message = Encoding.UTF8.GetBytes(DEFAULT_MESSAGE);
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

        private void handleClient(TcpClient userClient)
        {
            devConsole.Print("=== CREATING SESSION ===");

            SecureRandom rng = new SecureRandom();
            // Perform CRYSTALS-Kyber
            devConsole.Print(">> Creating session key with CRYSTALS-Kyber:");
            exchangeKyber(rng, userClient);
            // Perform CRYSTALS-Dilithium
            devConsole.Print(">> Exchanging CRYSTALS-Dilithium keys:");
            exchangeDilithium(rng, userClient);

            devConsole.Print("=== SESSION CREATION COMPLETE! ===");

            userClient.ReceiveBufferSize = BUFFER_SIZE;
            byte[] buffer = new byte[BUFFER_SIZE];
            int bytesReceived;

            NetworkStream ns = userClient.GetStream();

            while (true)
            {
                bytesReceived = ns.Read(buffer, 0, buffer.Length);

                if (bytesReceived == 0) // User Disconnected
                {
                    break;
                }

                processMessage(buffer[..bytesReceived]);
            }

            userClients.Remove(userClient);
            devConsole.Print(userClient.Client.RemoteEndPoint.ToString() + CLIENT_LEAVE_MSG);
            userClient.Client.Shutdown(SocketShutdown.Both);
            userClient.Close();
        }

        private void ListenButton_Click(object sender, EventArgs e)
        {
            ListenButton.Enabled = false;

            CheckForIllegalCrossThreadCalls = false;

            Thread serverThread = new Thread(new ThreadStart(startServer));
            serverThread.Start();
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            foreach (TcpClient userClient in userClients)
            {
                sendMessage(userClient.GetStream());
            }
        }
    }
}
