using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnicornBattleInstaller.DeveloperToolsUtility;

namespace UnicornBattleInstaller
{
    public partial class InstallForm : Form
    {
        public List<Title> AvailableTitles = new List<Title>();
        private int MAXSteps = 9;

        private delegate void InstallProgressEvent(int progress);
        private event InstallProgressEvent OnInstallProgress;

        public InstallForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            this.InstallButton.Enabled = false;
            Titles.SelectedIndexChanged += Titles_SelectedIndexChanged;
            InstallButton.MouseClick += InstallButton_MouseClick;

            OnInstallProgress += InstallForm_OnInstallProgress;

            TitleMigrater.OnLogTofile += TitleMigrater_OnLogTofile;

            base.OnLoad(e);
        }

        private void TitleMigrater_OnLogTofile(string info)
        {
            Console.WriteLine(info);
            InstallProgressText.Text = info;
        }

        protected override void OnClosed(EventArgs e)
        {
            Titles.SelectedIndexChanged -= Titles_SelectedIndexChanged;
            InstallButton.MouseClick -= InstallButton_MouseClick;

            OnInstallProgress -= InstallForm_OnInstallProgress;

            base.OnClosed(e);
        }

        private void InstallForm_OnInstallProgress(int progress)
        {
            var calcValue = (float)(progress / (float)MAXSteps);
            var progressValue = Convert.ToInt32( calcValue * 100);
            Console.WriteLine($"Current ProgressBar: {progressValue.ToString()}");
            InstallProgress.Value = progressValue;
            InstallProgress.Update();
            Application.DoEvents();
        }

        private async void InstallButton_MouseClick(object sender, MouseEventArgs e)
        {
            var tm = new TitleMigrater();
            tm.GetTitleSettings(Titles.SelectedIndex);
            InstallProgress.Visible = true;
            InstallProgressText.Visible = true;
            InstallButton.Visible = false;
            InstallProgress.Minimum = 0;
            InstallProgress.Maximum = MAXSteps / MAXSteps * 100;
            OnInstallProgress?.Invoke(1);


            //Step 1
            InstallProgressText.Text = "Validating Authentication";
            if (! await tm.GetAuthToken())
            {
                InstallProgressText.Text = "Failed to retrieve Auth Token";
                return;
            }

            //Step 2
            OnInstallProgress?.Invoke(2);
            InstallProgressText.Text = "Uploading Title Data...";
            if (! await tm.UploadTitleData())
            {
                InstallProgressText.Text = "Failed to upload TitleData.";
                InstallButton.Visible = true;
                return;
            }

            //Step 3
            OnInstallProgress?.Invoke(3);
            InstallProgressText.Text = "Uploading Economy Data...";
            if (! await tm.UploadEconomyData())
            {
                InstallProgressText.Text = "Failed to upload Economy Data.";
                InstallButton.Visible = true;
                return;
            }


            //Step 4
            OnInstallProgress?.Invoke(4);
            InstallProgressText.Text = "Uploading Event Data...";
            if (! await tm.UploadEventData())
            {
                InstallProgressText.Text = "Failed to upload Event Data.";
                InstallButton.Visible = true;
                return;
            }

            //Step 5
            OnInstallProgress?.Invoke(5);
            InstallProgressText.Text = "Uploading CloudScript ...";
            if (! await tm.UploadCloudScript())
            {
                InstallProgressText.Text = "Failed to upload CloudScript.";
                InstallButton.Visible = true;
                return;
            }

            //Step 6
            OnInstallProgress?.Invoke(6);
            InstallProgressText.Text = "Uploading Title News Data...";
            if (! await tm.UploadTitleNews())
            {
                InstallProgressText.Text = "Failed to upload TitleNews.";
                InstallButton.Visible = true;
                return;
            }

            //Step 7
            OnInstallProgress?.Invoke(7);
            InstallProgressText.Text = "Uploading Statistic Def Data...";
            if (! await tm.UploadStatisticDefinitions())
            {
                InstallProgressText.Text = "Failed to upload Statistics Definitions.";
                InstallButton.Visible = true;
                return;
            }

            if (!SkipCDN.Checked) {
                //Step 8
                OnInstallProgress?.Invoke(8);
                InstallProgressText.Text = "Uploading CDN Assets...";
                if (! await tm.UploadCdnAssets())
                {
                    InstallProgressText.Text = "Failed to upload CDN Assets.";
                    InstallButton.Visible = true;
                    return;
                }
            }

            //Step 9
            OnInstallProgress?.Invoke(9);
            InstallProgressText.Text = "Uploading Permission Policies...";
            if (! await tm.UploadPolicy(TitleMigrater.permissionPath))
            {
                InstallProgressText.Text = "Failed to upload permissions policy.";
                InstallButton.Visible = true;
                return;
            }


            InstallProgressText.Text = "Installation Complete!";
        }

        private void Titles_SelectedIndexChanged(object sender, EventArgs e)
        {
            InstallButton.Enabled = true;
        }
    }
}
