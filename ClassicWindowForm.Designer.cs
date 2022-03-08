namespace AdvancedWindowsAppearence
{
    partial class ClassicWindowForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClassicWindowForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuItem1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.subitemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItem2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItem1ToolStripMenuItem,
            this.menuItem2ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 36);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuItem1ToolStripMenuItem
            // 
            this.menuItem1ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subitemToolStripMenuItem});
            this.menuItem1ToolStripMenuItem.Name = "menuItem1ToolStripMenuItem";
            this.menuItem1ToolStripMenuItem.Size = new System.Drawing.Size(137, 32);
            this.menuItem1ToolStripMenuItem.Text = "Menu Item 1";
            // 
            // subitemToolStripMenuItem
            // 
            this.subitemToolStripMenuItem.Name = "subitemToolStripMenuItem";
            this.subitemToolStripMenuItem.Size = new System.Drawing.Size(270, 36);
            this.subitemToolStripMenuItem.Text = "Subitem";
            // 
            // menuItem2ToolStripMenuItem
            // 
            this.menuItem2ToolStripMenuItem.Name = "menuItem2ToolStripMenuItem";
            this.menuItem2ToolStripMenuItem.Size = new System.Drawing.Size(137, 32);
            this.menuItem2ToolStripMenuItem.Text = "Menu Item 2";
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Location = new System.Drawing.Point(0, 403);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(800, 48);
            this.hScrollBar1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Sample label";
            // 
            // ClassicWindowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hScrollBar1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ClassicWindowForm";
            this.Text = "ClassicWindowForm";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuItem1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subitemToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuItem2ToolStripMenuItem;
        private System.Windows.Forms.HScrollBar hScrollBar1;
        private System.Windows.Forms.Label label1;
    }
}