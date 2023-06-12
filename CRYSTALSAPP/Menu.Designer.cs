namespace CRYSTALSAPP
{
    partial class Menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ServerButton = new Button();
            ClientButton = new Button();
            SuspendLayout();
            // 
            // ServerButton
            // 
            ServerButton.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point);
            ServerButton.ForeColor = Color.DarkViolet;
            ServerButton.Location = new Point(12, 12);
            ServerButton.Name = "ServerButton";
            ServerButton.Size = new Size(260, 56);
            ServerButton.TabIndex = 0;
            ServerButton.Text = "SERVER";
            ServerButton.UseVisualStyleBackColor = true;
            ServerButton.Click += ServerButton_Click;
            // 
            // ClientButton
            // 
            ClientButton.Font = new Font("Segoe UI", 21.75F, FontStyle.Bold, GraphicsUnit.Point);
            ClientButton.ForeColor = Color.RoyalBlue;
            ClientButton.Location = new Point(12, 74);
            ClientButton.Name = "ClientButton";
            ClientButton.Size = new Size(260, 56);
            ClientButton.TabIndex = 1;
            ClientButton.Text = "CLIENT";
            ClientButton.UseVisualStyleBackColor = true;
            ClientButton.Click += ClientButton_Click;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(284, 161);
            Controls.Add(ClientButton);
            Controls.Add(ServerButton);
            Name = "Menu";
            Text = "Menu";
            ResumeLayout(false);
        }

        #endregion

        private Button ServerButton;
        private Button ClientButton;
    }
}