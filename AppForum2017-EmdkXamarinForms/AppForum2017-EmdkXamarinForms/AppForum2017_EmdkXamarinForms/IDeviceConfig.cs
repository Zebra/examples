using System;
using System.Collections.Generic;


namespace AppForum2017_EmdkXamarinForms
{
    public interface IDeviceConfig
    {
        event EventHandler<Dictionary<String,String>> setClockRequest;
        void SetClock(String time, String date, String time_zone);
    }
}




