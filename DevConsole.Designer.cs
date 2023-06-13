namespace CRYSTALSAPP
{
    partial class DevConsole
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
            PrintView = new ListView();
            ConsolePrint = new ColumnHeader();
            SuspendLayout();
            // 
            // PrintView
            // 
            PrintView.BackColor = Color.FromArgb(16, 17, 28);
            PrintView.BorderStyle = BorderStyle.None;
            PrintView.Columns.AddRange(new ColumnHeader[] { ConsolePrint });
            PrintView.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            PrintView.ForeColor = Color.White;
            PrintView.Location = new Point(12, 12);
            PrintView.Name = "PrintView";
            PrintView.Size = new Size(760, 537);
            PrintView.TabIndex = 0;
            PrintView.UseCompatibleStateImageBehavior = false;
            PrintView.View = View.Details;
            // 
            // ConsolePrint
            // 
            ConsolePrint.Text = "Output";
            ConsolePrint.Width = 755;
            // 
            // DevConsole
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(16, 17, 28);
            ClientSize = new Size(784, 561);
            Controls.Add(PrintView);
            Name = "DevConsole";
            Text = "DevConsole";
            ResumeLayout(false);
        }

        #endregion

        private ListView PrintView;
        private ColumnHeader ConsolePrint;
    }
}