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
            Food = new ColumnHeader();
            Amount = new ColumnHeader();
            AddChickenButton = new Button();
            AddFriesButton = new Button();
            AddPepsiButton = new Button();
            ClearButton = new Button();
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
            SendButton.Location = new Point(12, 303);
            SendButton.Name = "SendButton";
            SendButton.Size = new Size(184, 46);
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
            // OrderView
            // 
            OrderView.Columns.AddRange(new ColumnHeader[] { Food, Amount });
            OrderView.Location = new Point(12, 63);
            OrderView.Name = "OrderView";
            OrderView.Size = new Size(373, 241);
            OrderView.TabIndex = 7;
            OrderView.UseCompatibleStateImageBehavior = false;
            OrderView.View = View.Details;
            // 
            // Food
            // 
            Food.Text = "Food";
            Food.Width = 260;
            // 
            // Amount
            // 
            Amount.Text = "Amount";
            // 
            // AddChickenButton
            // 
            AddChickenButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            AddChickenButton.Location = new Point(391, 63);
            AddChickenButton.Name = "AddChickenButton";
            AddChickenButton.Size = new Size(181, 48);
            AddChickenButton.TabIndex = 8;
            AddChickenButton.Text = "+1 Chicken";
            AddChickenButton.TextAlign = ContentAlignment.MiddleLeft;
            AddChickenButton.UseVisualStyleBackColor = true;
            AddChickenButton.Click += AddChickenButton_Click;
            // 
            // AddFriesButton
            // 
            AddFriesButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            AddFriesButton.Location = new Point(391, 117);
            AddFriesButton.Name = "AddFriesButton";
            AddFriesButton.Size = new Size(181, 48);
            AddFriesButton.TabIndex = 9;
            AddFriesButton.Text = "+1 French Fries";
            AddFriesButton.TextAlign = ContentAlignment.MiddleLeft;
            AddFriesButton.UseVisualStyleBackColor = true;
            AddFriesButton.Click += AddFriesButton_Click;
            // 
            // AddPepsiButton
            // 
            AddPepsiButton.Font = new Font("Segoe UI Semibold", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            AddPepsiButton.Location = new Point(391, 171);
            AddPepsiButton.Name = "AddPepsiButton";
            AddPepsiButton.Size = new Size(181, 48);
            AddPepsiButton.TabIndex = 10;
            AddPepsiButton.Text = "+1 Pepsi";
            AddPepsiButton.TextAlign = ContentAlignment.MiddleLeft;
            AddPepsiButton.UseVisualStyleBackColor = true;
            AddPepsiButton.Click += AddPepsiButton_Click;
            // 
            // ClearButton
            // 
            ClearButton.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point);
            ClearButton.ForeColor = Color.Red;
            ClearButton.Location = new Point(202, 303);
            ClearButton.Name = "ClearButton";
            ClearButton.Size = new Size(183, 46);
            ClearButton.TabIndex = 11;
            ClearButton.Text = "CLEAR";
            ClearButton.UseVisualStyleBackColor = true;
            ClearButton.Click += ClearButton_Click;
            // 
            // ClientForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 361);
            Controls.Add(ClearButton);
            Controls.Add(AddPepsiButton);
            Controls.Add(AddFriesButton);
            Controls.Add(AddChickenButton);
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
        private ColumnHeader Food;
        private Button AddChickenButton;
        private Button AddFriesButton;
        private Button AddPepsiButton;
        private ColumnHeader Amount;
        private Button ClearButton;
    }
}