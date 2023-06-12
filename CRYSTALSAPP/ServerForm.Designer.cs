namespace CRYSTALSAPP
{
    partial class ServerForm
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
            ListenButton = new Button();
            SendButton = new Button();
            SuspendLayout();
            // 
            // ListenButton
            // 
            ListenButton.Location = new Point(238, 12);
            ListenButton.Name = "ListenButton";
            ListenButton.Size = new Size(108, 25);
            ListenButton.TabIndex = 1;
            ListenButton.Text = "LISTEN";
            ListenButton.UseVisualStyleBackColor = true;
            ListenButton.Click += ListenButton_Click;
            // 
            // SendButton
            // 
            SendButton.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            SendButton.Location = new Point(164, 106);
            SendButton.Name = "SendButton";
            SendButton.Size = new Size(256, 68);
            SendButton.TabIndex = 2;
            SendButton.Text = "SAY HI TO CLIENTS";
            SendButton.UseVisualStyleBackColor = true;
            SendButton.Click += SendButton_Click;
            // 
            // ServerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 361);
            Controls.Add(SendButton);
            Controls.Add(ListenButton);
            Name = "ServerForm";
            Text = "SERVER";
            ResumeLayout(false);
        }

        #endregion

        private Button ListenButton;
        private Button SendButton;
    }
}