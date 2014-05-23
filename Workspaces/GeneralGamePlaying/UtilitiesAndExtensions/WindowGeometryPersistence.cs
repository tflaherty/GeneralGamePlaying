using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace API.UtilitiesAndExtensions
{
    /*
        // how to use this class:
        //

            public TextWindow()
            {
                // TextWindow is this form's name.
                // Your form will have a different name most likely.
                InitializeComponent();
                GeometryFromString(Properties.Settings.Default.WindowGeometry, this);
            }

            void TextWindow_FormClosing(object sender, FormClosingEventArgs e)
            {
                // persist our geometry string.
                Properties.Settings.Default.WindowGeometry = GeometryToString(this);
                Properties.Settings.Default.Save();
            }
    */

    public class WindowGeometryPersistence
    {
        // hide public constructor
        private WindowGeometryPersistence()
        {
        }

        public static void SetFormsGeometry(System.Collections.Specialized.StringCollection stringCollection, Form form)
        {
            try
            {
                if (stringCollection == null || stringCollection.Count == 0)
                {
                    return;
                }

                // The form's tag holds a string that identifies this window.  I did it this way to make it independent of
                // the form's title.
                var tagName = form.Tag as string;
                if (tagName == null)
                {
                    return;
                }
                var tagNameWithSep = tagName + "|";

                // Find the first string in the collection that starts with this string immediately followed by '|'
                int index = -1;
                for (int x = 0; x < stringCollection.Count; x++)
                {
                    if (stringCollection[x].StartsWith(tagNameWithSep))
                    {
                        index = x;
                        break;
                    }
                }

                if (index == -1)
                {
                    return;
                }

                GeometryFromString(stringCollection[index].Substring(tagNameWithSep.Length), form);
            }
            // this can throw an exception upon startup which is cause by an MS bug, so just ignore it
            catch (Exception)
            {
                
                //throw;
            }
        }

        public static void SaveFormsGeometry(System.Collections.Specialized.StringCollection stringCollection, Form form)
        {
            if (stringCollection == null)
            {
                return;
            }

            // The form's tag holds a string that identifies this window.  I did it this way to make it independent of
            // the form's title.
            var tagName = form.Tag as string;
            if (tagName == null)
            {
                return;
            }
            var tagNameWithSep = tagName + "|";

            // Find the first string in the collection that starts with this string immediately followed by '|'
            int index = -1;
            for (int x = 0; x < stringCollection.Count; x++)
            {
                if (stringCollection[x].StartsWith(tagNameWithSep))
                {
                    index = x;
                    break;
                }
            }

            var newString = tagNameWithSep + GeometryToString(form);
            if (index == -1)
            {
                stringCollection.Add(newString);
            }
            else
            {
                stringCollection[index] = newString;
            }
        }


        public static void GeometryFromString(string thisWindowGeometry, Form form)
        {
            if (string.IsNullOrEmpty(thisWindowGeometry))
            {
                return;
            }

            string[] numbers = thisWindowGeometry.Split('|');
            string windowString = numbers[4];
            if (windowString == "Normal")
            {
                Point windowPoint = new Point(int.Parse(numbers[0]), int.Parse(numbers[1]));
                Size windowSize = new Size(int.Parse(numbers[2]), int.Parse(numbers[3]));

                bool locOkay = GeometryIsBizarreLocation(windowPoint, windowSize);
                bool sizeOkay = GeometryIsBizarreSize(windowSize);

                if (locOkay && sizeOkay)
                {
                    form.Location = windowPoint;
                    form.Size = windowSize;
                    form.StartPosition = FormStartPosition.Manual;
                    form.WindowState = FormWindowState.Normal;
                }
                else if (sizeOkay)
                {
                    form.Size = windowSize;
                }
            }
            else if (windowString == "Maximized")
            {
                form.Location = new Point(100, 100);
                form.StartPosition = FormStartPosition.Manual;
                form.WindowState = FormWindowState.Maximized;
            }
        }

        private static bool GeometryIsBizarreLocation(Point loc, Size size)
        {
            bool locOkay;
            if (loc.X < 0 || loc.Y < 0)
            {
                locOkay = false;
            }
            else if (loc.X + size.Width > Screen.PrimaryScreen.WorkingArea.Width)
            {
                locOkay = false;
            }
            else if (loc.Y + size.Height > Screen.PrimaryScreen.WorkingArea.Height)
            {
                locOkay = false;
            }
            else
            {
                locOkay = true;
            }
            return locOkay;
        }

        private static bool GeometryIsBizarreSize(Size size)
        {
            return (size.Height <= Screen.PrimaryScreen.WorkingArea.Height &&
                size.Width <= Screen.PrimaryScreen.WorkingArea.Width);
        }

        public static string GeometryToString(Form form)
        {
            return form.Location.X + "|" +
                   form.Location.Y + "|" +
                   form.Size.Width + "|" +
                   form.Size.Height + "|" +
                   form.WindowState;
        }
    }
}
