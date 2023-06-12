namespace CRYSTALSAPP
{
    partial class ClientForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ConnectButton = new Button();
            label1 = new Label();
            OrderBox = new RichTextBox();
            AddressBox = new TextBox();
            SendButton = new Button();
            label2 = new Label();
            NameBox = new TextBox();
            SuspendLayout();
            // 
            // ConnectButton
            // 
            ConnectButton.Location = new Point(464, 7);
            ConnectButton.Name = "ConnectButton";
            ConnectButton.Size = new Size(108, 25);
            ConnectButton.TabIndex = 0;
            ConnectButton.Text = "CONNECT";
            ConnectButton.UseVisualStyleBackColor = true;
            ConnectButton.Click += ConnectButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(116, 25);
            label1.TabIndex = 1;
            label1.Text = "IP ADDRESS:";
            // 
            // OrderBox
            // 
            OrderBox.Location = new Point(12, 64);
            OrderBox.Name = "OrderBox";
            OrderBox.Size = new Size(560, 233);
            OrderBox.TabIndex = 2;
            OrderBox.Text = "";
            // 
            // AddressBox
            // 
            AddressBox.Location = new Point(202, 6);
            AddressBox.Name = "AddressBox";
            AddressBox.Size = new Size(252, 23);
            AddressBox.TabIndex = 3;
            // 
            // SendButton
            // 
            SendButton.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            SendButton.Location = new Point(12, 303);
            SendButton.Name = "SendButton";
            SendButton.Size = new Size(187, 46);
            SendButton.TabIndex = 4;
            SendButton.Text = "SEND";
            SendButton.UseVisualStyleBackColor = true;
            SendButton.Click += SendButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(12, 35);
            label2.Name = "label2";
            label2.Size = new Size(184, 25);
            label2.TabIndex = 5;
            label2.Text = "CUSTOMER'S NAME:";
            // 
            // NameBox
            // 
            NameBox.Location = new Point(202, 35);
            NameBox.Name = "NameBox";
            NameBox.Size = new Size(252, 23);
            NameBox.TabIndex = 6;
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 361);
            Controls.Add(NameBox);
            Controls.Add(label2);
            Controls.Add(SendButton);
            Controls.Add(AddressBox);
            Controls.Add(OrderBox);
            Controls.Add(label1);
            Controls.Add(ConnectButton);
            Name = "ClientForm";
            Text = "ClientForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button ConnectButton;
        private Label label1;
        private RichTextBox OrderBox;
        private TextBox AddressBox;
        private Button SendButton;
        private Label label2;
        private TextBox NameBox;
    }
}