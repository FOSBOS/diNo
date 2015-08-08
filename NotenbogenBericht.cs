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
    public partial class NotenbogenBericht : Form
    {
        public NotenbogenBericht()
        {
            InitializeComponent();
        }

        private void NotenbogenBericht_Load(object sender, EventArgs e)
        {
            // TODO: Diese Codezeile lädt Daten in die Tabelle "diNoDataSet.Schueler". Sie können sie bei Bedarf verschieben oder entfernen.
            //this.SchuelerTableAdapter.Fill(this.diNoDataSet.Schueler);

            this.reportViewer1.RefreshReport();
        }
    }
}
