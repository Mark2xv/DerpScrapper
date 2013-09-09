namespace DerpScrapper.Library
{
    partial class EpisodeView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LabelEpisodeName = new System.Windows.Forms.Label();
            this.PictureEpisode = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PictureEpisode)).BeginInit();
            this.SuspendLayout();
            // 
            // LabelEpisodeName
            // 
            this.LabelEpisodeName.AutoSize = true;
            this.LabelEpisodeName.Location = new System.Drawing.Point(54, 137);
            this.LabelEpisodeName.Name = "LabelEpisodeName";
            this.LabelEpisodeName.Size = new System.Drawing.Size(70, 13);
            this.LabelEpisodeName.TabIndex = 0;
            this.LabelEpisodeName.Text = "episodename";
            // 
            // PictureEpisode
            // 
            this.PictureEpisode.Location = new System.Drawing.Point(0, 0);
            this.PictureEpisode.Name = "PictureEpisode";
            this.PictureEpisode.Size = new System.Drawing.Size(169, 107);
            this.PictureEpisode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PictureEpisode.TabIndex = 1;
            this.PictureEpisode.TabStop = false;
            // 
            // EpisodeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.PictureEpisode);
            this.Controls.Add(this.LabelEpisodeName);
            this.Name = "EpisodeView";
            this.Size = new System.Drawing.Size(169, 150);
            ((System.ComponentModel.ISupportInitialize)(this.PictureEpisode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelEpisodeName;
        private System.Windows.Forms.PictureBox PictureEpisode;
    }
}
