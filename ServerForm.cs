﻿using Org.BouncyCastle.Pqc.Crypto.Crystals.Kyber;
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
using System.Text.Json;
using Org.BouncyCastle.Utilities;

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

        byte[] sharedSecret;

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

            // Add Key Signing with X.509 here!
            ns.Write(publicEncoded, 0, publicEncoded.Length);
            devConsole.Print("Sent Public key to Client!");

            devConsole.Print("Waiting for Client's encapsulated secret...");
            bytesReceived = ns.Read(buffer, 0, buffer.Length);

            devConsole.Print("Encapsulated secret received! Extracting session key...");
            KyberKemExtractor aliceKemExtractor = new KyberKemExtractor(alicePrivate);
            sharedSecret = aliceKemExtractor.ExtractSecret(buffer[..bytesReceived]);
            devConsole.Print("Session key: " + PrettyPrint(sharedSecret));

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
            byte[] payload = encryptMessage(Encoding.UTF8.GetBytes(DEFAULT_MESSAGE));
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

        private void handleClient(TcpClient userClient)
        {
            devConsole.Print("=== CREATING SESSION ===");

            SecureRandom rng = new SecureRandom();
            // Perform CRYSTALS-Kyber
            exchangeKyber(rng, userClient);

            devConsole.Print("=== SESSION CREATION COMPLETE! ===");

            // Send Menu to Client
            sendMenu(userClient);

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

        private void sendMenu(TcpClient userClient)
        {
            byte[] payload = encryptMessage(File.ReadAllBytes("menu.json"));

            userClient.ReceiveBufferSize = BUFFER_SIZE;
            int bytesReceived;

            NetworkStream ns = userClient.GetStream();

            ns.Write(payload, 0, payload.Length);
            devConsole.Print("Menu sent to Client!");
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
