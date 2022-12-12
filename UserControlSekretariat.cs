using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace diNo
{
  public partial class UserControlSekretariat : UserControl
  {
    private Schueler schueler;
    private int FS2Art;
    private Dictionary<int, string> FachDict;

    public UserControlSekretariat()
    {
      InitializeComponent();
      btnSave.Visible = Zugriff.Instance.HatVerwaltungsrechte;

      FachDict = new Dictionary<int, string>();
      FachDict.Add(0, "");
      var ta = new FachTableAdapter();
      var dt = ta.GetData().Where(x => x.Kursniveau == (int)Kursniveau.Anfaenger);
      foreach (var f in dt)
      {
        FachDict.Add(f.Id, f.Bezeichnung);
      }
      cbAndereFremdspr2Fach.BeginUpdate();
      cbAndereFremdspr2Fach.DataSource = FachDict.ToList();
      cbAndereFremdspr2Fach.DisplayMember = "Value";
      cbAndereFremdspr2Fach.ValueMember = "Key";
      cbAndereFremdspr2Fach.EndUpdate();
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
          FS2Art = schueler.getNoten.ZweiteFSalt != null ? 2 : schueler.Data.AndereFremdspr2Art;
          opRS.Checked = FS2Art == 0;
          opErgPr.Checked = FS2Art == 1;
          opFFalt.Checked = FS2Art == 2;
          numAndereFremdspr2Note.Enabled = FS2Art < 2;
          numAndereFremdspr2Note.Value = schueler.Data.IsAndereFremdspr2NoteNull() ? null : (decimal?)schueler.Data.AndereFremdspr2Note;
          cbAndereFremdspr2Fach.SelectedValue = schueler.Data.IsAndereFremdspr2FachNull() ? 0 : schueler.Data.AndereFremdspr2Fach;
          if (FS2Art == 2)
          {
            var f = schueler.getNoten.ZweiteFSalt;
            try
            {
              lbFFalt.Text = "Hj1 = " + f.getHjLeistung(HjArt.Hj1).Punkte + ", Hj2 = " + f.getHjLeistung(HjArt.Hj2).Punkte + " aus "
                + f.getFach.Kuerzel + " der Jgst. " + (int)f.getHjLeistung(HjArt.Hj1).JgStufe;
            }
            catch
            { lbFFalt.Text = ""; }
          }
          else lbFFalt.Text = "";

          textBoxZeugnisbemerkung.Text = schueler.Data.IsZeugnisbemerkungNull() ? "" : schueler.Data.Zeugnisbemerkung;

          cbSchulischeVorbildung.Text = schueler.Data.IsSchulischeVorbildungNull() ? null : schueler.Data.SchulischeVorbildung;
          //cbEintrittAusSchulart.Text = schueler.Data.IsEintrittAusSchulartNull() ? null : schueler.Data.EintrittAusSchulart;
          numericUpDownDeutsch.Value = schueler.Data.IsMittlereReifeDeutschnoteNull() ? null : (decimal?)schueler.Data.MittlereReifeDeutschnote;
          numericUpDownEnglisch.Value = schueler.Data.IsMittlereReifeEnglischnoteNull() ? null : (decimal?)schueler.Data.MittlereReifeEnglischnote;
          numericUpDownMathe.Value = schueler.Data.IsMittlereReifeMathenoteNull() ? null : (decimal?)schueler.Data.MittlereReifeMathenote;

          textBoxID.Text = schueler.Id.ToString();
          checkBoxLegasthenie.Checked = schueler.IsLegastheniker;
          numLRSZuschlagMin.Value = schueler.Data.LRSZuschlagMin;
          numLRSZuschlagMax.Value = schueler.Data.LRSZuschlagMax;
          textBoxNachname.Text = schueler.Data.Name;
          textBoxVorname.Text = schueler.Data.Vorname;
          textBoxRufname.Text = schueler.Data.Rufname;
          textBoxAR.Text = schueler.Data.Ausbildungsrichtung;
          textBoxFB.Text = schueler.Data.Schulart;
          chkSonderfallNur2Hj.Checked = schueler.Data.SonderfallNur2Hj;
        }
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (numAndereFremdspr2Note.Value == null) schueler.Data.SetAndereFremdspr2NoteNull();
      else schueler.Data.AndereFremdspr2Note = (int)numAndereFremdspr2Note.Value.GetValueOrDefault();
      if ((int)cbAndereFremdspr2Fach.SelectedValue == 0) schueler.Data.SetAndereFremdspr2FachNull();
      else schueler.Data.AndereFremdspr2Fach = (int)cbAndereFremdspr2Fach.SelectedValue;

      schueler.Data.AndereFremdspr2Art = opErgPr.Checked ? 1 : 0;
      
      if (textBoxZeugnisbemerkung.Text == "") schueler.Data.SetZeugnisbemerkungNull();
      else schueler.Data.Zeugnisbemerkung = textBoxZeugnisbemerkung.Text;

      if (cbSchulischeVorbildung.Text == "") schueler.Data.SetSchulischeVorbildungNull();
      else schueler.Data.SchulischeVorbildung = cbSchulischeVorbildung.Text;
      //if (cbEintrittAusSchulart.Text == "") schueler.Data.SetEintrittAusSchulartNull();
      //else schueler.Data.EintrittAusSchulart = cbEintrittAusSchulart.Text;

      if (numericUpDownDeutsch.Value == null) schueler.Data.SetMittlereReifeDeutschnoteNull();
      else schueler.Data.MittlereReifeDeutschnote = (int)numericUpDownDeutsch.Value.GetValueOrDefault();
      if (numericUpDownEnglisch.Value == null) schueler.Data.SetMittlereReifeEnglischnoteNull();
      else schueler.Data.MittlereReifeEnglischnote = (int)numericUpDownEnglisch.Value.GetValueOrDefault();
      if (numericUpDownMathe.Value == null) schueler.Data.SetMittlereReifeMathenoteNull();
      else schueler.Data.MittlereReifeMathenote = (int)numericUpDownMathe.Value.GetValueOrDefault();

      schueler.IsLegastheniker = checkBoxLegasthenie.Checked;
      schueler.Data.LRSZuschlagMin = (int)numLRSZuschlagMin.Value;
      schueler.Data.LRSZuschlagMax = (int)numLRSZuschlagMax.Value;

      schueler.Data.Name = textBoxNachname.Text;
      schueler.Data.Vorname = textBoxVorname.Text;
      schueler.Data.Rufname = textBoxRufname.Text;
      schueler.Data.Ausbildungsrichtung = textBoxAR.Text;
      schueler.Data.Schulart = textBoxFB.Text;
      schueler.Data.SonderfallNur2Hj = chkSonderfallNur2Hj.Checked;

      schueler.Save();
    }

    private void InitializeComponent()
    {
      this.btnSave = new System.Windows.Forms.Button();
      this.groupBoxMittlereReife = new System.Windows.Forms.GroupBox();
      this.cbSchulischeVorbildung = new System.Windows.Forms.ComboBox();
      this.numericUpDownMathe = new diNo.NumericUpDownNullable();
      this.numericUpDownEnglisch = new diNo.NumericUpDownNullable();
      this.numericUpDownDeutsch = new diNo.NumericUpDownNullable();
      this.lblSchulischeVorbildung = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.lbFFalt = new System.Windows.Forms.Label();
      this.gbFS2Art = new System.Windows.Forms.GroupBox();
      this.opFFalt = new System.Windows.Forms.RadioButton();
      this.opErgPr = new System.Windows.Forms.RadioButton();
      this.opRS = new System.Windows.Forms.RadioButton();
      this.cbAndereFremdspr2Fach = new System.Windows.Forms.ComboBox();
      this.label15 = new System.Windows.Forms.Label();
      this.label14 = new System.Windows.Forms.Label();
      this.numAndereFremdspr2Note = new diNo.NumericUpDownNullable();
      this.textBoxZeugnisbemerkung = new System.Windows.Forms.TextBox();
      this.labelZeugnisbemerkung = new System.Windows.Forms.Label();
      this.groupBoxLegasthenie = new System.Windows.Forms.GroupBox();
      this.label8 = new System.Windows.Forms.Label();
      this.numLRSZuschlagMax = new diNo.NumericUpDownNullable();
      this.label6 = new System.Windows.Forms.Label();
      this.numLRSZuschlagMin = new diNo.NumericUpDownNullable();
      this.label5 = new System.Windows.Forms.Label();
      this.checkBoxLegasthenie = new System.Windows.Forms.CheckBox();
      this.grpGrunddaten = new System.Windows.Forms.GroupBox();
      this.chkSonderfallNur2Hj = new System.Windows.Forms.CheckBox();
      this.textBoxFB = new System.Windows.Forms.TextBox();
      this.lbFB = new System.Windows.Forms.Label();
      this.textBoxAR = new System.Windows.Forms.TextBox();
      this.label20 = new System.Windows.Forms.Label();
      this.textBoxID = new System.Windows.Forms.TextBox();
      this.labelID = new System.Windows.Forms.Label();
      this.textBoxRufname = new System.Windows.Forms.TextBox();
      this.label19 = new System.Windows.Forms.Label();
      this.textBoxVorname = new System.Windows.Forms.TextBox();
      this.label18 = new System.Windows.Forms.Label();
      this.textBoxNachname = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.groupBoxMittlereReife.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMathe)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnglisch)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDeutsch)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.gbFS2Art.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numAndereFremdspr2Note)).BeginInit();
      this.groupBoxLegasthenie.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numLRSZuschlagMax)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numLRSZuschlagMin)).BeginInit();
      this.grpGrunddaten.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnSave
      // 
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::diNo.Properties.Resources.Save;
      this.btnSave.Location = new System.Drawing.Point(541, 397);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(40, 40);
      this.btnSave.TabIndex = 27;
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // groupBoxMittlereReife
      // 
      this.groupBoxMittlereReife.Controls.Add(this.cbSchulischeVorbildung);
      this.groupBoxMittlereReife.Controls.Add(this.numericUpDownMathe);
      this.groupBoxMittlereReife.Controls.Add(this.numericUpDownEnglisch);
      this.groupBoxMittlereReife.Controls.Add(this.numericUpDownDeutsch);
      this.groupBoxMittlereReife.Controls.Add(this.lblSchulischeVorbildung);
      this.groupBoxMittlereReife.Controls.Add(this.label4);
      this.groupBoxMittlereReife.Controls.Add(this.label3);
      this.groupBoxMittlereReife.Controls.Add(this.label2);
      this.groupBoxMittlereReife.Location = new System.Drawing.Point(12, 397);
      this.groupBoxMittlereReife.Name = "groupBoxMittlereReife";
      this.groupBoxMittlereReife.Size = new System.Drawing.Size(258, 188);
      this.groupBoxMittlereReife.TabIndex = 28;
      this.groupBoxMittlereReife.TabStop = false;
      this.groupBoxMittlereReife.Text = "mittlere Reife";
      // 
      // cbSchulischeVorbildung
      // 
      this.cbSchulischeVorbildung.FormattingEnabled = true;
      this.cbSchulischeVorbildung.Items.AddRange(new object[] {
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
      this.cbSchulischeVorbildung.Location = new System.Drawing.Point(18, 48);
      this.cbSchulischeVorbildung.Name = "cbSchulischeVorbildung";
      this.cbSchulischeVorbildung.Size = new System.Drawing.Size(105, 21);
      this.cbSchulischeVorbildung.TabIndex = 86;
      // 
      // numericUpDownMathe
      // 
      this.numericUpDownMathe.Location = new System.Drawing.Point(73, 144);
      this.numericUpDownMathe.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
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
      this.numericUpDownEnglisch.Location = new System.Drawing.Point(73, 120);
      this.numericUpDownEnglisch.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
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
      this.numericUpDownDeutsch.Location = new System.Drawing.Point(73, 94);
      this.numericUpDownDeutsch.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
      this.numericUpDownDeutsch.Name = "numericUpDownDeutsch";
      this.numericUpDownDeutsch.Size = new System.Drawing.Size(49, 20);
      this.numericUpDownDeutsch.TabIndex = 5;
      this.numericUpDownDeutsch.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
      // 
      // lblSchulischeVorbildung
      // 
      this.lblSchulischeVorbildung.AutoSize = true;
      this.lblSchulischeVorbildung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSchulischeVorbildung.Location = new System.Drawing.Point(15, 32);
      this.lblSchulischeVorbildung.Name = "lblSchulischeVorbildung";
      this.lblSchulischeVorbildung.Size = new System.Drawing.Size(112, 13);
      this.lblSchulischeVorbildung.TabIndex = 77;
      this.lblSchulischeVorbildung.Text = "Schulische Vorbildung";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(15, 146);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(37, 13);
      this.label4.TabIndex = 4;
      this.label4.Text = "Mathe";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(15, 122);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(47, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Englisch";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(15, 96);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(47, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Deutsch";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.lbFFalt);
      this.groupBox1.Controls.Add(this.gbFS2Art);
      this.groupBox1.Controls.Add(this.cbAndereFremdspr2Fach);
      this.groupBox1.Controls.Add(this.label15);
      this.groupBox1.Controls.Add(this.label14);
      this.groupBox1.Controls.Add(this.numAndereFremdspr2Note);
      this.groupBox1.Location = new System.Drawing.Point(296, 22);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(285, 194);
      this.groupBox1.TabIndex = 67;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Andere 2. Fremdsprache";
      // 
      // lbFFalt
      // 
      this.lbFFalt.AutoSize = true;
      this.lbFFalt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbFFalt.Location = new System.Drawing.Point(14, 164);
      this.lbFFalt.Name = "lbFFalt";
      this.lbFFalt.Size = new System.Drawing.Size(78, 13);
      this.lbFFalt.TabIndex = 89;
      this.lbFFalt.Text = "HjL aus F-f (alt)";
      // 
      // gbFS2Art
      // 
      this.gbFS2Art.Controls.Add(this.opFFalt);
      this.gbFS2Art.Controls.Add(this.opErgPr);
      this.gbFS2Art.Controls.Add(this.opRS);
      this.gbFS2Art.Location = new System.Drawing.Point(14, 19);
      this.gbFS2Art.Name = "gbFS2Art";
      this.gbFS2Art.Size = new System.Drawing.Size(257, 77);
      this.gbFS2Art.TabIndex = 88;
      this.gbFS2Art.TabStop = false;
      // 
      // opFFalt
      // 
      this.opFFalt.AutoSize = true;
      this.opFFalt.Location = new System.Drawing.Point(14, 52);
      this.opFFalt.Name = "opFFalt";
      this.opFFalt.Size = new System.Drawing.Size(174, 17);
      this.opFFalt.TabIndex = 2;
      this.opFFalt.TabStop = true;
      this.opFFalt.Text = "früherer Kurs an unserer Schule";
      this.opFFalt.UseVisualStyleBackColor = true;
      // 
      // opErgPr
      // 
      this.opErgPr.AutoSize = true;
      this.opErgPr.Location = new System.Drawing.Point(14, 34);
      this.opErgPr.Name = "opErgPr";
      this.opErgPr.Size = new System.Drawing.Size(117, 17);
      this.opErgPr.TabIndex = 1;
      this.opErgPr.TabStop = true;
      this.opErgPr.Text = "Ergänzungsprüfung";
      this.opErgPr.UseVisualStyleBackColor = true;
      // 
      // opRS
      // 
      this.opRS.AutoSize = true;
      this.opRS.Checked = true;
      this.opRS.Location = new System.Drawing.Point(14, 16);
      this.opRS.Name = "opRS";
      this.opRS.Size = new System.Drawing.Size(113, 17);
      this.opRS.TabIndex = 0;
      this.opRS.TabStop = true;
      this.opRS.Text = "aus voriger Schule";
      this.opRS.UseVisualStyleBackColor = true;
      // 
      // cbAndereFremdspr2Fach
      // 
      this.cbAndereFremdspr2Fach.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbAndereFremdspr2Fach.FormattingEnabled = true;
      this.cbAndereFremdspr2Fach.Location = new System.Drawing.Point(17, 124);
      this.cbAndereFremdspr2Fach.Name = "cbAndereFremdspr2Fach";
      this.cbAndereFremdspr2Fach.Size = new System.Drawing.Size(185, 21);
      this.cbAndereFremdspr2Fach.TabIndex = 87;
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label15.Location = new System.Drawing.Point(14, 110);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(47, 13);
      this.label15.TabIndex = 70;
      this.label15.Text = "Sprache";
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label14.Location = new System.Drawing.Point(205, 108);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(69, 13);
      this.label14.TabIndex = 69;
      this.label14.Text = "Notenpunkte";
      // 
      // numAndereFremdspr2Note
      // 
      this.numAndereFremdspr2Note.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numAndereFremdspr2Note.Location = new System.Drawing.Point(208, 124);
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
      // textBoxZeugnisbemerkung
      // 
      this.textBoxZeugnisbemerkung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxZeugnisbemerkung.Location = new System.Drawing.Point(296, 258);
      this.textBoxZeugnisbemerkung.Multiline = true;
      this.textBoxZeugnisbemerkung.Name = "textBoxZeugnisbemerkung";
      this.textBoxZeugnisbemerkung.Size = new System.Drawing.Size(285, 117);
      this.textBoxZeugnisbemerkung.TabIndex = 84;
      // 
      // labelZeugnisbemerkung
      // 
      this.labelZeugnisbemerkung.AutoSize = true;
      this.labelZeugnisbemerkung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelZeugnisbemerkung.Location = new System.Drawing.Point(293, 240);
      this.labelZeugnisbemerkung.Name = "labelZeugnisbemerkung";
      this.labelZeugnisbemerkung.Size = new System.Drawing.Size(153, 13);
      this.labelZeugnisbemerkung.TabIndex = 83;
      this.labelZeugnisbemerkung.Text = "zusätzliche Zeugnisbemerkung";
      // 
      // groupBoxLegasthenie
      // 
      this.groupBoxLegasthenie.Controls.Add(this.label8);
      this.groupBoxLegasthenie.Controls.Add(this.numLRSZuschlagMax);
      this.groupBoxLegasthenie.Controls.Add(this.label6);
      this.groupBoxLegasthenie.Controls.Add(this.numLRSZuschlagMin);
      this.groupBoxLegasthenie.Controls.Add(this.label5);
      this.groupBoxLegasthenie.Controls.Add(this.checkBoxLegasthenie);
      this.groupBoxLegasthenie.Location = new System.Drawing.Point(12, 238);
      this.groupBoxLegasthenie.Name = "groupBoxLegasthenie";
      this.groupBoxLegasthenie.Size = new System.Drawing.Size(258, 139);
      this.groupBoxLegasthenie.TabIndex = 86;
      this.groupBoxLegasthenie.TabStop = false;
      this.groupBoxLegasthenie.Text = "Legasthenie";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(15, 83);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(41, 13);
      this.label8.TabIndex = 104;
      this.label8.Text = "minimal";
      // 
      // numLRSZuschlagMax
      // 
      this.numLRSZuschlagMax.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.numLRSZuschlagMax.Location = new System.Drawing.Point(70, 107);
      this.numLRSZuschlagMax.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
      this.numLRSZuschlagMax.Name = "numLRSZuschlagMax";
      this.numLRSZuschlagMax.Size = new System.Drawing.Size(49, 20);
      this.numLRSZuschlagMax.TabIndex = 103;
      this.numLRSZuschlagMax.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(15, 109);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(44, 13);
      this.label6.TabIndex = 102;
      this.label6.Text = "maximal";
      // 
      // numLRSZuschlagMin
      // 
      this.numLRSZuschlagMin.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.numLRSZuschlagMin.Location = new System.Drawing.Point(70, 81);
      this.numLRSZuschlagMin.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
      this.numLRSZuschlagMin.Name = "numLRSZuschlagMin";
      this.numLRSZuschlagMin.Size = new System.Drawing.Size(49, 20);
      this.numLRSZuschlagMin.TabIndex = 101;
      this.numLRSZuschlagMin.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(15, 63);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(67, 13);
      this.label5.TabIndex = 100;
      this.label5.Text = "Zeitzuschlag";
      // 
      // checkBoxLegasthenie
      // 
      this.checkBoxLegasthenie.AutoSize = true;
      this.checkBoxLegasthenie.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
      this.checkBoxLegasthenie.Location = new System.Drawing.Point(18, 31);
      this.checkBoxLegasthenie.Name = "checkBoxLegasthenie";
      this.checkBoxLegasthenie.Size = new System.Drawing.Size(86, 17);
      this.checkBoxLegasthenie.TabIndex = 99;
      this.checkBoxLegasthenie.Text = "Notenschutz";
      this.checkBoxLegasthenie.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
      this.checkBoxLegasthenie.UseVisualStyleBackColor = true;
      // 
      // grpGrunddaten
      // 
      this.grpGrunddaten.Controls.Add(this.chkSonderfallNur2Hj);
      this.grpGrunddaten.Controls.Add(this.textBoxFB);
      this.grpGrunddaten.Controls.Add(this.lbFB);
      this.grpGrunddaten.Controls.Add(this.textBoxAR);
      this.grpGrunddaten.Controls.Add(this.label20);
      this.grpGrunddaten.Controls.Add(this.textBoxID);
      this.grpGrunddaten.Controls.Add(this.labelID);
      this.grpGrunddaten.Controls.Add(this.textBoxRufname);
      this.grpGrunddaten.Controls.Add(this.label19);
      this.grpGrunddaten.Controls.Add(this.textBoxVorname);
      this.grpGrunddaten.Controls.Add(this.label18);
      this.grpGrunddaten.Controls.Add(this.textBoxNachname);
      this.grpGrunddaten.Controls.Add(this.label7);
      this.grpGrunddaten.Location = new System.Drawing.Point(11, 22);
      this.grpGrunddaten.Name = "grpGrunddaten";
      this.grpGrunddaten.Size = new System.Drawing.Size(258, 194);
      this.grpGrunddaten.TabIndex = 87;
      this.grpGrunddaten.TabStop = false;
      this.grpGrunddaten.Text = "Grunddaten";
      // 
      // chkSonderfallNur2Hj
      // 
      this.chkSonderfallNur2Hj.AutoSize = true;
      this.chkSonderfallNur2Hj.Location = new System.Drawing.Point(18, 148);
      this.chkSonderfallNur2Hj.Name = "chkSonderfallNur2Hj";
      this.chkSonderfallNur2Hj.Size = new System.Drawing.Size(148, 17);
      this.chkSonderfallNur2Hj.TabIndex = 107;
      this.chkSonderfallNur2Hj.Text = "Sonderfall nur 2 Halbjahre";
      this.chkSonderfallNur2Hj.UseVisualStyleBackColor = true;
      // 
      // textBoxFB
      // 
      this.textBoxFB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxFB.Location = new System.Drawing.Point(160, 113);
      this.textBoxFB.MaxLength = 1;
      this.textBoxFB.Name = "textBoxFB";
      this.textBoxFB.Size = new System.Drawing.Size(88, 20);
      this.textBoxFB.TabIndex = 105;
      // 
      // lbFB
      // 
      this.lbFB.AutoSize = true;
      this.lbFB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbFB.Location = new System.Drawing.Point(157, 99);
      this.lbFB.Name = "lbFB";
      this.lbFB.Size = new System.Drawing.Size(46, 13);
      this.lbFB.TabIndex = 106;
      this.lbFB.Text = "Schulart";
      // 
      // textBoxAR
      // 
      this.textBoxAR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxAR.Location = new System.Drawing.Point(160, 75);
      this.textBoxAR.MaxLength = 1;
      this.textBoxAR.Name = "textBoxAR";
      this.textBoxAR.Size = new System.Drawing.Size(88, 20);
      this.textBoxAR.TabIndex = 103;
      // 
      // label20
      // 
      this.label20.AutoSize = true;
      this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label20.Location = new System.Drawing.Point(157, 61);
      this.label20.Name = "label20";
      this.label20.Size = new System.Drawing.Size(102, 13);
      this.label20.TabIndex = 104;
      this.label20.Text = "Ausbildungsrichtung";
      // 
      // textBoxID
      // 
      this.textBoxID.Enabled = false;
      this.textBoxID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxID.Location = new System.Drawing.Point(160, 35);
      this.textBoxID.Name = "textBoxID";
      this.textBoxID.Size = new System.Drawing.Size(88, 20);
      this.textBoxID.TabIndex = 101;
      // 
      // labelID
      // 
      this.labelID.AutoSize = true;
      this.labelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelID.Location = new System.Drawing.Point(157, 22);
      this.labelID.Name = "labelID";
      this.labelID.Size = new System.Drawing.Size(57, 13);
      this.labelID.TabIndex = 102;
      this.labelID.Text = "Schüler-ID";
      // 
      // textBoxRufname
      // 
      this.textBoxRufname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxRufname.Location = new System.Drawing.Point(18, 112);
      this.textBoxRufname.Name = "textBoxRufname";
      this.textBoxRufname.Size = new System.Drawing.Size(116, 20);
      this.textBoxRufname.TabIndex = 99;
      // 
      // label19
      // 
      this.label19.AutoSize = true;
      this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label19.Location = new System.Drawing.Point(15, 98);
      this.label19.Name = "label19";
      this.label19.Size = new System.Drawing.Size(50, 13);
      this.label19.TabIndex = 100;
      this.label19.Text = "Rufname";
      // 
      // textBoxVorname
      // 
      this.textBoxVorname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxVorname.Location = new System.Drawing.Point(18, 75);
      this.textBoxVorname.Name = "textBoxVorname";
      this.textBoxVorname.Size = new System.Drawing.Size(116, 20);
      this.textBoxVorname.TabIndex = 97;
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label18.Location = new System.Drawing.Point(15, 61);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(49, 13);
      this.label18.TabIndex = 98;
      this.label18.Text = "Vorname";
      // 
      // textBoxNachname
      // 
      this.textBoxNachname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxNachname.Location = new System.Drawing.Point(18, 35);
      this.textBoxNachname.Name = "textBoxNachname";
      this.textBoxNachname.Size = new System.Drawing.Size(116, 20);
      this.textBoxNachname.TabIndex = 95;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(15, 21);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(59, 13);
      this.label7.TabIndex = 96;
      this.label7.Text = "Nachname";
      // 
      // UserControlSekretariat
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.grpGrunddaten);
      this.Controls.Add(this.groupBoxLegasthenie);
      this.Controls.Add(this.textBoxZeugnisbemerkung);
      this.Controls.Add(this.labelZeugnisbemerkung);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.groupBoxMittlereReife);
      this.Controls.Add(this.btnSave);
      this.Name = "UserControlSekretariat";
      this.Size = new System.Drawing.Size(634, 601);
      this.groupBoxMittlereReife.ResumeLayout(false);
      this.groupBoxMittlereReife.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMathe)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnglisch)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDeutsch)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.gbFS2Art.ResumeLayout(false);
      this.gbFS2Art.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numAndereFremdspr2Note)).EndInit();
      this.groupBoxLegasthenie.ResumeLayout(false);
      this.groupBoxLegasthenie.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numLRSZuschlagMax)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numLRSZuschlagMin)).EndInit();
      this.grpGrunddaten.ResumeLayout(false);
      this.grpGrunddaten.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
  }
}