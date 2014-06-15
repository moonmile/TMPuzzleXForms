using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using Xamarin.Forms;
using System.Diagnostics;
using Xamarin.Forms.Platform.iOS;

namespace TMPuzzleXForms.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Forms.Init();

            window = new UIWindow(UIScreen.MainScreen.Bounds);

            // window.RootViewController = App.GetMainPage().CreateViewController();
            var page = App.GetMainPage();
            window.RootViewController = page.CreateViewController();
            window.MakeKeyAndVisible();

            Disp(window.RootViewController);

            UIControl uc = SetNamePageToUIelement("textUserName", page);
            var obj = uc as UITextField;
            obj.AllTouchEvents += obj_AllTouchEvents;
            
            return true;
        }

        void obj_AllTouchEvents(object sender, EventArgs e)
        {
            Debug.WriteLine("text event");
        }

        void Disp(UIViewController vc, string spc = "")
        {
            Debug.WriteLine("{0}{1}", spc, vc.GetType().Name);
            Disp(vc.View);
        }
        void Disp(UIView vi, string spc = "")
        {
            Debug.WriteLine("{0}{1}", spc, vi.GetType().Name);
            foreach (var it in vi.Subviews)
            {
                Disp(it, spc + " ");
            }
        }

        UIControl SetNamePageToUIelement(string name, Xamarin.Forms.Page page)
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
                    var pa = rend as UIView;
                    var pi = pa.GetType().GetProperty("Control");
                    var obj = pi.GetValue(pa);
                    //obj.GetType().GetProperty("Name").SetValue(obj, name);

                    return obj as UIControl;
                }
            }
            return null;
        }

        UIView FindRenderer(View ent)
        {
            return Search(window.RootViewController.View, ent);
        }
        UIView Search(UIView el, View ent)
        {

            var pa = el as UIView;
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
                foreach (var it in pa.Subviews)
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
    }
}
