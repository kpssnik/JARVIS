namespace kpssJarvis
{
    partial class Form1
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
            this.startButton = new MetroFramework.Controls.MetroButton();
            this.settingsButton = new MetroFramework.Controls.MetroButton();
            this.metroProgressBar1 = new MetroFramework.Controls.MetroProgressBar();
            this.SuspendLayout();
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(23, 86);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(220, 23);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "START";
            this.startButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.Location = new System.Drawing.Point(23, 161);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(220, 23);
            this.settingsButton.TabIndex = 1;
            this.settingsButton.Text = "SETTINGS";
            this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);
            // 
            // metroProgressBar1
            // 
            this.metroProgressBar1.Location = new System.Drawing.Point(23, 115);
            this.metroProgressBar1.Name = "metroProgressBar1";
            this.metroProgressBar1.Size = new System.Drawing.Size(220, 23);
            this.metroProgressBar1.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 437);
            this.Controls.Add(this.metroProgressBar1);
            this.Controls.Add(this.settingsButton);
            this.Controls.Add(this.startButton);
            this.Name = "Form1";
            this.Resizable = false;
            this.Text = "Jarvis";
            this.TextAlign = System.Windows.Forms.VisualStyles.HorizontalAlign.Center;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroButton startButton;
        private MetroFramework.Controls.MetroButton settingsButton;
        private MetroFramework.Controls.MetroProgressBar metroProgressBar1;
    }
}

