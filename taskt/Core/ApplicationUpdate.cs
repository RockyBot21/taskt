﻿using System;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace taskt.Core
{
    public class ApplicationUpdate
    {
        public UpdateManifest GetManifest()
        {
            //create web client
            WebClient webClient = new WebClient();
            string manifestData = "";

            //get manifest
            try
            {
                manifestData = webClient.DownloadString(MyURLs.LatestJSONURL);           
            }
            catch (Exception)
            {
                //unable to get the manifest
                throw;
            }

            //initialize config
            UpdateManifest manifestConfig = new UpdateManifest();

            try
            {
                manifestConfig = JsonConvert.DeserializeObject<UpdateManifest>(manifestData);
            }
            catch (Exception)
            {
                //bad json received
                throw;
            }

            //create versions
            manifestConfig.RemoteVersionProper = new Version(manifestConfig.RemoteVersion);
            manifestConfig.LocalVersionProper = new Version(System.Windows.Forms.Application.ProductVersion);

            //determine comparison
            int versionCompare = manifestConfig.LocalVersionProper.CompareTo(manifestConfig.RemoteVersionProper);

            if (versionCompare < 0)
            {
                manifestConfig.RemoteVersionNewer = true;
            }
            else
            {
                manifestConfig.RemoteVersionNewer = false;
            }

            return manifestConfig;
        }

        public static void ShowUpdateResult(bool skipBeta, bool silent = true)
        {
            taskt.Core.ApplicationUpdate updater = new Core.ApplicationUpdate();
            Core.UpdateManifest manifest = new Core.UpdateManifest();
            try
            {
                manifest = updater.GetManifest();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error getting manifest: " + ex.ToString());
                if (!silent)
                {
                    using (var fm = new taskt.UI.Forms.Supplemental.frmDialog("Error getting manifest: " + ex.ToString(), "Error", taskt.UI.Forms.Supplemental.frmDialog.DialogType.OkOnly, 0))
                    {
                        fm.ShowDialog();
                    }
                }
                return;
            }

            bool startUpdate = false;
            if (manifest.RemoteVersionNewer)
            {
                if (!manifest.Beta)
                {
                    startUpdate = true;
                }
                else if (manifest.Beta && !skipBeta)
                {
                    startUpdate = true;
                }
            }

            if (startUpdate)
            {
                //using (UI.Forms.Supplement_Forms.frmUpdate frmUpdate = new UI.Forms.Supplement_Forms.frmUpdate(manifest))
                //{
                //    if (frmUpdate.ShowDialog() == DialogResult.OK)
                //    {

                //        //move update exe to root folder for execution
                //        var updaterExecutionResources = System.IO.Path.Combine(Application.StartupPath, "Resources", "taskt-updater.exe");
                //        //var updaterExecutableDestination = Application.StartupPath + "\\taskt-updater.exe";

                //        if (!System.IO.File.Exists(updaterExecutionResources))
                //        {
                //            //MessageBox.Show("taskt-updater.exe not found in Resources folder!");
                //            using (var fm = new taskt.UI.Forms.Supplemental.frmDialog("taskt-updater.exe not found in Resources folder!", "Error", taskt.UI.Forms.Supplemental.frmDialog.DialogType.OkOnly, 0))
                //            {
                //                fm.ShowDialog();
                //            }
                //            return;
                //        }

                //        var updateProcess = new System.Diagnostics.Process();
                //        //updateProcess.StartInfo.FileName = updaterExecutableDestination;
                //        updateProcess.StartInfo.FileName = updaterExecutionResources;
                //        updateProcess.StartInfo.Arguments = "/d " + manifest.PackageURL;

                //        updateProcess.Start();
                //        Application.Exit();
                //    }
                //}
                ShowUpdateForm(manifest);
            }
            else
            {
                if (!silent)
                {
                    using (var fm = new taskt.UI.Forms.Supplemental.frmDialog("taskt is currently up-to-date!", "No Updates Available", taskt.UI.Forms.Supplemental.frmDialog.DialogType.OkOnly, 0))
                    {
                        fm.ShowDialog();
                    }
                }
            }
        }

        public static void ShowUpdateResultAsync()
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;

            //get manifest
            try
            {
                wc.DownloadStringCompleted += CompleteDownloadUpdateInformation;
                wc.DownloadStringAsync(new Uri(MyURLs.LatestJSONURL));
            }
            catch (Exception ex)
            {
                // silent! ;-)
                //throw new Exception("Fail check update : " + ex.Message);
            }
        }

        private static void CompleteDownloadUpdateInformation(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string manifestString = e.Result;

                //initialize config
                UpdateManifest manifestConfig = new UpdateManifest();

                manifestConfig = JsonConvert.DeserializeObject<UpdateManifest>(manifestString);
                //create versions
                manifestConfig.RemoteVersionProper = new Version(manifestConfig.RemoteVersion);
                manifestConfig.LocalVersionProper = new Version(System.Windows.Forms.Application.ProductVersion);

                //determine comparison
                int versionCompare = manifestConfig.LocalVersionProper.CompareTo(manifestConfig.RemoteVersionProper);

                if (versionCompare < 0)
                {
                    manifestConfig.RemoteVersionNewer = true;
                }
                else
                {
                    manifestConfig.RemoteVersionNewer = false;
                }

                if (manifestConfig.RemoteVersionNewer && !manifestConfig.Beta)
                {
                    // show new version message
                    ShowUpdateForm(manifestConfig);
                }
            }
            catch (Exception ex)
            {
                // silent! ;-)
                //bad json received
                //throw new Exception("Fail parse update info : " + ex.Message);
            }
        }

        private static void ShowUpdateForm(UpdateManifest manifestConfig)
        {
            using (var frmUpdate = new taskt.UI.Forms.Supplement_Forms.frmUpdate(manifestConfig))
            {
                if (frmUpdate.ShowDialog() == DialogResult.OK)
                {

                    //move update exe to root folder for execution
                    var updaterExecutionResources = System.IO.Path.Combine(Application.StartupPath, "Resources", "taskt-updater.exe");
                    //var updaterExecutableDestination = Application.StartupPath + "\\taskt-updater.exe";

                    if (!System.IO.File.Exists(updaterExecutionResources))
                    {
                        //MessageBox.Show("taskt-updater.exe not found in Resources folder!");
                        using (var fm = new taskt.UI.Forms.Supplemental.frmDialog("taskt-updater.exe not found in Resources folder!", "Error", taskt.UI.Forms.Supplemental.frmDialog.DialogType.OkOnly, 0))
                        {
                            fm.ShowDialog();
                        }
                        return;
                    }

                    var updateProcess = new System.Diagnostics.Process();
                    updateProcess.StartInfo.FileName = updaterExecutionResources;

                    var zipURL = manifestConfig.PackageURL.Replace("%release_url%", MyURLs.GitReleaseURL).Replace("%version%", manifestConfig.RemoteVersion);
                    updateProcess.StartInfo.Arguments = "/d " + zipURL;

                    updateProcess.Start();
                    Application.Exit();
                }
            }
        }
    }

    public class UpdateManifest
    {
        //from manifest
        public string RemoteVersion { get; set; }
        public bool Beta { get; set; }
        public string PackageURL { get; set; }

        //helpers
        public bool RemoteVersionNewer { get; set; }
        public Version RemoteVersionProper { get; set; }
        public Version LocalVersionProper { get; set; }
    }
}
