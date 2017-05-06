using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Symbol.XamarinEMDK;
using System.Xml;
using System.IO;
using Xamarin.Forms;
using System.Collections.Generic;

namespace AppForum2017_EmdkXamarinForms.Droid
{
    [Activity(Label = "AppForum2017_EmdkXamarinForms", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, EMDKManager.IEMDKListener
        
    {
        private EMDKManager emdkManager = null;
        private ProfileManager profileManager = null;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            EMDKResults result = EMDKManager.GetEMDKManager( ApplicationContext, this);
            if (result.StatusCode != EMDKResults.STATUS_CODE.Success)
            {
                Toast.MakeText(this, "Error opening the EMDK Manager", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "EMDK Manager is available", ToastLength.Long).Show();
            }

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());

            var emdk = DependencyService.Get<IDeviceConfig>();
            emdk.setClockRequest += (cs, profileData) =>
            {
                string time = "";
                string date = "";
                string time_zone = "";

                profileData.TryGetValue("time", out time);
                profileData.TryGetValue("date", out date);
                profileData.TryGetValue("time_zone", out time_zone);

                String[] modifyData = new String[profileData.Count];
                String profileName = "Profile1/Clock/Clock-1";

                modifyData[0] = "Clock-1.TimeZone=" + time_zone;
                modifyData[1] = "Clock-1.Date=" + date;
                modifyData[2] = "Clock-1.Time=" + time;

                if (profileManager != null)
                {
                    EMDKResults results = profileManager.ProcessProfileAsync(profileName, ProfileManager.PROFILE_FLAG.Set, modifyData);
                }
            };
        }

        
        protected override void OnDestroy()
        {
            if (profileManager != null)
            {
                profileManager = null;
            }

            if (emdkManager != null)
            {
                emdkManager.Release();
                emdkManager = null;
            }

            base.OnDestroy();
        }

        public void OnClosed()
        {
            if (emdkManager != null)
            {
                emdkManager.Release();
                emdkManager = null;
            }
        }

        public void OnOpened(EMDKManager emdkManager)
        {
            this.emdkManager = emdkManager;
            profileManager = (ProfileManager)emdkManager.GetInstance(EMDKManager.FEATURE_TYPE.Profile);
            profileManager.Data += profileManager_Data;

        }

        void profileManager_Data(object sender, ProfileManager.DataEventArgs e)
        {
            String strStatus = "";
            // Call back with the result of the processProfileAsync
            EMDKResults results = e.P0.Result;

            if (results.StatusCode == EMDKResults.STATUS_CODE.Success)
            {
                strStatus = "Profile processed succesfully";
            }
            else if (results.StatusCode == EMDKResults.STATUS_CODE.CheckXml)
            {
                //Inspect the XML response to see if there are any errors, if not report success
                using (XmlReader reader = XmlReader.Create(new StringReader(results.StatusString)))
                {
                    String checkXmlStatus = "\n\n";
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                switch (reader.Name)
                                {
                                    case "parm-error":
                                        checkXmlStatus += "Parm Error:\n";
                                        checkXmlStatus += reader.GetAttribute("name") + " - ";
                                        checkXmlStatus += reader.GetAttribute("desc") + "\n\n";
                                        break;
                                    case "characteristic-error":
                                        checkXmlStatus += "characteristic Error:\n";
                                        checkXmlStatus += reader.GetAttribute("type") + " - ";
                                        checkXmlStatus += reader.GetAttribute("desc") + "\n\n";
                                        break;
                                }
                                break;
                        }
                    }

                    if (checkXmlStatus == "\n\n") { strStatus = "Profile applied successfully ..."; }
                    else
                    { strStatus = checkXmlStatus; }
                }
            }
            else { strStatus = "Something wrong on processing the profile"; }

            RunOnUiThread(() => MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "Status", strStatus));
        }




    }
}

