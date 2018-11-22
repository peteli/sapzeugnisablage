using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sapzeugnisablage
{
    /// <summary>
    /// helper class to redirect console writeline to form textbox
    /// </summary>
    // redirect console writeline to form textbox
    public class ConsoleWriter : TextWriter
    {
        delegate void StringArgReturningVoidDelegate(string text);
        TextBox _output = null;

        public ConsoleWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            //_output.AppendText(value.ToString());
            Write2TextBox(value.ToString());
        }

        private void Write2TextBox(string text)
        {

            if (_output.InvokeRequired)
            {
                /*
                 * Führt mit der angegebenen Argumentliste den angegebenen Delegaten für den Thread aus, 
                 * der das dem Steuerelement zugrunde liegende Fensterhandle besitzt.
                 */

                StringArgReturningVoidDelegate d = new StringArgReturningVoidDelegate(Write2TextBox); //invoke same function recursevely
                _output.Invoke(d, new object[] { text});
            }
            else
            {
                _output.AppendText(text);
            }
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }
    }
}
