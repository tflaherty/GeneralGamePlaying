using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;


namespace API.SWIProlog.SWIPrologServiceTest
{
    public partial class Form1 : Form
    {
        protected override void OnLoad(EventArgs e)
        { 

            using (var foo = new ServiceReference1.SWIPrologServiceClient())
            using (var foo2 = new ServiceReference2.SWIPrologServiceClient())
            {
                var ret1 = foo.Assert("cell(1, 1, b)");
                var ret2 = foo.Assert("cell(2, 3, x)");
                var solutions = foo.GetSolutionVariables("cell(X, Y, Z)");

                var ret12 = foo2.Assert("cell(2, 2, b)");
                var ret22 = foo2.Assert("cell(3, 3, x)");
                var solutions2 = foo2.GetSolutionVariables("cell(X, Y, Z)");
            }
        }

        public Form1()
        {
            InitializeComponent();
        }
    }
}
