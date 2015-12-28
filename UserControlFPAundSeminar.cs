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
          if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Elf)
          {
            FpANotenTableAdapter fpAAdapter = new FpANotenTableAdapter();
            var fpANoten = fpAAdapter.GetDataBySchuelerId(schueler.Id);
            if (fpANoten.Count == 1)
            {
              textBoxFpABemerkung.Text = fpANoten[0].Bemerkung;
              cbFPAErfolg.SelectedIndex = fpANoten[0].Note;
            }
          }

          if (schueler.getKlasse.Jahrgangsstufe == Jahrgangsstufe.Dreizehn)
          {
            SeminarfachnoteTableAdapter seminarfachAdapter = new SeminarfachnoteTableAdapter();
            var seminarfachnoten = seminarfachAdapter.GetDataBySchuelerId(schueler.Id);
            if (seminarfachnoten.Count == 1)
            {
              numSeminarfachPunkte.Value = seminarfachnoten[0].Gesamtnote;
              textBoxSeminarfachthemaKurz.Text = seminarfachnoten[0].ThemaKurz;
              textBoxSeminarfachthemaLang.Text = seminarfachnoten[0].ThemaLang;
            }
          }
        }
    }
}
