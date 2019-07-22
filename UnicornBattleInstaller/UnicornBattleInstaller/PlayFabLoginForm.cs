using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PlayFab;
using UnicornBattleInstaller.DeveloperToolsUtility;

namespace UnicornBattleInstaller
{
    public partial class PlayFabLoginForm : Form
    {
        private DeveloperTools _devtools;
        
        public PlayFabLoginForm()
        {
            InitializeComponent();
            ErrorMessage.SelectionAlignment = HorizontalAlignment.Center;
            
            //TODO: Remove for testing only.
            //EmailBox.Text = "marco+ubtest@hashbanggames.com";
            //PasswordBox.Text = "0143Skittles";

            _devtools = new DeveloperTools();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.LoginButton.MouseClick += LoginButton_MouseClick;

            base.OnLoad(e);
        }

        private void LoginButton_MouseClick(object sender, MouseEventArgs e)
        {
            _devtools.Login(this.EmailBox.Text, this.PasswordBox.Text, OnLoginSuccess, OnError);
        }

        private void OnError(PlayFabError error)
        {
            ErrorMessage.Text = error.ErrorMessage;
            ErrorMessage.Visible = true;
        }

        private void OnLoginSuccess()
        {
            ErrorMessage.Visible = false;
            _devtools.GetStudios(OnGetStudios, OnError);
        }

        private void OnGetStudios(List<Studio> studios)
        {
            var installForm = FormManager.Get<InstallForm>("InstallForm");
            installForm.Titles.Items.Insert(0, "Select One");
            installForm.Titles.SelectedIndex = 0;

            studios.ForEach(s=>{
                var titles = s.Titles.ToList();
                titles.ForEach(t =>
                {
                    installForm.Titles.Items.Add(t.Name);
                    installForm.AvailableTitles.Add(t);
                });
            });
            FormManager.Show("InstallForm");
        }
    }

    

}
