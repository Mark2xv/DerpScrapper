namespace DerpScrapper.Library
{
    partial class SelectHit
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
            this.LabelText = new System.Windows.Forms.Label();
            this.LabelPath = new System.Windows.Forms.Label();
            this.SelectableItems = new System.Windows.Forms.FlowLayoutPanel();
            this.ButtonOK = new System.Windows.Forms.Button();
            this.ButtonCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LabelText
            // 
            this.LabelText.AutoSize = true;
            this.LabelText.Location = new System.Drawing.Point(9, 9);
            this.LabelText.Name = "LabelText";
            this.LabelText.Size = new System.Drawing.Size(188, 26);
            this.LabelText.TabIndex = 0;
            this.LabelText.Text = "Failed to determine what series this is.\r\nPlease select the correct series below." +
    "";
            // 
            // LabelPath
            // 
            this.LabelPath.AutoSize = true;
            this.LabelPath.Location = new System.Drawing.Point(9, 45);
            this.LabelPath.Name = "LabelPath";
            this.LabelPath.Size = new System.Drawing.Size(63, 13);
            this.LabelPath.TabIndex = 1;
            this.LabelPath.Text = "blablablabla";
            // 
            // SelectableItems
            // 
            this.SelectableItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectableItems.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.SelectableItems.Location = new System.Drawing.Point(12, 71);
            this.SelectableItems.Name = "SelectableItems";
            this.SelectableItems.Size = new System.Drawing.Size(296, 288);
            this.SelectableItems.TabIndex = 2;
            // 
            // ButtonOK
            // 
            this.ButtonOK.Location = new System.Drawing.Point(233, 371);
            this.ButtonOK.Name = "ButtonOK";
            this.ButtonOK.Size = new System.Drawing.Size(75, 23);
            this.ButtonOK.TabIndex = 3;
            this.ButtonOK.Text = "OK";
            this.ButtonOK.UseVisualStyleBackColor = true;
            // 
            // ButtonCancel
            // 
            this.ButtonCancel.Location = new System.Drawing.Point(12, 371);
            this.ButtonCancel.Name = "ButtonCancel";
            this.ButtonCancel.Size = new System.Drawing.Size(75, 23);
            this.ButtonCancel.TabIndex = 4;
            this.ButtonCancel.Text = "Cancel";
            this.ButtonCancel.UseVisualStyleBackColor = true;
            // 
            // SelectHit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 406);
            this.Controls.Add(this.ButtonCancel);
            this.Controls.Add(this.ButtonOK);
            this.Controls.Add(this.SelectableItems);
            this.Controls.Add(this.LabelPath);
            this.Controls.Add(this.LabelText);
            this.Name = "SelectHit";
            this.Text = "Give us a hint ...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LabelText;
        private System.Windows.Forms.Label LabelPath;
        private System.Windows.Forms.FlowLayoutPanel SelectableItems;
        private System.Windows.Forms.Button ButtonOK;
        private System.Windows.Forms.Button ButtonCancel;
    }
}