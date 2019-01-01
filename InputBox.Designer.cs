namespace diNo
{
  partial class InputBox
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
      this.edInput = new System.Windows.Forms.TextBox();
      this.lbInput = new System.Windows.Forms.Label();
      this.btnOK = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // edInput
      // 
      this.edInput.Location = new System.Drawing.Point(42, 70);
      this.edInput.Name = "edInput";
      this.edInput.Size = new System.Drawing.Size(237, 20);
      this.edInput.TabIndex = 0;
      // 
      // lbInput
      // 
      this.lbInput.AutoSize = true;
      this.lbInput.Location = new System.Drawing.Point(39, 44);
      this.lbInput.Name = "lbInput";
      this.lbInput.Size = new System.Drawing.Size(60, 13);
      this.lbInput.TabIndex = 1;
      this.lbInput.Text = "neuer Wert";
      // 
      // btnOK
      // 
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.Location = new System.Drawing.Point(42, 112);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(102, 30);
      this.btnOK.TabIndex = 2;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(177, 112);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(102, 30);
      this.btnCancel.TabIndex = 3;
      this.btnCancel.Text = "Abbrechen";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // InputBox
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(332, 165);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.lbInput);
      this.Controls.Add(this.edInput);
      this.Name = "InputBox";
      this.Text = "diNo";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox edInput;
    private System.Windows.Forms.Label lbInput;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
  }
}