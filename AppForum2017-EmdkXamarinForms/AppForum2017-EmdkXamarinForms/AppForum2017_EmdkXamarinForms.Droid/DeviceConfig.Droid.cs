using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;



[assembly: Dependency(typeof(AppForum2017_EmdkXamarinForms.Droid.DeviceConfig))]

namespace AppForum2017_EmdkXamarinForms.Droid
{
    public class DeviceConfig : IDeviceConfig
    {
        public event EventHandler<Dictionary<String, String>> setClockRequest;

        public void SetClock(string time, string date, string time_zone) {

            Dictionary<String, String> config = new Dictionary<String, String>();
            config.Add("time", time);
            config.Add("date", date);
            config.Add("time_zone", time_zone);

            setClockRequest?.Invoke(this, config);
        }

    }
}