using diNo.diNoDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace diNo
{

    public partial class UserControlFPAundSeminar : UserControl
    {
        private Schueler schueler;

        public UserControlFPAundSeminar()
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
                if (this.schueler != null) {
                    Init();
                }
            }
        }
 
      public void Init() {
          if (pnlFPA.Enabled = schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
          {
                var fpANoten = schueler.FPANoten;                
                textBoxFpABemerkung.Text = fpANoten.IsBemerkungNull() ? "" : fpANoten.Bemerkung;
                cbFPAErfolg1Hj.SelectedIndex = fpANoten.IsErfolg1HjNull() ? 0 : fpANoten.Erfolg1Hj;
                cbFPAErfolg.SelectedIndex = fpANoten.IsErfolgNull() ? 0 : fpANoten.Erfolg;
                numPunkte.Value = fpANoten.IsPunkteNull() ? null : (decimal?) fpANoten.Punkte;
                numPunkte1Hj.Value = fpANoten.IsPunkte1HjNull() ? null : (decimal?) fpANoten.Punkte1Hj;
                numPunkte2Hj.Value = fpANoten.IsPunkte2HjNull() ? null : (decimal?) fpANoten.Punkte2Hj;
          }
          else
          {
                textBoxFpABemerkung.Text = "";
                cbFPAErfolg1Hj.SelectedIndex = 0;
                cbFPAErfolg.SelectedIndex = 0;
                numPunkte.Value = null;
                numPunkte1Hj.Value = null;
                numPunkte2Hj.Value = null;
          }

          if (pnlSeminar.Enabled = schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
          {
            SeminarfachnoteTableAdapter seminarfachAdapter = new SeminarfachnoteTableAdapter();
            var seminarfachnoten = seminarfachAdapter.GetDataBySchuelerId(schueler.Id);
            if (seminarfachnoten.Count == 1)
            {
              //numSeminarfachPunkte.Text = seminarfachnoten[0].Gesamtnote;
              textBoxSeminarfachthemaKurz.Text = seminarfachnoten[0].ThemaKurz;
              textBoxSeminarfachthemaLang.Text = seminarfachnoten[0].ThemaLang;
            }
          }
          else
          {
              textBoxSeminarfachthemaKurz.Text = "";
              textBoxSeminarfachthemaLang.Text = "";
          }  
        }

    public void DatenUebernehmen()
        {
            var fpANoten = schueler.FPANoten;
            if (textBoxFpABemerkung.Text=="") fpANoten.SetBemerkungNull(); else fpANoten.Bemerkung = textBoxFpABemerkung.Text;
            if (cbFPAErfolg1Hj.SelectedIndex==0) fpANoten.SetErfolg1HjNull(); else fpANoten.Erfolg1Hj = cbFPAErfolg1Hj.SelectedIndex;
            if (cbFPAErfolg.SelectedIndex==0) fpANoten.SetErfolgNull(); else fpANoten.Erfolg = cbFPAErfolg.SelectedIndex;                         
            if (numPunkte.Value==null) fpANoten.SetPunkteNull(); else fpANoten.Punkte = (int) numPunkte.Value;
            if (numPunkte1Hj.Value==null) fpANoten.SetPunkte1HjNull(); else fpANoten.Punkte1Hj = (int) numPunkte1Hj.Value;
            if (numPunkte2Hj.Value==null) fpANoten.SetPunkte2HjNull(); else fpANoten.Punkte2Hj = (int) numPunkte2Hj.Value;
        }

    }
}
