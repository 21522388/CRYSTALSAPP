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
            AddressBox = new TextBox();
            SendButton = new Button();
            label2 = new Label();
            NameBox = new TextBox();
            OrderView = new ListView();
            ID = new ColumnHeader();
            ItemName = new ColumnHeader();
            Price = new ColumnHeader();
            Available = new ColumnHeader();
            ClearButton = new Button();
            DeliveryAddressBox = new RichTextBox();
            label3 = new Label();
            label4 = new Label();
            TotalLabel = new Label();
            Amount = new ColumnHeader();
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
            SendButton.ForeColor = Color.FromArgb(0, 192, 0);
            SendButton.Location = new Point(500, 403);
            SendButton.Name = "SendButton";
            SendButton.Size = new Size(272, 46);
            SendButton.TabIndex = 4;
            SendButton.Text = "PLACE ORDER";
            SendButton.UseVisualStyleBackColor = true;
            SendButton.Click += SendButton_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(391, 63);
            label2.Name = "label2";
            label2.Size = new Size(184, 25);
            label2.TabIndex = 5;
            label2.Text = "CUSTOMER'S NAME:";
            // 
            // NameBox
            // 
            NameBox.Location = new Point(391, 91);
            NameBox.Name = "NameBox";
            NameBox.Size = new Size(381, 23);
            NameBox.TabIndex = 6;
            // 
            // OrderView
            // 
            OrderView.Columns.AddRange(new ColumnHeader[] { ID, ItemName, Price, Available, Amount });
            OrderView.Location = new Point(12, 63);
            OrderView.Name = "OrderView";
            OrderView.Size = new Size(373, 334);
            OrderView.TabIndex = 7;
            OrderView.UseCompatibleStateImageBehavior = false;
            OrderView.View = View.Details;
            // 
            // ID
            // 
            ID.Text = "ID";
            ID.Width = 40;
            // 
            // ItemName
            // 
            ItemName.Text = "ItemName";
            ItemName.Width = 120;
            // 
            // Price
            // 
            Price.Text = "Price";
            Price.Width = 80;
            // 
            // Available
            // 
            Available.Text = "Available";
            // 
            // ClearButton
            // 
            ClearButton.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            ClearButton.ForeColor = Color.Red;
            ClearButton.Location = new Point(202, 403);
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(183, 46);
            ClearButton.TabIndex = 11;
            ClearButton.Text = "CLEAR";
            ClearButton.UseVisualStyleBackColor = true;
            ClearButton.Click += ClearButton_Click;
            // 
            // DeliveryAddressBox
            // 
            DeliveryAddressBox.Location = new Point(391, 145);
            DeliveryAddressBox.Name = "DeliveryAddressBox";
            DeliveryAddressBox.Size = new Size(381, 87);
            DeliveryAddressBox.TabIndex = 12;
            DeliveryAddressBox.Text = "";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(391, 117);
            label3.Name = "label3";
            label3.Size = new Size(181, 25);
            label3.TabIndex = 13;
            label3.Text = "DELIVERY ADDRESS:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(391, 332);
            label4.Name = "label4";
            label4.Size = new Size(68, 25);
            label4.TabIndex = 14;
            label4.Text = "TOTAL:";
            // 
            // TotalLabel
            // 
            TotalLabel.AutoSize = true;
            TotalLabel.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point);
            TotalLabel.Location = new Point(391, 357);
            TotalLabel.Name = "TotalLabel";
            TotalLabel.Size = new Size(105, 40);
            TotalLabel.TabIndex = 15;
            TotalLabel.Text = "0 VND";
            // 
            // Amount
            // 
            Amount.Text = "Amount";
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(TotalLabel);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(DeliveryAddressBox);
            Controls.Add(ClearButton);
            Controls.Add(OrderView);
            Controls.Add(NameBox);
            Controls.Add(label2);
            Controls.Add(SendButton);
            Controls.Add(AddressBox);
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
        private TextBox AddressBox;
        private Button SendButton;
        private Label label2;
        private TextBox NameBox;
        private ListView OrderView;
        private Button ClearButton;
        private RichTextBox DeliveryAddressBox;
        private Label label3;
        private ColumnHeader ID;
        private ColumnHeader ItemName;
        private ColumnHeader Price;
        private ColumnHeader Available;
        private Label label4;
        private Label TotalLabel;
        private ColumnHeader Amount;
    }
}