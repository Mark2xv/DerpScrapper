namespace DerpScrapper.UI.Tabpanels
{
    partial class Library
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
            this.serieOverviewList = new System.Windows.Forms.ListView();
            this.serieDetailView = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // serieOverviewList
            // 
            this.serieOverviewList.Dock = System.Windows.Forms.DockStyle.Left;
            this.serieOverviewList.Location = new System.Drawing.Point(0, 0);
            this.serieOverviewList.Name = "serieOverviewList";
            this.serieOverviewList.Size = new System.Drawing.Size(137, 423);
            this.serieOverviewList.TabIndex = 0;
            this.serieOverviewList.UseCompatibleStateImageBehavior = false;
            // 
            // serieDetailView
            // 
            this.serieDetailView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.serieDetailView.Location = new System.Drawing.Point(137, 0);
            this.serieDetailView.Name = "serieDetailView";
            this.serieDetailView.Size = new System.Drawing.Size(577, 423);
            this.serieDetailView.TabIndex = 1;
            this.serieDetailView.UseCompatibleStateImageBehavior = false;
            // 
            // Serie
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.serieDetailView);
            this.Controls.Add(this.serieOverviewList);
            this.Name = "Serie";
            this.Size = new System.Drawing.Size(714, 423);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView serieOverviewList;
        private System.Windows.Forms.ListView serieDetailView;
    }
}
