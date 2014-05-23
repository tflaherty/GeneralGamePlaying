using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using API.SWIProlog.Engine;
using API.SWIProlog.SWIPrologServiceLibrary;

namespace API.GGP.GeneralGamePlayingNS
{
    public partial class PrologQuery : Form
    {
        private PrologEngine Engine;

        public PrologQuery(PrologEngine engine)
        {
            InitializeComponent();
            Engine = engine;
        }

        private void PrologQuery_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F12)
            {
                e.Handled = true;
                btn_Submit_Click(null, null);
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                this.Close();
            }
            else if (e.KeyCode == Keys.Home)
            {
                e.Handled = true;
                txb_Query.Text = string.Empty;
                txb_Query.Focus();
            }
        }

        private void btn_Submit_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();

            txb_QueryResults.Clear();

            foreach (List<SolutionVariable> solutionVariables in Engine.GetSolutionVariables(txb_Query.Text))
            {
                foreach (SolutionVariable solutionVariable in solutionVariables)
                {
                    sb.Append(solutionVariable.Variable + " = " + solutionVariable.Value + ", ");
                }

                if (sb.Length >= ", ".Length)
                {
                    sb.Remove(sb.Length - ", ".Length, ", ".Length);
                }
                sb.AppendLine();
            }

            txb_QueryResults.Text = sb.ToString();
        }

        private void btn_ListAll_Click(object sender, EventArgs e)
        {
            txb_QueryResults.Text = Engine.ListAll();
        }

        private void btn_SaveResults_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btn_ListAllFacts_Click(object sender, EventArgs e)
        {
            txb_QueryResults.Text = Engine.ListAllFacts(sortAlphabetically:true);
        }

        private void btn_ListAllNonAlphabetically_Click(object sender, EventArgs e)
        {
            txb_QueryResults.Text = Engine.ListAll(sortAlphabetically: false);
        }

        private void btn_Copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txb_QueryResults.Text);
        }

    }
}
