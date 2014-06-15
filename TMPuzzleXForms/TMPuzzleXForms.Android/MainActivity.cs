using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using TMPuzzle.Core;


namespace TMPuzzleXForms.Droid
{
    [Activity(Label = "TMPuzzleXForms", MainLauncher = true)]
    public class MainActivity : AndroidActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

            // SetPage(App.GetMainPage());
            var page = App.GetMainPage();
            SetPage(page);
            Disp(this.Window.DecorView);

            Android.Views.View vi = SetNamePageToUIelement("textUserName", page);
            vi.FocusChange += vi_FocusChange;

        }

        void vi_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("focus change");
        }

        void Disp(Android.Views.View vi, string spc = "")
        {
            System.Diagnostics.Debug.WriteLine("{0}{1}", spc, vi.GetType().Name);
            var vg = vi as ViewGroup;
            if (vg != null)
            {
                for (int i = 0; i < vg.ChildCount; i++)
                {
                    var v = vg.GetChildAt(i);
                    Disp(v, spc + " ");
                }
            }
        }
        Android.Views.View SetNamePageToUIelement(string name, Xamarin.Forms.Page page)
        {
            var el = page.FindByName<Xamarin.Forms.View>(name);
            if (el != null)
            {
                var rend = FindRenderer(el);
                if (rend != null)
                {
                    // var en = rend as EntryRenderer;
                    // en.Control.Name = name;
                    // リフレクションで
                    var pa = rend as Android.Views.View;
                    var pi = pa.GetType().GetProperty("Control");
                    var obj = pi.GetValue(pa);
                    // obj.GetType().GetProperty("Name").SetValue(obj, name);

                    return obj as Android.Views.View;
                }
            }
            return null;
        }

        Android.Views.View FindRenderer(Xamarin.Forms.View ent)
        {
            return Search(this.Window.DecorView, ent);
        }
        Android.Views.View Search(Android.Views.View el, Xamarin.Forms.View ent)
        {

            var pa = el as Android.Views.View;
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
                var vg = pa as ViewGroup;
                if (vg != null)
                {
                    for (int i = 0; i < vg.ChildCount; i++)
                    {
                        var ret = Search(vg.GetChildAt(i), ent);
                        if (ret != null)
                        {
                            return ret;
                        }
                    }
                }
            }
            return null;
        }
    }
}
