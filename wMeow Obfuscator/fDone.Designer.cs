namespace wMeow_Obfuscator
{
    partial class fDone
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fDone));
            this.siticoneElipse1 = new ns1.SiticoneElipse(this.components);
            this.siticoneRoundedButton1 = new ns1.SiticoneRoundedButton();
            this.rtitle = new ns1.SiticoneLabel();
            this.Btitle = new ns1.SiticoneLabel();
            this.siticoneImageButton1 = new ns1.SiticoneImageButton();
            this.SuspendLayout();
            // 
            // siticoneElipse1
            // 
            this.siticoneElipse1.TargetControl = this;
            // 
            // siticoneRoundedButton1
            // 
            this.siticoneRoundedButton1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.siticoneRoundedButton1.BorderThickness = 1;
            this.siticoneRoundedButton1.CheckedState.Parent = this.siticoneRoundedButton1;
            this.siticoneRoundedButton1.CustomImages.Parent = this.siticoneRoundedButton1;
            this.siticoneRoundedButton1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.siticoneRoundedButton1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.siticoneRoundedButton1.ForeColor = System.Drawing.Color.White;
            this.siticoneRoundedButton1.HoveredState.Parent = this.siticoneRoundedButton1;
            this.siticoneRoundedButton1.Location = new System.Drawing.Point(328, 73);
            this.siticoneRoundedButton1.Name = "siticoneRoundedButton1";
            this.siticoneRoundedButton1.ShadowDecoration.Parent = this.siticoneRoundedButton1;
            this.siticoneRoundedButton1.Size = new System.Drawing.Size(60, 35);
            this.siticoneRoundedButton1.TabIndex = 0;
            this.siticoneRoundedButton1.Text = "OK";
            this.siticoneRoundedButton1.Click += new System.EventHandler(this.siticoneRoundedButton1_Click);
            // 
            // rtitle
            // 
            this.rtitle.BackColor = System.Drawing.Color.Transparent;
            this.rtitle.Font = new System.Drawing.Font("Corbel", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtitle.ForeColor = System.Drawing.Color.White;
            this.rtitle.IsContextMenuEnabled = false;
            this.rtitle.IsSelectionEnabled = false;
            this.rtitle.Location = new System.Drawing.Point(196, 48);
            this.rtitle.Name = "rtitle";
            this.rtitle.Size = new System.Drawing.Size(106, 25);
            this.rtitle.TabIndex = 4;
            this.rtitle.Text = "Successfully !";
            this.rtitle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            this.rtitle.UseGdiPlusTextRendering = true;
            // 
            // Btitle
            // 
            this.Btitle.BackColor = System.Drawing.Color.Transparent;
            this.Btitle.Font = new System.Drawing.Font("Corbel", 14.25F, System.Drawing.FontStyle.Bold);
            this.Btitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.Btitle.IsContextMenuEnabled = false;
            this.Btitle.IsSelectionEnabled = false;
            this.Btitle.Location = new System.Drawing.Point(73, 48);
            this.Btitle.Name = "Btitle";
            this.Btitle.Size = new System.Drawing.Size(121, 25);
            this.Btitle.TabIndex = 3;
            this.Btitle.Text = "OBFUSCATED";
            this.Btitle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            this.Btitle.UseGdiPlusTextRendering = true;
            // 
            // siticoneImageButton1
            // 
            this.siticoneImageButton1.CheckedState.Parent = this.siticoneImageButton1;
            this.siticoneImageButton1.Cursor = System.Windows.Forms.Cursors.Default;
            this.siticoneImageButton1.HoveredState.ImageSize = new System.Drawing.Size(50, 50);
            this.siticoneImageButton1.HoveredState.Parent = this.siticoneImageButton1;
            this.siticoneImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("siticoneImageButton1.Image")));
            this.siticoneImageButton1.ImageSize = new System.Drawing.Size(50, 50);
            this.siticoneImageButton1.Location = new System.Drawing.Point(17, 35);
            this.siticoneImageButton1.Name = "siticoneImageButton1";
            this.siticoneImageButton1.PressedState.ImageSize = new System.Drawing.Size(50, 50);
            this.siticoneImageButton1.PressedState.Parent = this.siticoneImageButton1;
            this.siticoneImageButton1.Size = new System.Drawing.Size(50, 50);
            this.siticoneImageButton1.TabIndex = 10;
            // 
            // done
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            this.ClientSize = new System.Drawing.Size(400, 120);
            this.Controls.Add(this.siticoneImageButton1);
            this.Controls.Add(this.rtitle);
            this.Controls.Add(this.Btitle);
            this.Controls.Add(this.siticoneRoundedButton1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(400, 120);
            this.Name = "done";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.done_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ns1.SiticoneElipse siticoneElipse1;
        private ns1.SiticoneRoundedButton siticoneRoundedButton1;
        private ns1.SiticoneLabel rtitle;
        private ns1.SiticoneLabel Btitle;
        private ns1.SiticoneImageButton siticoneImageButton1;
    }
}