namespace diNo
{
  partial class UserControlNotenbogen
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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dataGridNoten = new System.Windows.Forms.DataGridView();
      this.cFach = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.csLHj1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cSchnittMdl1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cSAHj1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cPunkte2DezHj1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cHj1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.undefiniertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.einbringenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.nichtEinbringenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.ungueltigToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.csLHj2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cSchnittMdl2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cSAHj2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cPunkte2DezHj2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cHj2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cSAP = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.MAP = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.APG = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.cJN = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.chkShowHj1 = new System.Windows.Forms.CheckBox();
      this.chkShowHj2 = new System.Windows.Forms.CheckBox();
      this.chkShowAbi = new System.Windows.Forms.CheckBox();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridNoten)).BeginInit();
      this.contextMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // dataGridNoten
      // 
      this.dataGridNoten.AllowUserToAddRows = false;
      this.dataGridNoten.AllowUserToDeleteRows = false;
      dataGridViewCellStyle14.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      dataGridViewCellStyle14.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle14.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle14.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle14.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dataGridNoten.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle14;
      this.dataGridNoten.ColumnHeadersHeight = 30;
      this.dataGridNoten.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      this.dataGridNoten.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cFach,
            this.csLHj1,
            this.cSchnittMdl1,
            this.cSAHj1,
            this.cPunkte2DezHj1,
            this.cHj1,
            this.csLHj2,
            this.cSchnittMdl2,
            this.cSAHj2,
            this.cPunkte2DezHj2,
            this.cHj2,
            this.cSAP,
            this.MAP,
            this.APG,
            this.cJN});
      this.dataGridNoten.Location = new System.Drawing.Point(0, 70);
      this.dataGridNoten.MultiSelect = false;
      this.dataGridNoten.Name = "dataGridNoten";
      this.dataGridNoten.ReadOnly = true;
      this.dataGridNoten.RowHeadersVisible = false;
      this.dataGridNoten.RowHeadersWidth = 70;
      this.dataGridNoten.Size = new System.Drawing.Size(1149, 505);
      this.dataGridNoten.TabIndex = 1;
      this.dataGridNoten.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridNoten_CellMouseDown);
      // 
      // cFach
      // 
      dataGridViewCellStyle15.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      this.cFach.DefaultCellStyle = dataGridViewCellStyle15;
      this.cFach.DividerWidth = 5;
      this.cFach.Frozen = true;
      this.cFach.HeaderText = "Fach";
      this.cFach.Name = "cFach";
      this.cFach.ReadOnly = true;
      this.cFach.Width = 200;
      // 
      // csLHj1
      // 
      dataGridViewCellStyle16.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      this.csLHj1.DefaultCellStyle = dataGridViewCellStyle16;
      this.csLHj1.Frozen = true;
      this.csLHj1.HeaderText = "sL";
      this.csLHj1.Name = "csLHj1";
      this.csLHj1.ReadOnly = true;
      this.csLHj1.ToolTipText = "sonstige Leistungen (KA, Ex, mdl.)";
      this.csLHj1.Width = 152;
      // 
      // cSchnittMdl1
      // 
      dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.cSchnittMdl1.DefaultCellStyle = dataGridViewCellStyle17;
      this.cSchnittMdl1.Frozen = true;
      this.cSchnittMdl1.HeaderText = "Ø sL";
      this.cSchnittMdl1.Name = "cSchnittMdl1";
      this.cSchnittMdl1.ReadOnly = true;
      this.cSchnittMdl1.ToolTipText = "Durchschnitt aller sonstigen Leistungen";
      this.cSchnittMdl1.Width = 50;
      // 
      // cSAHj1
      // 
      dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      this.cSAHj1.DefaultCellStyle = dataGridViewCellStyle18;
      this.cSAHj1.Frozen = true;
      this.cSAHj1.HeaderText = "SA";
      this.cSAHj1.Name = "cSAHj1";
      this.cSAHj1.ReadOnly = true;
      this.cSAHj1.ToolTipText = "Schulaufgaben im 1. Halbjahr";
      this.cSAHj1.Width = 50;
      // 
      // cPunkte2DezHj1
      // 
      dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.cPunkte2DezHj1.DefaultCellStyle = dataGridViewCellStyle19;
      this.cPunkte2DezHj1.Frozen = true;
      this.cPunkte2DezHj1.HeaderText = "Ø";
      this.cPunkte2DezHj1.Name = "cPunkte2DezHj1";
      this.cPunkte2DezHj1.ReadOnly = true;
      this.cPunkte2DezHj1.ToolTipText = "Gesamtnotenpunkte 1. Halbjahr auf 2 Dezimalen";
      this.cPunkte2DezHj1.Width = 50;
      // 
      // cHj1
      // 
      this.cHj1.ContextMenuStrip = this.contextMenu;
      dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cHj1.DefaultCellStyle = dataGridViewCellStyle20;
      this.cHj1.DividerWidth = 5;
      this.cHj1.Frozen = true;
      this.cHj1.HeaderText = "1. Hj";
      this.cHj1.Name = "cHj1";
      this.cHj1.ReadOnly = true;
      this.cHj1.ToolTipText = "Gesamtpunktzahl 1. Halbjahr";
      this.cHj1.Width = 55;
      // 
      // contextMenu
      // 
      this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undefiniertToolStripMenuItem,
            this.einbringenToolStripMenuItem,
            this.nichtEinbringenToolStripMenuItem,
            this.ungueltigToolStripMenuItem});
      this.contextMenu.Name = "contextMenu";
      this.contextMenu.Size = new System.Drawing.Size(162, 92);
      this.contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
      // 
      // undefiniertToolStripMenuItem
      // 
      this.undefiniertToolStripMenuItem.Name = "undefiniertToolStripMenuItem";
      this.undefiniertToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.undefiniertToolStripMenuItem.Tag = "0";
      this.undefiniertToolStripMenuItem.Text = "undefiniert";
      this.undefiniertToolStripMenuItem.Click += new System.EventHandler(this.undefiniertToolStripMenuItem_Click);
      // 
      // einbringenToolStripMenuItem
      // 
      this.einbringenToolStripMenuItem.Name = "einbringenToolStripMenuItem";
      this.einbringenToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.einbringenToolStripMenuItem.Tag = "1";
      this.einbringenToolStripMenuItem.Text = "einbringen";
      this.einbringenToolStripMenuItem.Click += new System.EventHandler(this.einbringenToolStripMenuItem_Click);
      // 
      // nichtEinbringenToolStripMenuItem
      // 
      this.nichtEinbringenToolStripMenuItem.Name = "nichtEinbringenToolStripMenuItem";
      this.nichtEinbringenToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.nichtEinbringenToolStripMenuItem.Tag = "2";
      this.nichtEinbringenToolStripMenuItem.Text = "nicht einbringen";
      this.nichtEinbringenToolStripMenuItem.Click += new System.EventHandler(this.nichtEinbringenToolStripMenuItem_Click);
      // 
      // ungueltigToolStripMenuItem
      // 
      this.ungueltigToolStripMenuItem.Name = "ungueltigToolStripMenuItem";
      this.ungueltigToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
      this.ungueltigToolStripMenuItem.Tag = "3";
      this.ungueltigToolStripMenuItem.Text = "ungültig";
      this.ungueltigToolStripMenuItem.Click += new System.EventHandler(this.ungueltigToolStripMenuItem_Click);
      // 
      // csLHj2
      // 
      dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      this.csLHj2.DefaultCellStyle = dataGridViewCellStyle7;
      this.csLHj2.Frozen = true;
      this.csLHj2.HeaderText = "sL";
      this.csLHj2.Name = "csLHj2";
      this.csLHj2.ReadOnly = true;
      this.csLHj2.ToolTipText = "sonstige Leistungen (KA, Ex, mdl.)";
      this.csLHj2.Width = 152;
      // 
      // cSchnittMdl2
      // 
      dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.cSchnittMdl2.DefaultCellStyle = dataGridViewCellStyle21;
      this.cSchnittMdl2.Frozen = true;
      this.cSchnittMdl2.HeaderText = "Ø sL";
      this.cSchnittMdl2.Name = "cSchnittMdl2";
      this.cSchnittMdl2.ReadOnly = true;
      this.cSchnittMdl2.ToolTipText = "Durchschnitt aller sonstigen Leistungen";
      this.cSchnittMdl2.Width = 50;
      // 
      // cSAHj2
      // 
      dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      this.cSAHj2.DefaultCellStyle = dataGridViewCellStyle22;
      this.cSAHj2.Frozen = true;
      this.cSAHj2.HeaderText = "SA";
      this.cSAHj2.Name = "cSAHj2";
      this.cSAHj2.ReadOnly = true;
      this.cSAHj2.ToolTipText = "Schulaufgaben im 2. Halbjahr";
      this.cSAHj2.Width = 50;
      // 
      // cPunkte2DezHj2
      // 
      dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.cPunkte2DezHj2.DefaultCellStyle = dataGridViewCellStyle23;
      this.cPunkte2DezHj2.Frozen = true;
      this.cPunkte2DezHj2.HeaderText = "Ø";
      this.cPunkte2DezHj2.Name = "cPunkte2DezHj2";
      this.cPunkte2DezHj2.ReadOnly = true;
      this.cPunkte2DezHj2.ToolTipText = "Gesamtnotenpunkte 2. Halbjahr auf 2 Dezimalen";
      this.cPunkte2DezHj2.Width = 50;
      // 
      // cHj2
      // 
      this.cHj2.ContextMenuStrip = this.contextMenu;
      dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle24.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cHj2.DefaultCellStyle = dataGridViewCellStyle24;
      this.cHj2.DividerWidth = 5;
      this.cHj2.Frozen = true;
      this.cHj2.HeaderText = "2. Hj";
      this.cHj2.Name = "cHj2";
      this.cHj2.ReadOnly = true;
      this.cHj2.ToolTipText = "Gesamtpunktzahl 2. Halbjahr";
      this.cHj2.Width = 55;
      // 
      // cSAP
      // 
      this.cSAP.Frozen = true;
      this.cSAP.HeaderText = "SAP";
      this.cSAP.Name = "cSAP";
      this.cSAP.ReadOnly = true;
      this.cSAP.Width = 50;
      // 
      // MAP
      // 
      this.MAP.Frozen = true;
      this.MAP.HeaderText = "MAP";
      this.MAP.Name = "MAP";
      this.MAP.ReadOnly = true;
      this.MAP.Width = 50;
      // 
      // APG
      // 
      this.APG.DividerWidth = 5;
      this.APG.Frozen = true;
      this.APG.HeaderText = "APG";
      this.APG.Name = "APG";
      this.APG.ReadOnly = true;
      this.APG.Width = 55;
      // 
      // cJN
      // 
      this.cJN.ContextMenuStrip = this.contextMenu;
      dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      this.cJN.DefaultCellStyle = dataGridViewCellStyle12;
      this.cJN.DividerWidth = 2;
      this.cJN.Frozen = true;
      this.cJN.HeaderText = "JN";
      this.cJN.Name = "cJN";
      this.cJN.ReadOnly = true;
      this.cJN.ToolTipText = "Jahresnote";
      this.cJN.Width = 52;
      // 
      // chkShowHj1
      // 
      this.chkShowHj1.AutoSize = true;
      this.chkShowHj1.Checked = true;
      this.chkShowHj1.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkShowHj1.Location = new System.Drawing.Point(3, 3);
      this.chkShowHj1.Name = "chkShowHj1";
      this.chkShowHj1.Size = new System.Drawing.Size(123, 17);
      this.chkShowHj1.TabIndex = 2;
      this.chkShowHj1.Text = "1. Halbjahr anzeigen";
      this.chkShowHj1.UseVisualStyleBackColor = true;
      this.chkShowHj1.CheckedChanged += new System.EventHandler(this.chkShowHj1_CheckedChanged);
      // 
      // chkShowHj2
      // 
      this.chkShowHj2.AutoSize = true;
      this.chkShowHj2.Checked = true;
      this.chkShowHj2.CheckState = System.Windows.Forms.CheckState.Checked;
      this.chkShowHj2.Location = new System.Drawing.Point(3, 25);
      this.chkShowHj2.Name = "chkShowHj2";
      this.chkShowHj2.Size = new System.Drawing.Size(123, 17);
      this.chkShowHj2.TabIndex = 3;
      this.chkShowHj2.Text = "2. Halbjahr anzeigen";
      this.chkShowHj2.UseVisualStyleBackColor = true;
      this.chkShowHj2.CheckedChanged += new System.EventHandler(this.chkShowHj2_CheckedChanged);
      // 
      // chkShowAbi
      // 
      this.chkShowAbi.AutoSize = true;
      this.chkShowAbi.Location = new System.Drawing.Point(3, 47);
      this.chkShowAbi.Name = "chkShowAbi";
      this.chkShowAbi.Size = new System.Drawing.Size(99, 17);
      this.chkShowAbi.TabIndex = 4;
      this.chkShowAbi.Text = "Abitur anzeigen";
      this.chkShowAbi.UseVisualStyleBackColor = true;
      this.chkShowAbi.CheckedChanged += new System.EventHandler(this.chkShowAbi_CheckedChanged);
      // 
      // UserControlNotenbogen
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.chkShowAbi);
      this.Controls.Add(this.chkShowHj2);
      this.Controls.Add(this.chkShowHj1);
      this.Controls.Add(this.dataGridNoten);
      this.Name = "UserControlNotenbogen";
      this.Size = new System.Drawing.Size(1149, 578);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridNoten)).EndInit();
      this.contextMenu.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridNoten;
    private System.Windows.Forms.CheckBox chkShowHj1;
    private System.Windows.Forms.CheckBox chkShowHj2;
    private System.Windows.Forms.CheckBox chkShowAbi;
    private System.Windows.Forms.ContextMenuStrip contextMenu;
    private System.Windows.Forms.DataGridViewTextBoxColumn cFach;
    private System.Windows.Forms.DataGridViewTextBoxColumn csLHj1;
    private System.Windows.Forms.DataGridViewTextBoxColumn cSchnittMdl1;
    private System.Windows.Forms.DataGridViewTextBoxColumn cSAHj1;
    private System.Windows.Forms.DataGridViewTextBoxColumn cPunkte2DezHj1;
    private System.Windows.Forms.DataGridViewTextBoxColumn cHj1;
    private System.Windows.Forms.ToolStripMenuItem undefiniertToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem einbringenToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem nichtEinbringenToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem ungueltigToolStripMenuItem;
    private System.Windows.Forms.DataGridViewTextBoxColumn csLHj2;
    private System.Windows.Forms.DataGridViewTextBoxColumn cSchnittMdl2;
    private System.Windows.Forms.DataGridViewTextBoxColumn cSAHj2;
    private System.Windows.Forms.DataGridViewTextBoxColumn cPunkte2DezHj2;
    private System.Windows.Forms.DataGridViewTextBoxColumn cHj2;
    private System.Windows.Forms.DataGridViewTextBoxColumn cSAP;
    private System.Windows.Forms.DataGridViewTextBoxColumn MAP;
    private System.Windows.Forms.DataGridViewTextBoxColumn APG;
    private System.Windows.Forms.DataGridViewTextBoxColumn cJN;
  }
}
