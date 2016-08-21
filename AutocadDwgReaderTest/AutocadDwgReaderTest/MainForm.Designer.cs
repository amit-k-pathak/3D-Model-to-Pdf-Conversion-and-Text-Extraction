namespace AutocadDwgReaderTest
{
    partial class MainForm
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDwgFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToMultisheetPDFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergePDFsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToAutoCADToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.launchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.accToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMainMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.listBoxFileContents = new System.Windows.Forms.ListBox();
            this.buttonSaveLog = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.extractAllVectorContentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.convertToolStripMenuItem,
            this.connectToolStripMenuItem,
            this.launchToolStripMenuItem,
            this.exitToolStripMainMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(742, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDwgFileToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openDwgFileToolStripMenuItem
            // 
            this.openDwgFileToolStripMenuItem.Name = "openDwgFileToolStripMenuItem";
            this.openDwgFileToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.openDwgFileToolStripMenuItem.Text = "Open Dwg/Dxf File";
            this.openDwgFileToolStripMenuItem.Click += new System.EventHandler(this.openDwgFileToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // convertToolStripMenuItem
            // 
            this.convertToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convertToPDFToolStripMenuItem,
            this.convertToMultisheetPDFToolStripMenuItem,
            this.mergePDFsToolStripMenuItem,
            this.extractAllVectorContentsToolStripMenuItem});
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.convertToolStripMenuItem.Text = "Convert";
            // 
            // convertToPDFToolStripMenuItem
            // 
            this.convertToPDFToolStripMenuItem.Name = "convertToPDFToolStripMenuItem";
            this.convertToPDFToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.convertToPDFToolStripMenuItem.Text = "Convert to PDF";
            this.convertToPDFToolStripMenuItem.Click += new System.EventHandler(this.convertToPDFToolStripMenuItem_Click);
            // 
            // convertToMultisheetPDFToolStripMenuItem
            // 
            this.convertToMultisheetPDFToolStripMenuItem.Name = "convertToMultisheetPDFToolStripMenuItem";
            this.convertToMultisheetPDFToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.convertToMultisheetPDFToolStripMenuItem.Text = "Convert to Multisheet PDF";
            this.convertToMultisheetPDFToolStripMenuItem.Click += new System.EventHandler(this.convertToMultisheetPDFToolStripMenuItem_Click);
            // 
            // mergePDFsToolStripMenuItem
            // 
            this.mergePDFsToolStripMenuItem.Name = "mergePDFsToolStripMenuItem";
            this.mergePDFsToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.mergePDFsToolStripMenuItem.Text = "Merge PDFs";
            this.mergePDFsToolStripMenuItem.Click += new System.EventHandler(this.mergePDFsToolStripMenuItem_Click);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToAutoCADToolStripMenuItem});
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.connectToolStripMenuItem.Text = "Connect";
            // 
            // connectToAutoCADToolStripMenuItem
            // 
            this.connectToAutoCADToolStripMenuItem.Name = "connectToAutoCADToolStripMenuItem";
            this.connectToAutoCADToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.connectToAutoCADToolStripMenuItem.Text = "Connect to AutoCAD";
            this.connectToAutoCADToolStripMenuItem.Click += new System.EventHandler(this.connectToAutoCADToolStripMenuItem_Click);
            // 
            // launchToolStripMenuItem
            // 
            this.launchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.accToolStripMenuItem});
            this.launchToolStripMenuItem.Name = "launchToolStripMenuItem";
            this.launchToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.launchToolStripMenuItem.Text = "Launch";
            // 
            // accToolStripMenuItem
            // 
            this.accToolStripMenuItem.Name = "accToolStripMenuItem";
            this.accToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.accToolStripMenuItem.Text = "Accore DWG True View";
            this.accToolStripMenuItem.Click += new System.EventHandler(this.accToolStripMenuItem_Click);
            // 
            // exitToolStripMainMenu
            // 
            this.exitToolStripMainMenu.Name = "exitToolStripMainMenu";
            this.exitToolStripMainMenu.Size = new System.Drawing.Size(37, 20);
            this.exitToolStripMainMenu.Text = "Exit";
            this.exitToolStripMainMenu.Click += new System.EventHandler(this.exitToolStripMainMenu_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "DWG Files|*.dwg|DXF Files|*.dxf";
            // 
            // listBoxFileContents
            // 
            this.listBoxFileContents.FormattingEnabled = true;
            this.listBoxFileContents.Location = new System.Drawing.Point(21, 49);
            this.listBoxFileContents.Name = "listBoxFileContents";
            this.listBoxFileContents.Size = new System.Drawing.Size(698, 550);
            this.listBoxFileContents.TabIndex = 1;
            // 
            // buttonSaveLog
            // 
            this.buttonSaveLog.Location = new System.Drawing.Point(315, 609);
            this.buttonSaveLog.Name = "buttonSaveLog";
            this.buttonSaveLog.Size = new System.Drawing.Size(115, 23);
            this.buttonSaveLog.TabIndex = 2;
            this.buttonSaveLog.Text = "Save Log";
            this.buttonSaveLog.UseVisualStyleBackColor = true;
            this.buttonSaveLog.Click += new System.EventHandler(this.buttonSaveLog_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Text Files|*.txt";
            // 
            // extractAllVectorContentsToolStripMenuItem
            // 
            this.extractAllVectorContentsToolStripMenuItem.Name = "extractAllVectorContentsToolStripMenuItem";
            this.extractAllVectorContentsToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.extractAllVectorContentsToolStripMenuItem.Text = "Extract All vector contents";
            this.extractAllVectorContentsToolStripMenuItem.Click += new System.EventHandler(this.extractAllVectorContentsToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.ClientSize = new System.Drawing.Size(742, 644);
            this.Controls.Add(this.buttonSaveLog);
            this.Controls.Add(this.listBoxFileContents);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Main Form";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDwgFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMainMenu;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ListBox listBoxFileContents;
        private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToPDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToMultisheetPDFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToAutoCADToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem launchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem accToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mergePDFsToolStripMenuItem;
        private System.Windows.Forms.Button buttonSaveLog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem extractAllVectorContentsToolStripMenuItem;
    }
}

