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
    public partial class NumericUpDownNullable : NumericUpDown
    {
        public NumericUpDownNullable()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        new public decimal? Value
        {
            get {
                if (Text=="") return null;
                else return Math.Round(base.Value);
            } 
            set
            {               
                if (value == null)
                {
                    Text = "";
                }
                else
                {
                    Text = value.ToString();
                    base.Value = (decimal) value;
                }
            }
        }        
    }
}
