namespace DerpScrapper.Library
{
    partial class SeriesView
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
            this.FlowLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.SerieImage = new System.Windows.Forms.PictureBox();
            this.LabelSerieName = new System.Windows.Forms.Label();
            this.LabelSerieSynopsis = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SerieImage)).BeginInit();
            this.SuspendLayout();
            // 
            // FlowLayout
            // 
            this.FlowLayout.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FlowLayout.AutoScroll = true;
            this.FlowLayout.Location = new System.Drawing.Point(0, 178);
            this.FlowLayout.Name = "FlowLayout";
            this.FlowLayout.Size = new System.Drawing.Size(408, 272);
            this.FlowLayout.TabIndex = 0;
            // 
            // SerieImage
            // 
            this.SerieImage.Location = new System.Drawing.Point(12, 12);
            this.SerieImage.Name = "SerieImage";
            this.SerieImage.Size = new System.Drawing.Size(104, 150);
            this.SerieImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.SerieImage.TabIndex = 1;
            this.SerieImage.TabStop = false;
            // 
            // LabelSerieName
            // 
            this.LabelSerieName.AutoSize = true;
            this.LabelSerieName.Font = new System.Drawing.Font("Cambria", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSerieName.Location = new System.Drawing.Point(151, 12);
            this.LabelSerieName.Name = "LabelSerieName";
            this.LabelSerieName.Size = new System.Drawing.Size(98, 34);
            this.LabelSerieName.TabIndex = 2;
            this.LabelSerieName.Text = "label1";
            // 
            // LabelSerieSynopsis
            // 
            this.LabelSerieSynopsis.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelSerieSynopsis.AutoEllipsis = true;
            this.LabelSerieSynopsis.Location = new System.Drawing.Point(132, 50);
            this.LabelSerieSynopsis.Name = "LabelSerieSynopsis";
            this.LabelSerieSynopsis.Size = new System.Drawing.Size(264, 112);
            this.LabelSerieSynopsis.TabIndex = 3;
            this.LabelSerieSynopsis.Text = "SerieSynopsis";
            // 
            // SeriesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 450);
            this.Controls.Add(this.LabelSerieSynopsis);
            this.Controls.Add(this.LabelSerieName);
            this.Controls.Add(this.SerieImage);
            this.Controls.Add(this.FlowLayout);
            this.Name = "SeriesView";
            this.Text = "SeriesView";
            this.Load += new System.EventHandler(this.SeriesView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.SerieImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel FlowLayout;
        private System.Windows.Forms.PictureBox SerieImage;
        private System.Windows.Forms.Label LabelSerieName;
        private System.Windows.Forms.Label LabelSerieSynopsis;
    }
}