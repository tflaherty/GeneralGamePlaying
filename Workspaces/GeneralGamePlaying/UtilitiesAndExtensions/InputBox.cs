using System;
using System.Collections.Generic;

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace API.UtilitiesAndExtensions
{
	public enum InputBoxResultType
	{
		Any,
		Boolean,
		Byte,
		Char,
		Date,
		Decimal,
		Double,
		Float,
		Int16,
		Int32,
		Int64,
		UInt16,
		UInt32,
		UInt64,
        Enum
	}

	/// <summary>
	/// Summary description for InputBox.
	/// </summary>
	public partial class InputBox : System.Windows.Forms.Form
	{
		private string defaultValue = string.Empty;

		private InputBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.Text = Application.ProductName;
		}

		public static DialogResult ShowDialog(out string input, Control parent)
		{
			return InputBox.ShowDialog(null, null, null, out input, InputBoxResultType.Any, false, parent);
		}

		public static DialogResult ShowDialog(out string input, InputBoxResultType validationType, Control parent)
		{
			return InputBox.ShowDialog(null, null, null, out input, validationType, false, parent);
		}

		public static DialogResult ShowDialog(string caption, out string input, Control parent)
		{
			return InputBox.ShowDialog(caption, null, null, out input, InputBoxResultType.Any, false, parent);
		}

		public static DialogResult ShowDialog(string caption, out string input, InputBoxResultType validationType, bool isNullable, Control parent)
		{
			return InputBox.ShowDialog(caption, null, null, out input, validationType, isNullable, parent);
		}

		public static DialogResult ShowDialog(string caption, string prompt, out string input, Control parent)
		{
			return InputBox.ShowDialog(caption, prompt, null, out input, InputBoxResultType.Any, false, parent);
		}

		public static DialogResult ShowDialog(string caption, string prompt, out string input, InputBoxResultType validationType, Control parent)
		{
			return InputBox.ShowDialog(caption, prompt, null, out input, validationType, false, parent);
		}

        public static DialogResult ShowDialog(IEnumerable<string> enumStrings, string caption, string prompt, string defaultValue, out string input, InputBoxResultType validationType, bool isNullable, Control parent)
        {
            DialogResult result = DialogResult.None;

            InputBox inputBox = new InputBox();
            inputBox.TopMost = true;

            inputBox.comboBox1.Visible = true;
            inputBox.comboBox1.Enabled = true;
            inputBox.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;

            // Set the members of the new instance
            // according to the value of the parameters
            if (string.IsNullOrEmpty(caption))
            {
                inputBox.Text = Application.ProductName;
            }
            else
            {
                inputBox.Text = caption;
                if (isNullable)
                {
                    inputBox.Text += " (Optional)";
                }
                else
                {
                    inputBox.Text += " (Required)";
                }
            }

            if (!string.IsNullOrEmpty(prompt))
            {
                inputBox.lblPrompt.Text = prompt;
            }

            inputBox.defaultValue = defaultValue;

            foreach (string @string in enumStrings)
            {
                inputBox.comboBox1.Items.Add(@string);
            }

            if (!String.IsNullOrEmpty(defaultValue))
            {
                foreach (var item in inputBox.comboBox1.Items)
                {
                    if (item.ToString() == defaultValue)
                    {
                        inputBox.comboBox1.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                inputBox.comboBox1.SelectedIndex = 0;
            }

            result = inputBox.ShowDialog(parent);
            input = inputBox.comboBox1.SelectedItem.ToString();

            return result;

        }

		public static DialogResult ShowDialog(string caption, string prompt, string defaultValue, out string input, InputBoxResultType validationType, bool isNullable, Control parent)
		{
			// Create an instance of the InputBox class.
			InputBox inputBox = new InputBox();
			inputBox.TopMost = true;

            if (validationType == InputBoxResultType.Date)
            {
                inputBox.dateTimePicker1.Visible = true;
                inputBox.dateTimePicker1.Enabled = true;
            }
            else
            {
                inputBox.txtInput.Visible = true;
                inputBox.txtInput.Enabled = true;
            }

			// Set the members of the new instance
			// according to the value of the parameters
			if (string.IsNullOrEmpty(caption))
			{
				inputBox.Text = Application.ProductName;
			}
			else
			{
				inputBox.Text = caption;
				if (isNullable)
				{
					inputBox.Text += " (Optional)";
				}
				else
				{
					inputBox.Text += " (Required)";
				}
			}

			if (!string.IsNullOrEmpty(prompt))
			{
				inputBox.lblPrompt.Text = prompt;
			}

			if (!string.IsNullOrEmpty(defaultValue))
			{
				inputBox.defaultValue = inputBox.txtInput.Text = defaultValue;
			}

			// Calculate size required for prompt message and adjust
			// Label and dialog size to fit.
			Size promptSize = inputBox.lblPrompt.CreateGraphics().MeasureString(prompt, inputBox.lblPrompt.Font,
			   inputBox.ClientRectangle.Width - 20).ToSize();
			// a little wriggle room
			if (promptSize.Height > inputBox.lblPrompt.Height)
			{
				promptSize.Width += 4;
				promptSize.Height += 4;
			}
			inputBox.lblPrompt.Width = inputBox.ClientRectangle.Width - 20;
			inputBox.lblPrompt.Height = Math.Max(inputBox.lblPrompt.Height, promptSize.Height);

			int postLabelMargin = 2;
			if ((inputBox.lblPrompt.Top + inputBox.lblPrompt.Height + postLabelMargin) > inputBox.txtInput.Top)
			{
				inputBox.ClientSize = new Size(inputBox.ClientSize.Width, inputBox.ClientSize.Height +
				   (inputBox.lblPrompt.Top + inputBox.lblPrompt.Height + postLabelMargin - inputBox.txtInput.Top));
			}
			else if ((inputBox.lblPrompt.Top + inputBox.lblPrompt.Height + postLabelMargin) < inputBox.txtInput.Top)
			{
				inputBox.ClientSize = new Size(inputBox.ClientSize.Width, inputBox.ClientSize.Height -
				   (inputBox.lblPrompt.Top + inputBox.lblPrompt.Height + postLabelMargin - inputBox.txtInput.Top));
			}

			// Ensure that the value of input is set
			// There will be a compile error later if not
			input = string.Empty;

			// Declare a variable to hold the result to be
			// returned on exitting the method
			DialogResult result = DialogResult.None;

			// Loop round until the user enters
			// some valid data, or cancels.
			while (result == DialogResult.None)
			{
				result = inputBox.ShowDialog(parent);

				if (result == DialogResult.OK)
				{
					// Only test if specific type is required
					if (validationType != InputBoxResultType.Any)
					{
						// If the test fails - Invalid input.
						if (!inputBox.Validate(validationType))
						{
							// Set variables ready for another loop
							input = string.Empty;

							if (!isNullable)
							{
								// result to 'None' to ensure while loop
								// repeats
								result = DialogResult.None;
								// Let user know there is a problem
								MessageBox.Show(inputBox, "The data entered is not a valid " + validationType.ToString() + ".");
								// Set the focus back to the TextBox
								inputBox.txtInput.Select();								
							}
						}
                        else
						{
                            // if user clicked OK, validate the entry
                            if(validationType == InputBoxResultType.Date)
                            {
                                input = inputBox.dateTimePicker1.Text;
                            }
                            else
                            {
                                input = inputBox.txtInput.Text;						                                    
                            }
						}
					}
					else
					{
						input = inputBox.txtInput.Text;						                                    						
					}
				}
				else
				{
					// User has cancelled.
					// Use the defaultValue if there is one, or else
					// an empty string.
					if (string.IsNullOrEmpty(inputBox.defaultValue))
					{
						input = string.Empty;
					}
					else
					{
						input = inputBox.defaultValue;
					}
				}
			}

			// Trash the dialog if it is hanging around.
			if (inputBox != null)
			{
				inputBox.Dispose();
			}

			// Send back the result.
			return result;
		}

		private bool Validate(InputBoxResultType validationType)
		{
			bool result = false;
			switch (validationType)
			{
				case InputBoxResultType.Boolean:
					try
					{
						Boolean.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.Byte:
					try
					{
						Byte.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.Char:
					try
					{
						Char.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.Date:
					try
					{
						DateTime.Parse(this.dateTimePicker1.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.Decimal:
					try
					{
						Decimal.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.Double:
					try
					{
						Double.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.Float:
					try
					{
						Single.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.Int16:
					try
					{
						Int16.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.Int32:
					try
					{
						Int32.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.Int64:
					try
					{
						Int64.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.UInt16:
					try
					{
						UInt16.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.UInt32:
					try
					{
						UInt32.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
				case InputBoxResultType.UInt64:
					try
					{
						UInt64.Parse(this.txtInput.Text);
						result = true;
					}
					catch
					{
						// just eat it
					}
					break;
			}

			return result;
		}

		private void InputBox_Load(object sender, System.EventArgs e)
		{
			this.txtInput.Focus();
		}
	}
}