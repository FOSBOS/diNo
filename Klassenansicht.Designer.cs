﻿namespace diNo
{
  partial class Klassenansicht
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
      this.treeListView1 = new BrightIdeasSoftware.TreeListView();
      this.olvColumnBezeichnung = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
      this.objectListView1 = new BrightIdeasSoftware.ObjectListView();
      this.btnNotenbogenZeigen = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).BeginInit();
      this.SuspendLayout();
      // 
      // treeListView1
      // 
      this.treeListView1.AllColumns.Add(this.olvColumnBezeichnung);
      this.treeListView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.treeListView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.olvColumnBezeichnung});
      this.treeListView1.Location = new System.Drawing.Point(12, 12);
      this.treeListView1.Name = "treeListView1";
      this.treeListView1.OwnerDraw = true;
      this.treeListView1.ShowGroups = false;
      this.treeListView1.Size = new System.Drawing.Size(207, 280);
      this.treeListView1.TabIndex = 5;
      this.treeListView1.UseCompatibleStateImageBehavior = false;
      this.treeListView1.View = System.Windows.Forms.View.Details;
      this.treeListView1.VirtualMode = true;
      this.treeListView1.SelectedIndexChanged += new System.EventHandler(this.treeListView1_SelectedIndexChanged);
      // 
      // olvColumnBezeichnung
      // 
      this.olvColumnBezeichnung.Hideable = false;
      this.olvColumnBezeichnung.IsEditable = false;
      this.olvColumnBezeichnung.Text = "Bezeichnung";
      this.olvColumnBezeichnung.Width = 204;
      // 
      // objectListView1
      // 
      this.objectListView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.objectListView1.GridLines = true;
      this.objectListView1.Location = new System.Drawing.Point(225, 12);
      this.objectListView1.Name = "objectListView1";
      this.objectListView1.ShowGroups = false;
      this.objectListView1.Size = new System.Drawing.Size(706, 280);
      this.objectListView1.Sorting = System.Windows.Forms.SortOrder.Ascending;
      this.objectListView1.TabIndex = 6;
      this.objectListView1.UseCompatibleStateImageBehavior = false;
      this.objectListView1.View = System.Windows.Forms.View.Details;
      // 
      // btnNotenbogenZeigen
      // 
      this.btnNotenbogenZeigen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnNotenbogenZeigen.Image = global::diNo.Properties.Resources.piktogrammNotenbogen;
      this.btnNotenbogenZeigen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btnNotenbogenZeigen.Location = new System.Drawing.Point(12, 298);
      this.btnNotenbogenZeigen.Name = "btnNotenbogenZeigen";
      this.btnNotenbogenZeigen.Size = new System.Drawing.Size(207, 33);
      this.btnNotenbogenZeigen.TabIndex = 4;
      this.btnNotenbogenZeigen.Text = "Notenbogen anzeigen";
      this.btnNotenbogenZeigen.UseVisualStyleBackColor = true;
      this.btnNotenbogenZeigen.Click += new System.EventHandler(this.btnNotenbogenZeigen_Click);
      // 
      // Klassenansicht
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(943, 343);
      this.Controls.Add(this.objectListView1);
      this.Controls.Add(this.treeListView1);
      this.Controls.Add(this.btnNotenbogenZeigen);
      this.Name = "Klassenansicht";
      this.Text = "Klassenansicht";
      this.Load += new System.EventHandler(this.Klassenansicht_Load);
      ((System.ComponentModel.ISupportInitialize)(this.treeListView1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.objectListView1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button btnNotenbogenZeigen;
    private BrightIdeasSoftware.TreeListView treeListView1;
    private BrightIdeasSoftware.OLVColumn olvColumnBezeichnung;
    private BrightIdeasSoftware.ObjectListView objectListView1;
  }
}