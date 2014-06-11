using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

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
            MainPage page = App.GetMainPage() as MainPage;
            SetPage(page);
            page.OnCreate();    // 初期化


            for (int i = 0; i < DataModel.BOARD_X_MAX * DataModel.BOARD_Y_MAX; i++)
            {
                /*
                ImageView img = FindViewById<ImageView>(Resource.i  _iv[i]);
                img.Tag = i;
                img.Click += img_Click;
                */
            }
        }
    }
}

