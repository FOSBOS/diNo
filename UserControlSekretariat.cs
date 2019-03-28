using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlSekretariat : UserControl
  {
    private Schueler schueler;

    public UserControlSekretariat()
    {
      InitializeComponent();
    }

    public Schueler Schueler
    {
      get
      {
        return schueler;
      }
      set
      {
        this.schueler = value;
        if (schueler != null)
        {
          listBoxMittlereReifeSchulart.SelectedItem = schueler.Data.IsEintrittAusSchulartNull() ? null : schueler.Data.EintrittAusSchulart;
          numericUpDownDeutsch.Value = schueler.Data.IsMittlereReifeDeutschnoteNull() ? null : (decimal?)schueler.Data.MittlereReifeDeutschnote;
          numericUpDownEnglisch.Value = schueler.Data.IsMittlereReifeEnglischnoteNull() ? null : (decimal?)schueler.Data.MittlereReifeEnglischnote;
          numericUpDownMathe.Value = schueler.Data.IsMittlereReifeMathenoteNull() ? null : (decimal?)schueler.Data.MittlereReifeMathenote;
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      schueler.Data.EintrittAusSchulart = (listBoxMittlereReifeSchulart.SelectedItem != null) ? (String)listBoxMittlereReifeSchulart.SelectedItem : null;
      if (numericUpDownDeutsch.Value == null) schueler.Data.SetMittlereReifeDeutschnoteNull();
      else schueler.Data.MittlereReifeDeutschnote = (int)numericUpDownDeutsch.Value.GetValueOrDefault();
      if (numericUpDownEnglisch.Value == null) schueler.Data.SetMittlereReifeEnglischnoteNull();
      else schueler.Data.MittlereReifeEnglischnote = (int)numericUpDownEnglisch.Value.GetValueOrDefault();
      if (numericUpDownMathe.Value == null) schueler.Data.SetMittlereReifeMathenoteNull();
      else schueler.Data.MittlereReifeMathenote = (int)numericUpDownMathe.Value.GetValueOrDefault();

      schueler.Save();
    }

    private void InitializeComponent()
    {
      this.listBoxMittlereReifeSchulart = new System.Windows.Forms.ListBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.groupBoxMittlereReife = new System.Windows.Forms.GroupBox();
      this.numericUpDownMathe = new diNo.NumericUpDownNullable();
      this.numericUpDownEnglisch = new diNo.NumericUpDownNullable();
      this.numericUpDownDeutsch = new diNo.NumericUpDownNullable();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBoxMittlereReife.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMathe)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnglisch)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDeutsch)).BeginInit();
      this.SuspendLayout();
      // 
      // listBoxMittlereReifeSchulart
      // 
      this.listBoxMittlereReifeSchulart.FormattingEnabled = true;
      this.listBoxMittlereReifeSchulart.Items.AddRange(new object[] {
            "GY",
            "RS",
            "WS",
            "MS"});
      this.listBoxMittlereReifeSchulart.Location = new System.Drawing.Point(6, 43);
      this.listBoxMittlereReifeSchulart.Name = "listBoxMittlereReifeSchulart";
      this.listBoxMittlereReifeSchulart.Size = new System.Drawing.Size(120, 69);
      this.listBoxMittlereReifeSchulart.TabIndex = 0;
      // 
      // btnSave
      // 
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::diNo.Properties.Resources.Save;
      this.btnSave.Location = new System.Drawing.Point(267, 84);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(40, 40);
      this.btnSave.TabIndex = 27;
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // groupBoxMittlereReife
      // 
      this.groupBoxMittlereReife.Controls.Add(this.numericUpDownMathe);
      this.groupBoxMittlereReife.Controls.Add(this.numericUpDownEnglisch);
      this.groupBoxMittlereReife.Controls.Add(this.numericUpDownDeutsch);
      this.groupBoxMittlereReife.Controls.Add(this.label4);
      this.groupBoxMittlereReife.Controls.Add(this.label3);
      this.groupBoxMittlereReife.Controls.Add(this.label2);
      this.groupBoxMittlereReife.Controls.Add(this.label1);
      this.groupBoxMittlereReife.Controls.Add(this.listBoxMittlereReifeSchulart);
      this.groupBoxMittlereReife.Location = new System.Drawing.Point(3, 3);
      this.groupBoxMittlereReife.Name = "groupBoxMittlereReife";
      this.groupBoxMittlereReife.Size = new System.Drawing.Size(258, 120);
      this.groupBoxMittlereReife.TabIndex = 28;
      this.groupBoxMittlereReife.TabStop = false;
      this.groupBoxMittlereReife.Text = "mittlere Reife";
      // 
      // numericUpDownMathe
      // 
      this.numericUpDownMathe.Location = new System.Drawing.Point(199, 93);
      this.numericUpDownMathe.Name = "numericUpDownMathe";
      this.numericUpDownMathe.Size = new System.Drawing.Size(49, 20);
      this.numericUpDownMathe.TabIndex = 7;
      this.numericUpDownMathe.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
      // 
      // numericUpDownEnglisch
      // 
      this.numericUpDownEnglisch.Location = new System.Drawing.Point(199, 69);
      this.numericUpDownEnglisch.Name = "numericUpDownEnglisch";
      this.numericUpDownEnglisch.Size = new System.Drawing.Size(49, 20);
      this.numericUpDownEnglisch.TabIndex = 6;
      this.numericUpDownEnglisch.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
      // 
      // numericUpDownDeutsch
      // 
      this.numericUpDownDeutsch.Location = new System.Drawing.Point(199, 43);
      this.numericUpDownDeutsch.Name = "numericUpDownDeutsch";
      this.numericUpDownDeutsch.Size = new System.Drawing.Size(49, 20);
      this.numericUpDownDeutsch.TabIndex = 5;
      this.numericUpDownDeutsch.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(146, 95);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(37, 13);
      this.label4.TabIndex = 4;
      this.label4.Text = "Mathe";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(146, 71);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(47, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Englisch";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(146, 45);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(47, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Deutsch";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 27);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(46, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Schulart";
      // 
      // UserControlSekretariat
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.groupBoxMittlereReife);
      this.Controls.Add(this.btnSave);
      this.Name = "UserControlSekretariat";
      this.Size = new System.Drawing.Size(621, 390);
      this.groupBoxMittlereReife.ResumeLayout(false);
      this.groupBoxMittlereReife.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMathe)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnglisch)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDeutsch)).EndInit();
      this.ResumeLayout(false);

    }
  }
}