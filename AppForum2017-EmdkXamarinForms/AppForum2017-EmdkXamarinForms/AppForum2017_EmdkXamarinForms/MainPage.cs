using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace AppForum2017_EmdkXamarinForms
{
    public class MainPage : ContentPage
    {
        Entry txtTime;
        Entry txtDate;
        Entry txtTimeZone;
        Button btnSubmit;
        Label statusLabel;


        public MainPage()
        {
            this.Padding = new Thickness(20, Device.OnPlatform(40, 20, 20), 20, 20);

            StackLayout panel = new StackLayout
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Vertical,
                Spacing = 15,
            };

            panel.Children.Add(new Label
            {
                Text = "Enter Time:"
            });

            panel.Children.Add(txtTime = new Entry
            {
                Text = "01:30:30"
            });


            panel.Children.Add(new Label
            {
                Text = "Enter Date:"
            });
            panel.Children.Add(txtDate = new Entry
            {
                Text = "2017-04-01"
            });

            panel.Children.Add(new Label
            {
                Text = "Enter TimeZone:"
            });
            panel.Children.Add(txtTimeZone = new Entry
            {
                Text = "GMT-5"
            });

            panel.Children.Add(btnSubmit = new Button
            {
                Text = "Submit Profile"
            });

            btnSubmit.Clicked += OnSubmit;

            panel.Children.Add(new Label
            {
                Text = "Status:"
            });

            panel.Children.Add(statusLabel = new Label
            {
                Text = ""
            });

            this.Content = panel;

             MessagingCenter.Subscribe<App, string> ((App)Xamarin.Forms.Application.Current, "Status", (sender, arg) => {
             	statusLabel.Text = arg;
             });
        }

        void OnSubmit(object sender, System.EventArgs e)
        {
            var list = new List<KeyValuePair<string, string>>() {
                new KeyValuePair<string, string>("time", txtTime.Text),
                new KeyValuePair<string, string>("date", txtDate.Text),
                new KeyValuePair<string, string>("timezone", txtTimeZone.Text)
                };
        
            MessagingCenter.Send<App, List<KeyValuePair<string, string>>>((App)Xamarin.Forms.Application.Current, "ProcessProfile",list);
        }
    }
}
