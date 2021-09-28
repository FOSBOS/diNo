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

          listBoxZuletztBesuchteSchulart.SelectedItem = schueler.Data.IsSchulischeVorbildungNull() ? null : schueler.Data.SchulischeVorbildung;
          textBoxZeugnisbemerkung.Text = schueler.Data.IsZeugnisbemerkungNull() ? "" : schueler.Data.Zeugnisbemerkung;

          listBoxMittlereReifeSchulart.SelectedItem = schueler.Data.IsEintrittAusSchulartNull() ? null : schueler.Data.EintrittAusSchulart;
          numericUpDownDeutsch.Value = schueler.Data.IsMittlereReifeDeutschnoteNull() ? null : (decimal?)schueler.Data.MittlereReifeDeutschnote;
          numericUpDownEnglisch.Value = schueler.Data.IsMittlereReifeEnglischnoteNull() ? null : (decimal?)schueler.Data.MittlereReifeEnglischnote;
          numericUpDownMathe.Value = schueler.Data.IsMittlereReifeMathenoteNull() ? null : (decimal?)schueler.Data.MittlereReifeMathenote;

          textBoxID.Text = schueler.Id.ToString();
          checkBoxLegasthenie.Checked = schueler.IsLegastheniker;
          checkBoxSonderfall2Hj.Checked = schueler.Data.SonderfallNur2Hj;
          textBoxNachname.Text = schueler.Data.Name;
          textBoxVorname.Text = schueler.Data.Vorname;
          textBoxRufname.Text = schueler.Data.Rufname;
          textBoxAR.Text = schueler.Data.Ausbildungsrichtung;
          textBoxFB.Text = schueler.Data.Schulart;
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

      schueler.Data.SchulischeVorbildung = (listBoxZuletztBesuchteSchulart.SelectedItem != null) ? (String)listBoxZuletztBesuchteSchulart.SelectedItem : null;
      schueler.Data.EintrittAusSchulart = (listBoxMittlereReifeSchulart.SelectedItem != null) ? (String)listBoxMittlereReifeSchulart.SelectedItem : null;

      if (numericUpDownDeutsch.Value == null) schueler.Data.SetMittlereReifeDeutschnoteNull();
      else schueler.Data.MittlereReifeDeutschnote = (int)numericUpDownDeutsch.Value.GetValueOrDefault();
      if (numericUpDownEnglisch.Value == null) schueler.Data.SetMittlereReifeEnglischnoteNull();
      else schueler.Data.MittlereReifeEnglischnote = (int)numericUpDownEnglisch.Value.GetValueOrDefault();
      if (numericUpDownMathe.Value == null) schueler.Data.SetMittlereReifeMathenoteNull();
      else schueler.Data.MittlereReifeMathenote = (int)numericUpDownMathe.Value.GetValueOrDefault();

      schueler.IsLegastheniker = checkBoxLegasthenie.Checked;
      schueler.Data.SonderfallNur2Hj = checkBoxSonderfall2Hj.Checked;
      schueler.Data.Name = textBoxNachname.Text;
      schueler.Data.Vorname = textBoxVorname.Text;
      schueler.Data.Rufname = textBoxRufname.Text;
      schueler.Data.Ausbildungsrichtung = textBoxAR.Text;
      schueler.Data.Schulart = textBoxFB.Text;

      schueler.Save();
    }

    private void InitializeComponent()
    {
      this.listBoxMittlereReifeSchulart = new System.Windows.Forms.ListBox();
      this.btnSave = new System.Windows.Forms.Button();
      this.groupBoxMittlereReife = new System.Windows.Forms.GroupBox();
      this.listBoxZuletztBesuchteSchulart = new System.Windows.Forms.ListBox();
      this.numericUpDownMathe = new diNo.NumericUpDownNullable();
      this.numericUpDownEnglisch = new diNo.NumericUpDownNullable();
      this.numericUpDownDeutsch = new diNo.NumericUpDownNullable();
      this.lblSchulischeVorbildung = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
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
      this.panelSekretariat = new System.Windows.Forms.Panel();
      this.checkBoxSonderfall2Hj = new System.Windows.Forms.CheckBox();
      this.textBoxAR = new System.Windows.Forms.TextBox();
      this.label20 = new System.Windows.Forms.Label();
      this.textBoxID = new System.Windows.Forms.TextBox();
      this.labelID = new System.Windows.Forms.Label();
      this.checkBoxLegasthenie = new System.Windows.Forms.CheckBox();
      this.textBoxRufname = new System.Windows.Forms.TextBox();
      this.label19 = new System.Windows.Forms.Label();
      this.textBoxVorname = new System.Windows.Forms.TextBox();
      this.label18 = new System.Windows.Forms.Label();
      this.textBoxNachname = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.textBoxFB = new System.Windows.Forms.TextBox();
      this.lbFB = new System.Windows.Forms.Label();
      this.groupBoxMittlereReife.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMathe)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEnglisch)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDeutsch)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.gbFS2Art.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numAndereFremdspr2Note)).BeginInit();
      this.panelSekretariat.SuspendLayout();
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
      this.groupBoxMittlereReife.Location = new System.Drawing.Point(3, 208);
      this.groupBoxMittlereReife.Name = "groupBoxMittlereReife";
      this.groupBoxMittlereReife.Size = new System.Drawing.Size(258, 271);
      this.groupBoxMittlereReife.TabIndex = 28;
      this.groupBoxMittlereReife.TabStop = false;
      this.groupBoxMittlereReife.Text = "mittlere Reife";
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
      this.textBoxZeugnisbemerkung.Location = new System.Drawing.Point(296, 274);
      this.textBoxZeugnisbemerkung.Multiline = true;
      this.textBoxZeugnisbemerkung.Name = "textBoxZeugnisbemerkung";
      this.textBoxZeugnisbemerkung.Size = new System.Drawing.Size(285, 89);
      this.textBoxZeugnisbemerkung.TabIndex = 84;
      // 
      // labelZeugnisbemerkung
      // 
      this.labelZeugnisbemerkung.AutoSize = true;
      this.labelZeugnisbemerkung.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelZeugnisbemerkung.Location = new System.Drawing.Point(293, 258);
      this.labelZeugnisbemerkung.Name = "labelZeugnisbemerkung";
      this.labelZeugnisbemerkung.Size = new System.Drawing.Size(153, 13);
      this.labelZeugnisbemerkung.TabIndex = 83;
      this.labelZeugnisbemerkung.Text = "zusätzliche Zeugnisbemerkung";
      // 
      // panelSekretariat
      // 
      this.panelSekretariat.Controls.Add(this.textBoxFB);
      this.panelSekretariat.Controls.Add(this.lbFB);
      this.panelSekretariat.Controls.Add(this.checkBoxSonderfall2Hj);
      this.panelSekretariat.Controls.Add(this.textBoxAR);
      this.panelSekretariat.Controls.Add(this.label20);
      this.panelSekretariat.Controls.Add(this.textBoxID);
      this.panelSekretariat.Controls.Add(this.labelID);
      this.panelSekretariat.Controls.Add(this.checkBoxLegasthenie);
      this.panelSekretariat.Controls.Add(this.textBoxRufname);
      this.panelSekretariat.Controls.Add(this.label19);
      this.panelSekretariat.Controls.Add(this.textBoxVorname);
      this.panelSekretariat.Controls.Add(this.label18);
      this.panelSekretariat.Controls.Add(this.textBoxNachname);
      this.panelSekretariat.Controls.Add(this.label7);
      this.panelSekretariat.Location = new System.Drawing.Point(9, 22);
      this.panelSekretariat.Name = "panelSekretariat";
      this.panelSekretariat.Size = new System.Drawing.Size(260, 166);
      this.panelSekretariat.TabIndex = 85;
      // 
      // checkBoxSonderfall2Hj
      // 
      this.checkBoxSonderfall2Hj.AutoSize = true;
      this.checkBoxSonderfall2Hj.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkBoxSonderfall2Hj.Location = new System.Drawing.Point(128, 130);
      this.checkBoxSonderfall2Hj.Name = "checkBoxSonderfall2Hj";
      this.checkBoxSonderfall2Hj.Size = new System.Drawing.Size(113, 17);
      this.checkBoxSonderfall2Hj.TabIndex = 91;
      this.checkBoxSonderfall2Hj.Text = "Sonderfall nur 2 Hj";
      this.checkBoxSonderfall2Hj.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
      this.checkBoxSonderfall2Hj.UseVisualStyleBackColor = true;
      // 
      // textBoxAR
      // 
      this.textBoxAR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxAR.Location = new System.Drawing.Point(153, 58);
      this.textBoxAR.MaxLength = 1;
      this.textBoxAR.Name = "textBoxAR";
      this.textBoxAR.Size = new System.Drawing.Size(88, 20);
      this.textBoxAR.TabIndex = 89;
      // 
      // label20
      // 
      this.label20.AutoSize = true;
      this.label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label20.Location = new System.Drawing.Point(150, 44);
      this.label20.Name = "label20";
      this.label20.Size = new System.Drawing.Size(102, 13);
      this.label20.TabIndex = 90;
      this.label20.Text = "Ausbildungsrichtung";
      // 
      // textBoxID
      // 
      this.textBoxID.Enabled = false;
      this.textBoxID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxID.Location = new System.Drawing.Point(153, 18);
      this.textBoxID.Name = "textBoxID";
      this.textBoxID.Size = new System.Drawing.Size(88, 20);
      this.textBoxID.TabIndex = 87;
      // 
      // labelID
      // 
      this.labelID.AutoSize = true;
      this.labelID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.labelID.Location = new System.Drawing.Point(150, 5);
      this.labelID.Name = "labelID";
      this.labelID.Size = new System.Drawing.Size(57, 13);
      this.labelID.TabIndex = 88;
      this.labelID.Text = "Schüler-ID";
      // 
      // checkBoxLegasthenie
      // 
      this.checkBoxLegasthenie.AutoSize = true;
      this.checkBoxLegasthenie.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.checkBoxLegasthenie.Location = new System.Drawing.Point(25, 130);
      this.checkBoxLegasthenie.Name = "checkBoxLegasthenie";
      this.checkBoxLegasthenie.Size = new System.Drawing.Size(93, 17);
      this.checkBoxLegasthenie.TabIndex = 80;
      this.checkBoxLegasthenie.Text = "Legasthenie   ";
      this.checkBoxLegasthenie.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
      this.checkBoxLegasthenie.UseVisualStyleBackColor = true;
      // 
      // textBoxRufname
      // 
      this.textBoxRufname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxRufname.Location = new System.Drawing.Point(2, 96);
      this.textBoxRufname.Name = "textBoxRufname";
      this.textBoxRufname.Size = new System.Drawing.Size(116, 20);
      this.textBoxRufname.TabIndex = 78;
      // 
      // label19
      // 
      this.label19.AutoSize = true;
      this.label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label19.Location = new System.Drawing.Point(-1, 82);
      this.label19.Name = "label19";
      this.label19.Size = new System.Drawing.Size(50, 13);
      this.label19.TabIndex = 79;
      this.label19.Text = "Rufname";
      // 
      // textBoxVorname
      // 
      this.textBoxVorname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxVorname.Location = new System.Drawing.Point(2, 59);
      this.textBoxVorname.Name = "textBoxVorname";
      this.textBoxVorname.Size = new System.Drawing.Size(116, 20);
      this.textBoxVorname.TabIndex = 76;
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label18.Location = new System.Drawing.Point(-1, 45);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(49, 13);
      this.label18.TabIndex = 77;
      this.label18.Text = "Vorname";
      // 
      // textBoxNachname
      // 
      this.textBoxNachname.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxNachname.Location = new System.Drawing.Point(2, 19);
      this.textBoxNachname.Name = "textBoxNachname";
      this.textBoxNachname.Size = new System.Drawing.Size(116, 20);
      this.textBoxNachname.TabIndex = 74;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(-1, 5);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(59, 13);
      this.label7.TabIndex = 75;
      this.label7.Text = "Nachname";
      // 
      // textBoxFB
      // 
      this.textBoxFB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBoxFB.Location = new System.Drawing.Point(153, 96);
      this.textBoxFB.MaxLength = 1;
      this.textBoxFB.Name = "textBoxFB";
      this.textBoxFB.Size = new System.Drawing.Size(88, 20);
      this.textBoxFB.TabIndex = 92;
      // 
      // lbFB
      // 
      this.lbFB.AutoSize = true;
      this.lbFB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lbFB.Location = new System.Drawing.Point(150, 82);
      this.lbFB.Name = "lbFB";
      this.lbFB.Size = new System.Drawing.Size(46, 13);
      this.lbFB.TabIndex = 93;
      this.lbFB.Text = "Schulart";
      // 
      // UserControlSekretariat
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.panelSekretariat);
      this.Controls.Add(this.textBoxZeugnisbemerkung);
      this.Controls.Add(this.labelZeugnisbemerkung);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.groupBoxMittlereReife);
      this.Controls.Add(this.btnSave);
      this.Name = "UserControlSekretariat";
      this.Size = new System.Drawing.Size(621, 484);
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
      this.panelSekretariat.ResumeLayout(false);
      this.panelSekretariat.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
  }
}