using System;
using PlayFab;

namespace UnicornBattleInstaller
{
    partial class InstallForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InstallForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.InstallButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Titles = new System.Windows.Forms.ComboBox();
            this.InstallProgress = new System.Windows.Forms.ProgressBar();
            this.InstallProgressText = new System.Windows.Forms.TextBox();
            this.SkipCDN = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.InitialImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.InitialImage")));
            this.pictureBox1.Location = new System.Drawing.Point(22, 25);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1723, 1080);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // InstallButton
            // 
            this.InstallButton.Enabled = false;
            this.InstallButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 35F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstallButton.Location = new System.Drawing.Point(1378, 1165);
            this.InstallButton.Name = "InstallButton";
            this.InstallButton.Size = new System.Drawing.Size(367, 137);
            this.InstallButton.TabIndex = 1;
            this.InstallButton.Text = "Install";
            this.InstallButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 27F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(39, 1129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(452, 82);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select a Title";
            // 
            // Titles
            // 
            this.Titles.Font = new System.Drawing.Font("Microsoft Sans Serif", 27F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Titles.FormattingEnabled = true;
            this.Titles.Location = new System.Drawing.Point(53, 1214);
            this.Titles.Name = "Titles";
            this.Titles.Size = new System.Drawing.Size(1239, 90);
            this.Titles.TabIndex = 3;
            // 
            // InstallProgress
            // 
            this.InstallProgress.Location = new System.Drawing.Point(36, 1025);
            this.InstallProgress.Name = "InstallProgress";
            this.InstallProgress.Size = new System.Drawing.Size(1697, 68);
            this.InstallProgress.TabIndex = 4;
            this.InstallProgress.Visible = false;
            // 
            // InstallProgressText
            // 
            this.InstallProgressText.BackColor = System.Drawing.SystemColors.Info;
            this.InstallProgressText.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstallProgressText.Location = new System.Drawing.Point(36, 939);
            this.InstallProgressText.Name = "InstallProgressText";
            this.InstallProgressText.Size = new System.Drawing.Size(1697, 68);
            this.InstallProgressText.TabIndex = 5;
            this.InstallProgressText.Visible = false;
            // 
            // SkipCDN
            // 
            this.SkipCDN.AutoSize = true;
            this.SkipCDN.Checked = true;
            this.SkipCDN.CheckState = System.Windows.Forms.CheckState.Checked;
            this.SkipCDN.Location = new System.Drawing.Point(1011, 1152);
            this.SkipCDN.Name = "SkipCDN";
            this.SkipCDN.Size = new System.Drawing.Size(281, 29);
            this.SkipCDN.TabIndex = 6;
            this.SkipCDN.Text = "Skip CDN Asset Bundles";
            this.SkipCDN.UseVisualStyleBackColor = true;
            // 
            // InstallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1771, 1332);
            this.Controls.Add(this.SkipCDN);
            this.Controls.Add(this.InstallProgressText);
            this.Controls.Add(this.InstallProgress);
            this.Controls.Add(this.Titles);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.InstallButton);
            this.Controls.Add(this.pictureBox1);
            this.Name = "InstallForm";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button InstallButton;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox Titles;
        private System.Windows.Forms.ProgressBar InstallProgress;
        private System.Windows.Forms.TextBox InstallProgressText;
        private System.Windows.Forms.CheckBox SkipCDN;
    }
}

