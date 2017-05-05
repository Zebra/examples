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
        private const String profileName = "Profile1";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            EMDKResults result = EMDKManager.GetEMDKManager(/*Application.Context*/ ApplicationContext, this);
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

            MessagingCenter.Subscribe<App, List<KeyValuePair<string, string>>>((App)Xamarin.Forms.Application.Current, "ProcessProfile", (sender, arg) => {
                string time = "";
                string date = "";
                string timeZone = "";

                foreach (KeyValuePair<string,string> parm in arg)
                {
                    if (parm.Key.Equals("time"))
                    {
                        time = parm.Value;
                    }

                    if (parm.Key.Equals("date"))
                    {
                        date = parm.Value;
                    }

                    if (parm.Key.Equals("timeZone"))
                    {
                        timeZone = parm.Value;
                    }

                }

                ProcessEMDKProfile(time, date, timeZone);
            });
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
            
        }


        public void ProcessEMDKProfile(String time, String date, String timeZone)
        {

            String strStatus = "";
            String[] modifyData = new String[1];

            modifyData[0] =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                        "<characteristic type=\"Profile\">" +
                        "<parm name=\"ProfileName\" value=\"Profile1\"/>" +
                        "<characteristic type=\"Clock\" version=\"0.2\">" +
                        "<parm name=\"TimeZone\" value=\"" + timeZone + "\"/>" +
                        "<parm name=\"Date\" value=\"" + date + "\"/>" +
                        "<parm name=\"Time\" value=\"" + time + "\"/>" +
                        "</characteristic>" +
                        "</characteristic>";


            EMDKResults results = profileManager.ProcessProfile(profileName, ProfileManager.PROFILE_FLAG.Set, modifyData);

            if (results.StatusCode == EMDKResults.STATUS_CODE.Success)
            {
                System.Diagnostics.Debug.WriteLine("Success!");

                strStatus = "Profile processed succesfully";
            }
            else if (results.StatusCode == EMDKResults.STATUS_CODE.CheckXml)
            {
                System.Diagnostics.Debug.WriteLine("Checking XML");
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

                    if (checkXmlStatus == "\n\n"){ strStatus = "Profile applied successfully ..."; }
                    else
                    { strStatus = checkXmlStatus; }
                }
            }
            else{ strStatus = "Something wrong on processing the profile"; }

            MessagingCenter.Send<App, string>((App)Xamarin.Forms.Application.Current, "Status", strStatus);

        }


    }
}

