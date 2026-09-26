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
          checkBoxFranzB1.Checked = !schueler.Data.IsFranzB1Null() && schueler.Data.FranzB1;
          checkBoxSpanischB1.Checked = !schueler.Data.IsSpanischB1Null() && schueler.Data.SpanischB1;

          textBoxID.Text = schueler.Id.ToString();
          checkBoxLegasthenie.Checked = schueler.Data.LRSStoerung;
          numLRSZuschlagMin.Value = schueler.Data.LRSZuschlagMin;
          numLRSZuschlagMax.Value = schueler.Data.LRSZuschlagMax;
          textBoxNachname.Text = schueler.Data.Name;
          textBoxVorname.Text = schueler.Data.Vorname;
          textBoxRufname.Text = schueler.Data.Rufname;
          textBoxAR.Text = schueler.Data.Ausbildungsrichtung;
          textBoxFB.Text = schueler.Data.Schulart;
          textBoxASVID.Text = schueler.AsvId;

          LadeElternteil("1", textBoxEltern1Vorname, textBoxEltern1Nachname, textBoxEltern1Telefon, textBoxEltern1Email, checkBoxEltern1Haupt);
          LadeElternteil("2", textBoxEltern2Vorname, textBoxEltern2Nachname, textBoxEltern2Telefon, textBoxEltern2Email, checkBoxEltern2Haupt);
        }
      }
    }

    private void LadeElternteil(string anschriftWessen, TextBox vorname, TextBox nachname, TextBox telefon, TextBox email, CheckBox hauptAnsprechpartner)
    {
      var row = schueler.getAnschriftenRows().FirstOrDefault(a => a.AnschriftWessen == anschriftWessen);
      vorname.Text = row == null || row.IsVornamePersonNull() ? "" : row.VornamePerson;
      nachname.Text = row == null || row.IsNachnamePersonNull() ? "" : row.NachnamePerson;
      telefon.Text = row == null || row.IsTelefonnummerNull() ? "" : row.Telefonnummer;
      email.Text = row == null || row.IsEmailNull() ? "" : row.Email;
      hauptAnsprechpartner.Checked = row != null && !row.IsHauptAnsprechpartnerNull() && row.HauptAnsprechpartner;
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
      schueler.Data.FranzB1 = checkBoxFranzB1.Checked;
      schueler.Data.SpanischB1 = checkBoxSpanischB1.Checked;

      schueler.Data.LRSStoerung = checkBoxLegasthenie.Checked;
      schueler.Data.LRSZuschlagMin = (int)numLRSZuschlagMin.Value;
      schueler.Data.LRSZuschlagMax = (int)numLRSZuschlagMax.Value;

      schueler.Data.Name = textBoxNachname.Text;
      schueler.Data.Vorname = textBoxVorname.Text;
      schueler.Data.Rufname = textBoxRufname.Text;
      schueler.Data.Ausbildungsrichtung = textBoxAR.Text;
      schueler.Data.Schulart = textBoxFB.Text;
      schueler.Data.asv_id = textBoxASVID.Text;

      schueler.SaveErziehungsberechtigter("1", textBoxEltern1Vorname.Text.Trim(), textBoxEltern1Nachname.Text.Trim(), textBoxEltern1Telefon.Text.Trim(), textBoxEltern1Email.Text.Trim(), checkBoxEltern1Haupt.Checked);
      schueler.SaveErziehungsberechtigter("2", textBoxEltern2Vorname.Text.Trim(), textBoxEltern2Nachname.Text.Trim(), textBoxEltern2Telefon.Text.Trim(), textBoxEltern2Email.Text.Trim(), checkBoxEltern2Haupt.Checked);

      schueler.Save();
    }

    private void InitializeComponent()
    {
      this.btnSave = new System.Windows.Forms.Button();
      this.groupBoxMittlereReife = new System.Windows.Forms.GroupBox();
      this.cbSchulischeVorbildung = new System.Windows.Forms.ComboBox();
      this.lblSchulischeVorbildung = new System.Windows.Forms.Label();
      this.checkBoxFranzB1 = new System.Windows.Forms.CheckBox();
      this.checkBoxSpanischB1 = new System.Windows.Forms.CheckBox();
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
      this.textBoxASVID = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
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
      this.groupBoxEltern = new System.Windows.Forms.GroupBox();
      this.lblEltern1 = new System.Windows.Forms.Label();
      this.lblEltern1Vorname = new System.Windows.Forms.Label();
      this.lblEltern1Nachname = new System.Windows.Forms.Label();
      this.textBoxEltern1Vorname = new System.Windows.Forms.TextBox();
      this.textBoxEltern1Nachname = new System.Windows.Forms.TextBox();
      this.lblEltern1Telefon = new System.Windows.Forms.Label();
      this.textBoxEltern1Telefon = new System.Windows.Forms.TextBox();
      this.checkBoxEltern1Haupt = new System.Windows.Forms.CheckBox();
      this.lblEltern1Email = new System.Windows.Forms.Label();
      this.textBoxEltern1Email = new System.Windows.Forms.TextBox();
      this.lblEltern2 = new System.Windows.Forms.Label();
      this.lblEltern2Vorname = new System.Windows.Forms.Label();
      this.lblEltern2Nachname = new System.Windows.Forms.Label();
      this.textBoxEltern2Vorname = new System.Windows.Forms.TextBox();
      this.textBoxEltern2Nachname = new System.Windows.Forms.TextBox();
      this.lblEltern2Telefon = new System.Windows.Forms.Label();
      this.textBoxEltern2Telefon = new System.Windows.Forms.TextBox();
      this.checkBoxEltern2Haupt = new System.Windows.Forms.CheckBox();
      this.lblEltern2Email = new System.Windows.Forms.Label();
      this.textBoxEltern2Email = new System.Windows.Forms.TextBox();
      this.groupBoxMittlereReife.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.gbFS2Art.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numAndereFremdspr2Note)).BeginInit();
      this.groupBoxLegasthenie.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numLRSZuschlagMax)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numLRSZuschlagMin)).BeginInit();
      this.grpGrunddaten.SuspendLayout();
      this.groupBoxEltern.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnSave
      // 
      this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnSave.Image = global::diNo.Properties.Resources.Save;
      this.btnSave.Location = new System.Drawing.Point(541, 727);
      this.btnSave.Name = "btnSave";
      this.btnSave.Size = new System.Drawing.Size(40, 40);
      this.btnSave.TabIndex = 27;
      this.btnSave.UseVisualStyleBackColor = true;
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // groupBoxMittlereReife
      // 
      this.groupBoxMittlereReife.Controls.Add(this.cbSchulischeVorbildung);
      this.groupBoxMittlereReife.Controls.Add(this.lblSchulischeVorbildung);
      this.groupBoxMittlereReife.Controls.Add(this.checkBoxFranzB1);
      this.groupBoxMittlereReife.Controls.Add(this.checkBoxSpanischB1);
      this.groupBoxMittlereReife.Location = new System.Drawing.Point(12, 397);
      this.groupBoxMittlereReife.Name = "groupBoxMittlereReife";
      this.groupBoxMittlereReife.Size = new System.Drawing.Size(258, 188);
      this.groupBoxMittlereReife.TabIndex = 28;
      this.groupBoxMittlereReife.TabStop = false;
      this.groupBoxMittlereReife.Text = "Schulische Vorbildung";
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
      // checkBoxFranzB1
      //
      this.checkBoxFranzB1.AutoSize = true;
      this.checkBoxFranzB1.Location = new System.Drawing.Point(18, 96);
      this.checkBoxFranzB1.Name = "checkBoxFranzB1";
      this.checkBoxFranzB1.Size = new System.Drawing.Size(97, 17);
      this.checkBoxFranzB1.TabIndex = 100;
      this.checkBoxFranzB1.Text = "Französisch B1";
      this.checkBoxFranzB1.UseVisualStyleBackColor = true;
      //
      // checkBoxSpanischB1
      //
      this.checkBoxSpanischB1.AutoSize = true;
      this.checkBoxSpanischB1.Location = new System.Drawing.Point(18, 121);
      this.checkBoxSpanischB1.Name = "checkBoxSpanischB1";
      this.checkBoxSpanischB1.Size = new System.Drawing.Size(89, 17);
      this.checkBoxSpanischB1.TabIndex = 101;
      this.checkBoxSpanischB1.Text = "Spanisch B1";
      this.checkBoxSpanischB1.UseVisualStyleBackColor = true;
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
      this.grpGrunddaten.Controls.Add(this.textBoxASVID);
      this.grpGrunddaten.Controls.Add(this.label1);
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
      // textBoxASVID
      // 
      this.textBoxASVID.Enabled = false;
      this.textBoxASVID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxASVID.Location = new System.Drawing.Point(160, 76);
      this.textBoxASVID.Name = "textBoxASVID";
      this.textBoxASVID.Size = new System.Drawing.Size(88, 20);
      this.textBoxASVID.TabIndex = 107;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(157, 63);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(42, 13);
      this.label1.TabIndex = 108;
      this.label1.Text = "ASV-ID";
      // 
      // textBoxFB
      // 
      this.textBoxFB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxFB.Location = new System.Drawing.Point(160, 157);
      this.textBoxFB.MaxLength = 1;
      this.textBoxFB.Name = "textBoxFB";
      this.textBoxFB.Size = new System.Drawing.Size(88, 20);
      this.textBoxFB.TabIndex = 105;
      // 
      // lbFB
      // 
      this.lbFB.AutoSize = true;
      this.lbFB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbFB.Location = new System.Drawing.Point(157, 143);
      this.lbFB.Name = "lbFB";
      this.lbFB.Size = new System.Drawing.Size(46, 13);
      this.lbFB.TabIndex = 106;
      this.lbFB.Text = "Schulart";
      // 
      // textBoxAR
      // 
      this.textBoxAR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxAR.Location = new System.Drawing.Point(17, 157);
      this.textBoxAR.MaxLength = 1;
      this.textBoxAR.Name = "textBoxAR";
      this.textBoxAR.Size = new System.Drawing.Size(117, 20);
      this.textBoxAR.TabIndex = 103;
      // 
      // label20
      // 
      this.label20.AutoSize = true;
      this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label20.Location = new System.Drawing.Point(14, 141);
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
      // groupBoxEltern
      //
      this.groupBoxEltern.Controls.Add(this.lblEltern1);
      this.groupBoxEltern.Controls.Add(this.lblEltern1Vorname);
      this.groupBoxEltern.Controls.Add(this.lblEltern1Nachname);
      this.groupBoxEltern.Controls.Add(this.textBoxEltern1Vorname);
      this.groupBoxEltern.Controls.Add(this.textBoxEltern1Nachname);
      this.groupBoxEltern.Controls.Add(this.lblEltern1Telefon);
      this.groupBoxEltern.Controls.Add(this.textBoxEltern1Telefon);
      this.groupBoxEltern.Controls.Add(this.checkBoxEltern1Haupt);
      this.groupBoxEltern.Controls.Add(this.lblEltern1Email);
      this.groupBoxEltern.Controls.Add(this.textBoxEltern1Email);
      this.groupBoxEltern.Controls.Add(this.lblEltern2);
      this.groupBoxEltern.Controls.Add(this.lblEltern2Vorname);
      this.groupBoxEltern.Controls.Add(this.lblEltern2Nachname);
      this.groupBoxEltern.Controls.Add(this.textBoxEltern2Vorname);
      this.groupBoxEltern.Controls.Add(this.textBoxEltern2Nachname);
      this.groupBoxEltern.Controls.Add(this.lblEltern2Telefon);
      this.groupBoxEltern.Controls.Add(this.textBoxEltern2Telefon);
      this.groupBoxEltern.Controls.Add(this.checkBoxEltern2Haupt);
      this.groupBoxEltern.Controls.Add(this.lblEltern2Email);
      this.groupBoxEltern.Controls.Add(this.textBoxEltern2Email);
      this.groupBoxEltern.Location = new System.Drawing.Point(296, 397);
      this.groupBoxEltern.Name = "groupBoxEltern";
      this.groupBoxEltern.Size = new System.Drawing.Size(285, 320);
      this.groupBoxEltern.TabIndex = 109;
      this.groupBoxEltern.TabStop = false;
      this.groupBoxEltern.Text = "Eltern";
      //
      // lblEltern1
      //
      this.lblEltern1.AutoSize = true;
      this.lblEltern1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEltern1.Location = new System.Drawing.Point(11, 20);
      this.lblEltern1.Name = "lblEltern1";
      this.lblEltern1.Size = new System.Drawing.Size(127, 13);
      this.lblEltern1.TabIndex = 110;
      this.lblEltern1.Text = "1. Erziehungsberechtigte(r)";
      //
      // lblEltern1Vorname
      //
      this.lblEltern1Vorname.AutoSize = true;
      this.lblEltern1Vorname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEltern1Vorname.Location = new System.Drawing.Point(14, 38);
      this.lblEltern1Vorname.Name = "lblEltern1Vorname";
      this.lblEltern1Vorname.Size = new System.Drawing.Size(49, 13);
      this.lblEltern1Vorname.TabIndex = 111;
      this.lblEltern1Vorname.Text = "Vorname";
      //
      // lblEltern1Nachname
      //
      this.lblEltern1Nachname.AutoSize = true;
      this.lblEltern1Nachname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEltern1Nachname.Location = new System.Drawing.Point(146, 38);
      this.lblEltern1Nachname.Name = "lblEltern1Nachname";
      this.lblEltern1Nachname.Size = new System.Drawing.Size(59, 13);
      this.lblEltern1Nachname.TabIndex = 112;
      this.lblEltern1Nachname.Text = "Nachname";
      //
      // textBoxEltern1Vorname
      //
      this.textBoxEltern1Vorname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxEltern1Vorname.Location = new System.Drawing.Point(14, 52);
      this.textBoxEltern1Vorname.Name = "textBoxEltern1Vorname";
      this.textBoxEltern1Vorname.Size = new System.Drawing.Size(122, 20);
      this.textBoxEltern1Vorname.TabIndex = 113;
      //
      // textBoxEltern1Nachname
      //
      this.textBoxEltern1Nachname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxEltern1Nachname.Location = new System.Drawing.Point(146, 52);
      this.textBoxEltern1Nachname.Name = "textBoxEltern1Nachname";
      this.textBoxEltern1Nachname.Size = new System.Drawing.Size(122, 20);
      this.textBoxEltern1Nachname.TabIndex = 114;
      //
      // lblEltern1Telefon
      //
      this.lblEltern1Telefon.AutoSize = true;
      this.lblEltern1Telefon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEltern1Telefon.Location = new System.Drawing.Point(14, 78);
      this.lblEltern1Telefon.Name = "lblEltern1Telefon";
      this.lblEltern1Telefon.Size = new System.Drawing.Size(45, 13);
      this.lblEltern1Telefon.TabIndex = 115;
      this.lblEltern1Telefon.Text = "Telefon";
      //
      // textBoxEltern1Telefon
      //
      this.textBoxEltern1Telefon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxEltern1Telefon.Location = new System.Drawing.Point(14, 92);
      this.textBoxEltern1Telefon.Name = "textBoxEltern1Telefon";
      this.textBoxEltern1Telefon.Size = new System.Drawing.Size(122, 20);
      this.textBoxEltern1Telefon.TabIndex = 116;
      //
      // checkBoxEltern1Haupt
      //
      this.checkBoxEltern1Haupt.AutoSize = true;
      this.checkBoxEltern1Haupt.Location = new System.Drawing.Point(146, 94);
      this.checkBoxEltern1Haupt.Name = "checkBoxEltern1Haupt";
      this.checkBoxEltern1Haupt.Size = new System.Drawing.Size(133, 17);
      this.checkBoxEltern1Haupt.TabIndex = 117;
      this.checkBoxEltern1Haupt.Text = "Hauptansprechpartner";
      this.checkBoxEltern1Haupt.UseVisualStyleBackColor = true;
      //
      // lblEltern1Email
      //
      this.lblEltern1Email.AutoSize = true;
      this.lblEltern1Email.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEltern1Email.Location = new System.Drawing.Point(14, 118);
      this.lblEltern1Email.Name = "lblEltern1Email";
      this.lblEltern1Email.Size = new System.Drawing.Size(34, 13);
      this.lblEltern1Email.TabIndex = 118;
      this.lblEltern1Email.Text = "Email";
      //
      // textBoxEltern1Email
      //
      this.textBoxEltern1Email.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxEltern1Email.Location = new System.Drawing.Point(14, 132);
      this.textBoxEltern1Email.Name = "textBoxEltern1Email";
      this.textBoxEltern1Email.Size = new System.Drawing.Size(254, 20);
      this.textBoxEltern1Email.TabIndex = 119;
      //
      // lblEltern2
      //
      this.lblEltern2.AutoSize = true;
      this.lblEltern2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEltern2.Location = new System.Drawing.Point(11, 167);
      this.lblEltern2.Name = "lblEltern2";
      this.lblEltern2.Size = new System.Drawing.Size(157, 13);
      this.lblEltern2.TabIndex = 120;
      this.lblEltern2.Text = "2. Erziehungsberechtigte(r)";
      //
      // lblEltern2Vorname
      //
      this.lblEltern2Vorname.AutoSize = true;
      this.lblEltern2Vorname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEltern2Vorname.Location = new System.Drawing.Point(14, 185);
      this.lblEltern2Vorname.Name = "lblEltern2Vorname";
      this.lblEltern2Vorname.Size = new System.Drawing.Size(49, 13);
      this.lblEltern2Vorname.TabIndex = 121;
      this.lblEltern2Vorname.Text = "Vorname";
      //
      // lblEltern2Nachname
      //
      this.lblEltern2Nachname.AutoSize = true;
      this.lblEltern2Nachname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEltern2Nachname.Location = new System.Drawing.Point(146, 185);
      this.lblEltern2Nachname.Name = "lblEltern2Nachname";
      this.lblEltern2Nachname.Size = new System.Drawing.Size(59, 13);
      this.lblEltern2Nachname.TabIndex = 122;
      this.lblEltern2Nachname.Text = "Nachname";
      //
      // textBoxEltern2Vorname
      //
      this.textBoxEltern2Vorname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxEltern2Vorname.Location = new System.Drawing.Point(14, 199);
      this.textBoxEltern2Vorname.Name = "textBoxEltern2Vorname";
      this.textBoxEltern2Vorname.Size = new System.Drawing.Size(122, 20);
      this.textBoxEltern2Vorname.TabIndex = 123;
      //
      // textBoxEltern2Nachname
      //
      this.textBoxEltern2Nachname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxEltern2Nachname.Location = new System.Drawing.Point(146, 199);
      this.textBoxEltern2Nachname.Name = "textBoxEltern2Nachname";
      this.textBoxEltern2Nachname.Size = new System.Drawing.Size(122, 20);
      this.textBoxEltern2Nachname.TabIndex = 124;
      //
      // lblEltern2Telefon
      //
      this.lblEltern2Telefon.AutoSize = true;
      this.lblEltern2Telefon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEltern2Telefon.Location = new System.Drawing.Point(14, 225);
      this.lblEltern2Telefon.Name = "lblEltern2Telefon";
      this.lblEltern2Telefon.Size = new System.Drawing.Size(45, 13);
      this.lblEltern2Telefon.TabIndex = 125;
      this.lblEltern2Telefon.Text = "Telefon";
      //
      // textBoxEltern2Telefon
      //
      this.textBoxEltern2Telefon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxEltern2Telefon.Location = new System.Drawing.Point(14, 239);
      this.textBoxEltern2Telefon.Name = "textBoxEltern2Telefon";
      this.textBoxEltern2Telefon.Size = new System.Drawing.Size(122, 20);
      this.textBoxEltern2Telefon.TabIndex = 126;
      //
      // checkBoxEltern2Haupt
      //
      this.checkBoxEltern2Haupt.AutoSize = true;
      this.checkBoxEltern2Haupt.Location = new System.Drawing.Point(146, 241);
      this.checkBoxEltern2Haupt.Name = "checkBoxEltern2Haupt";
      this.checkBoxEltern2Haupt.Size = new System.Drawing.Size(133, 17);
      this.checkBoxEltern2Haupt.TabIndex = 127;
      this.checkBoxEltern2Haupt.Text = "Hauptansprechpartner";
      this.checkBoxEltern2Haupt.UseVisualStyleBackColor = true;
      //
      // lblEltern2Email
      //
      this.lblEltern2Email.AutoSize = true;
      this.lblEltern2Email.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblEltern2Email.Location = new System.Drawing.Point(14, 265);
      this.lblEltern2Email.Name = "lblEltern2Email";
      this.lblEltern2Email.Size = new System.Drawing.Size(34, 13);
      this.lblEltern2Email.TabIndex = 128;
      this.lblEltern2Email.Text = "Email";
      //
      // textBoxEltern2Email
      //
      this.textBoxEltern2Email.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxEltern2Email.Location = new System.Drawing.Point(14, 279);
      this.textBoxEltern2Email.Name = "textBoxEltern2Email";
      this.textBoxEltern2Email.Size = new System.Drawing.Size(254, 20);
      this.textBoxEltern2Email.TabIndex = 129;
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
      this.Controls.Add(this.groupBoxEltern);
      this.Controls.Add(this.btnSave);
      this.Name = "UserControlSekretariat";
      this.Size = new System.Drawing.Size(634, 785);
      this.groupBoxMittlereReife.ResumeLayout(false);
      this.groupBoxMittlereReife.PerformLayout();
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
      this.groupBoxEltern.ResumeLayout(false);
      this.groupBoxEltern.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
  }
}