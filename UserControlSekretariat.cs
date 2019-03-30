using System;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlSekretariat : UserControl
  {
    private Schueler schueler;

    public UserControlSekretariat()
    {
      InitializeComponent();

      btnSave.Visible = Zugriff.Instance.HatVerwaltungsrechte;
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
          numAndereFremdspr2Note.Value = schueler.Data.IsAndereFremdspr2NoteNull() ? null : (decimal?)schueler.Data.AndereFremdspr2Note;
          textBoxAndereFremdspr2Text.Text = schueler.Data.IsAndereFremdspr2TextNull() ? "" : schueler.Data.AndereFremdspr2Text;
          listBoxZuletztBesuchteSchulart.SelectedItem = schueler.Data.IsSchulischeVorbildungNull() ? null : schueler.Data.SchulischeVorbildung;
          textBoxZeugnisbemerkung.Text = schueler.Data.IsZeugnisbemerkungNull() ? "" : schueler.Data.Zeugnisbemerkung;

          listBoxMittlereReifeSchulart.SelectedItem = schueler.Data.IsEintrittAusSchulartNull() ? null : schueler.Data.EintrittAusSchulart;
          numericUpDownDeutsch.Value = schueler.Data.IsMittlereReifeDeutschnoteNull() ? null : (decimal?)schueler.Data.MittlereReifeDeutschnote;
          numericUpDownEnglisch.Value = schueler.Data.IsMittlereReifeEnglischnoteNull() ? null : (decimal?)schueler.Data.MittlereReifeEnglischnote;
          numericUpDownMathe.Value = schueler.Data.IsMittlereReifeMathenoteNull() ? null : (decimal?)schueler.Data.MittlereReifeMathenote;
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (numAndereFremdspr2Note.Value == null) schueler.Data.SetAndereFremdspr2NoteNull();
      else schueler.Data.AndereFremdspr2Note = (int)numAndereFremdspr2Note.Value.GetValueOrDefault();
      if (textBoxAndereFremdspr2Text.Text == "") schueler.Data.SetAndereFremdspr2TextNull();
      else schueler.Data.AndereFremdspr2Text = textBoxAndereFremdspr2Text.Text;

      if (textBoxZeugnisbemerkung.Text == "") schueler.Data.SetZeugnisbemerkungNull();
      else schueler.Data.Zeugnisbemerkung = textBoxZeugnisbemerkung.Text;

      schueler.Data.SchulischeVorbildung = (listBoxZuletztBesuchteSchulart.SelectedItem != null) ? (String)listBoxZuletztBesuchteSchulart.SelectedItem : null;
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
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.textBoxAndereFremdspr2Text = new System.Windows.Forms.TextBox();
      this.label15 = new System.Windows.Forms.Label();
      this.label14 = new System.Windows.Forms.Label();
      this.textBoxZeugnisbemerkung = new System.Windows.Forms.TextBox();
      this.labelZeugnisbemerkung = new System.Windows.Forms.Label();
      this.lblSchulischeVorbildung = new System.Windows.Forms.Label();
      this.listBoxZuletztBesuchteSchulart = new System.Windows.Forms.ListBox();
      this.numAndereFremdspr2Note = new diNo.NumericUpDownNullable();
      this.numericUpDownMathe = new diNo.NumericUpDownNullable();
      this.numericUpDownEnglisch = new diNo.NumericUpDownNullable();
      this.numericUpDownDeutsch = new diNo.NumericUpDownNullable();
      this.groupBoxMittlereReife.SuspendLayout();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numAndereFremdspr2Note)).BeginInit();
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
            "MS",
            "BS",
            "WS",
            "BO",
            "SO",
            "KS"});
      this.listBoxMittlereReifeSchulart.Location = new System.Drawing.Point(6, 43);
      this.listBoxMittlereReifeSchulart.Name = "listBoxMittlereReifeSchulart";
      this.listBoxMittlereReifeSchulart.Size = new System.Drawing.Size(120, 108);
      this.listBoxMittlereReifeSchulart.TabIndex = 0;
      // 
      // btnSave
      // 
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::diNo.Properties.Resources.Save;
      this.btnSave.Location = new System.Drawing.Point(527, 428);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(40, 40);
      this.btnSave.TabIndex = 27;
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // groupBoxMittlereReife
      // 
      this.groupBoxMittlereReife.Controls.Add(this.listBoxZuletztBesuchteSchulart);
      this.groupBoxMittlereReife.Controls.Add(this.numericUpDownMathe);
      this.groupBoxMittlereReife.Controls.Add(this.numericUpDownEnglisch);
      this.groupBoxMittlereReife.Controls.Add(this.numericUpDownDeutsch);
      this.groupBoxMittlereReife.Controls.Add(this.lblSchulischeVorbildung);
      this.groupBoxMittlereReife.Controls.Add(this.label4);
      this.groupBoxMittlereReife.Controls.Add(this.label3);
      this.groupBoxMittlereReife.Controls.Add(this.label2);
      this.groupBoxMittlereReife.Controls.Add(this.label1);
      this.groupBoxMittlereReife.Controls.Add(this.listBoxMittlereReifeSchulart);
      this.groupBoxMittlereReife.Location = new System.Drawing.Point(3, 3);
      this.groupBoxMittlereReife.Name = "groupBoxMittlereReife";
      this.groupBoxMittlereReife.Size = new System.Drawing.Size(258, 272);
      this.groupBoxMittlereReife.TabIndex = 28;
      this.groupBoxMittlereReife.TabStop = false;
      this.groupBoxMittlereReife.Text = "mittlere Reife";
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
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.textBoxAndereFremdspr2Text);
      this.groupBox1.Controls.Add(this.label15);
      this.groupBox1.Controls.Add(this.label14);
      this.groupBox1.Controls.Add(this.numAndereFremdspr2Note);
      this.groupBox1.Location = new System.Drawing.Point(282, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(285, 75);
      this.groupBox1.TabIndex = 67;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Alternative 2. Fremdsprache";
      // 
      // textBoxAndereFremdspr2Text
      // 
      this.textBoxAndereFremdspr2Text.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxAndereFremdspr2Text.Location = new System.Drawing.Point(86, 41);
      this.textBoxAndereFremdspr2Text.Name = "textBoxAndereFremdspr2Text";
      this.textBoxAndereFremdspr2Text.Size = new System.Drawing.Size(193, 20);
      this.textBoxAndereFremdspr2Text.TabIndex = 68;
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label15.Location = new System.Drawing.Point(83, 25);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(61, 13);
      this.label15.TabIndex = 70;
      this.label15.Text = "Erläuterung";
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label14.Location = new System.Drawing.Point(11, 25);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(69, 13);
      this.label14.TabIndex = 69;
      this.label14.Text = "Notenpunkte";
      // 
      // textBoxZeugnisbemerkung
      // 
      this.textBoxZeugnisbemerkung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxZeugnisbemerkung.Location = new System.Drawing.Point(285, 103);
      this.textBoxZeugnisbemerkung.Name = "textBoxZeugnisbemerkung";
      this.textBoxZeugnisbemerkung.Size = new System.Drawing.Size(276, 20);
      this.textBoxZeugnisbemerkung.TabIndex = 84;
      // 
      // labelZeugnisbemerkung
      // 
      this.labelZeugnisbemerkung.AutoSize = true;
      this.labelZeugnisbemerkung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelZeugnisbemerkung.Location = new System.Drawing.Point(282, 87);
      this.labelZeugnisbemerkung.Name = "labelZeugnisbemerkung";
      this.labelZeugnisbemerkung.Size = new System.Drawing.Size(153, 13);
      this.labelZeugnisbemerkung.TabIndex = 83;
      this.labelZeugnisbemerkung.Text = "zusätzliche Zeugnisbemerkung";
      // 
      // lblSchulischeVorbildung
      // 
      this.lblSchulischeVorbildung.AutoSize = true;
      this.lblSchulischeVorbildung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSchulischeVorbildung.Location = new System.Drawing.Point(6, 168);
      this.lblSchulischeVorbildung.Name = "lblSchulischeVorbildung";
      this.lblSchulischeVorbildung.Size = new System.Drawing.Size(126, 13);
      this.lblSchulischeVorbildung.TabIndex = 77;
      this.lblSchulischeVorbildung.Text = "zuletzt besuchte Schulart";
      // 
      // listBoxZuletztBesuchteSchulart
      // 
      this.listBoxZuletztBesuchteSchulart.FormattingEnabled = true;
      this.listBoxZuletztBesuchteSchulart.Items.AddRange(new object[] {
            "",
            "BFo",
            "BFS",
            "BP",
            "BS",
            "BSo",
            "F10",
            "FAo",
            "GY0",
            "GY1",
            "H",
            "HSo",
            "HSq",
            "M",
            "QB",
            "R3a",
            "R3b",
            "RS",
            "RS1",
            "RS2",
            "RS3",
            "SoM",
            "VSo",
            "WS",
            "WSH",
            "WSM"});
      this.listBoxZuletztBesuchteSchulart.Location = new System.Drawing.Point(6, 184);
      this.listBoxZuletztBesuchteSchulart.Name = "listBoxZuletztBesuchteSchulart";
      this.listBoxZuletztBesuchteSchulart.Size = new System.Drawing.Size(118, 82);
      this.listBoxZuletztBesuchteSchulart.TabIndex = 85;
      // 
      // numAndereFremdspr2Note
      // 
      this.numAndereFremdspr2Note.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numAndereFremdspr2Note.Location = new System.Drawing.Point(14, 41);
      this.numAndereFremdspr2Note.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
      this.numAndereFremdspr2Note.Name = "numAndereFremdspr2Note";
      this.numAndereFremdspr2Note.Size = new System.Drawing.Size(63, 23);
      this.numAndereFremdspr2Note.TabIndex = 67;
      this.numAndereFremdspr2Note.Value = null;
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
      // UserControlSekretariat
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.textBoxZeugnisbemerkung);
      this.Controls.Add(this.labelZeugnisbemerkung);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.groupBoxMittlereReife);
      this.Controls.Add(this.btnSave);
      this.Name = "UserControlSekretariat";
      this.Size = new System.Drawing.Size(621, 479);
      this.groupBoxMittlereReife.ResumeLayout(false);
      this.groupBoxMittlereReife.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numAndereFremdspr2Note)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMathe)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnglisch)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDeutsch)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
  }
}