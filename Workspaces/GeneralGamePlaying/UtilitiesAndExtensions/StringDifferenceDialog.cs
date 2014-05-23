using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

// ToDo:  Use code at this website for diff http://www.mathertel.de/Diff/
// check out http://www.pindari.com/rtf1.html for rtf formatting information
namespace StringDifferenceDialog
{
    public partial class StringDifferenceDialog : Form
    {
        private bool _breakIntoLines;

        private string _string1, _string2;
        private string[] _stringArray1, _stringArray2;

        public StringDifferenceDialog(string string1, string string2, bool breakIntoLines = true)
        {
            _breakIntoLines = breakIntoLines;
            _string1 = string1;
            _string2 = string2;

            if (_breakIntoLines)
            {
                _stringArray1 = _string1.Split(Environment.NewLine.ToCharArray());
                _stringArray2 = _string2.Split(Environment.NewLine.ToCharArray());
            }
            else
            {
                _stringArray1 = new string[] { _string1 };
                _stringArray2 = new string[] { _string2 };
            }

            InitializeComponent();
        }

        public StringDifferenceDialog(string[] stringArray1, string[] stringArray2, bool breakIntoLines = true)
        {
            _breakIntoLines = breakIntoLines;
            _stringArray1 = stringArray1;
            _stringArray2 = stringArray2;

            if (_breakIntoLines)
            {
                _stringArray1 = _string1.Split(Environment.NewLine.ToCharArray());
                _stringArray2 = _string2.Split(Environment.NewLine.ToCharArray());
            }
            else
            {
                _stringArray1 = new string[] { _string1 };
                _stringArray2 = new string[] { _string2 };
            }

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            StringBuilder rtfsb = new StringBuilder();

            bool differenceDetected, prevDifferenceDetected = false;
            for (int line = 0; line < Math.Max(_stringArray1.Length, _stringArray2.Length); line++)
            {
                var sb1 = new StringBuilder();
                var sb2 = new StringBuilder();

                //sb1.Append(@"\b0 ");
                //sb2.Append(@"\b0 ");
                sb1.Append(@"\cf1");
                sb2.Append(@"\cf1");

                for (int charIndex = 0; charIndex < Math.Max(_stringArray1[line].Length, _stringArray2[line].Length); charIndex++ )
                {
                    differenceDetected = true;
                    if (charIndex < _stringArray1[line].Length && charIndex < _stringArray2[line].Length)
                    {
                        differenceDetected = (_stringArray1[line][charIndex] != _stringArray2[line][charIndex]);
                    }

                    if (charIndex < _stringArray1[line].Length)
                    {
                        if (differenceDetected && !prevDifferenceDetected)
                        {
                            //sb1.Append(@"\b ");
                            sb1.Append(@"\cf2");
                        }
                        else if (!differenceDetected && prevDifferenceDetected)
                        {
                            //sb1.Append(@"\b0");
                            sb1.Append(@"\cf1");
                        }
                        sb1.Append(_stringArray1[line][charIndex]);
                    }

                    if (charIndex < _stringArray2[line].Length)
                    {
                        sb2.Append(_stringArray2[line][charIndex]);
                    }

                    prevDifferenceDetected = differenceDetected;                    
                }

                rtfsb.Append(sb1.ToString() + @"\par ");
                rtfsb.Append(sb2.ToString() + @"\par ");

                // add a line in between pairs of lines
                rtfsb.Append(@"\par ");
            }

            richTextBox1.Rtf = @"{\rtf1\ansi\kerning0 {\fonttbl {\f0 Courier New;}}{\colortbl;\red0\green0\blue0;\red255\green0\blue0;}\fs20" + rtfsb.ToString() + "}";
        }

        static public DialogResult ShowDialog(string string1, string string2, bool breakIntoLines = true)
        {
            // Call the private constructor so the users only need to call this
            // function, which is similar to MessageBox.Show.
            // Returns a standard DialogResult.
            using (var dialog = new StringDifferenceDialog(string1, string2, breakIntoLines))
            {
                DialogResult result = dialog.ShowDialog();
                return result;
            }
        }

    }
}
