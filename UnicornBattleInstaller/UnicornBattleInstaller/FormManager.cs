using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UnicornBattleInstaller
{
    public class FormManager
    {
        private static Dictionary<string, Form> _forms = new Dictionary<string, Form>();

        public static void Initialize()
        {
            _forms.Add("PlayFabLoginForm", new PlayFabLoginForm());
            _forms.Add("InstallForm", new InstallForm());
        }

        public static T Get<T>(string FormKey) where T : Form  {
            return (T)_forms[FormKey];
        }

        public static void Show(string FormKey)
        {
            foreach(var form in _forms)
            {
                Hide(form.Key);
            }
            _forms[FormKey].Show();
        }

        public static void Hide(string FormKey)
        {
            _forms[FormKey].Hide();
        }


    }
}
