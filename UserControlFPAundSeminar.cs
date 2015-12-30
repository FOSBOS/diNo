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
          // TODO: Punktewerte übernehmen, dazu brauchen wir ein geeignetes numerisches Control, dass NULL erlaubt.
          }
          else
          {
                textBoxFpABemerkung.Text = "";
                cbFPAErfolg1Hj.SelectedIndex = 0;
                cbFPAErfolg.SelectedIndex = 0;
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
        }

    }
}
