using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Xamarin.Forms;
using System.Diagnostics;
using System.ComponentModel;
using Xamarin.Forms.Platform.WinPhone;


namespace TMPuzzleXForms.WinPhone
{
    public partial class MainPage : PhoneApplicationPage
    {

        // TMPuzzleXForms.MainPage page;
        public MainPage()
        {
            InitializeComponent();

            Forms.Init();
            // Content = TMPuzzleXForms.App.GetMainPage().ConvertPageToUIElement(this);
            var page = TMPuzzleXForms.App.GetMainPage();
            this.Content = page.ConvertPageToUIElement(this);
            Disp(Content);

            SetNamePageToUIelement("textUserName", page);
            var obj = this.FindName("textUserName") as UIElement;
            obj.LostFocus += obj_LostFocus;
        }


        void obj_LostFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("lost focus");
        }

        void SetNamePageToUIelement(string name, Xamarin.Forms.Page page)
        {
            var el = page.FindByName<View>(name);
            if (el != null)
            {
                var rend = FindRenderer(el);
                if (rend != null)
                {
                    // var en = rend as EntryRenderer;
                    // en.Control.Name = name;
                    // リフレクションで
                    var pa = rend as Panel;
                    var pi = pa.GetType().GetProperty("Control");
                    var obj = pi.GetValue(pa);
                    obj.GetType().GetProperty("Name").SetValue(obj, name);
                }
            }
        }

        UIElement FindRenderer(View ent)
        {
            return Search(this.Content, ent);
        }
        UIElement Search(UIElement el, View ent)
        {

            var pa = el as Panel;
            if (pa != null)
            {

                var pi = pa.GetType().GetProperty("Element");
                if (pi != null)
                {
                    var enel = pi.GetValue(pa);
                    if (enel == ent)
                    {
                        return pa;
                    }
                }
                foreach (var it in pa.Children)
                {
                    var ret = Search(it, ent);
                    if (ret != null)
                    {
                        return ret;
                    }
                }
            }
            return null;
        }


        void Disp(UIElement el, string spc = "")
        {
            var pa = el as Panel;
            if (pa == null)
            {
                Debug.WriteLine("{0}{1}", spc, el.GetType().Name);
            }
            else
            {
                Debug.WriteLine("{0}{1} '{2}'", spc, pa.GetType().Name, pa.Name);
                if (pa.GetType().Name == "EntryRenderer")
                {

                    var en = pa as EntryRenderer;
                    string name = pa.Name;
                    var elem = en.Element;
                    var cont = en.Control;
                }
                foreach (var it in pa.Children)
                {
                    Disp(it, spc + " ");
                }
            }
        }
    }
}
