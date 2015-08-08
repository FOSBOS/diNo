using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace diNo
{
    public partial class Bericht : Form
    {
        public Bericht()
        {
            InitializeComponent();
        }

        private void Bericht_Load(object sender, EventArgs e)
        {
            // TODO: Diese Codezeile lädt Daten in die Tabelle "diNoDataSet.Lehrer". Sie können sie bei Bedarf verschieben oder entfernen.
            this.LehrerTableAdapter.Fill(this.diNoDataSet.Lehrer);

            this.reportViewer1.RefreshReport();
        }
    }
}
