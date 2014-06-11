using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace TMPuzzle.MobileSample
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
public sealed partial class MainPage : Page
{
    public static MobileServiceClient MobileService = new MobileServiceClient(
        "https://xamarinmobile.azure-mobile.net/",
        "fndyrAMMktHwVULfHioZwAFJFWomGu30"
    );
        
    public MainPage()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// データのアップロード
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickUpdate(object sender, RoutedEventArgs e)
    {
        var t = MobileService.GetTable<MobileData>();
        var data = new MobileData
        {
            UserName = "your name",
            Score = 999,
            Modified = DateTime.Now
        };
        await t.InsertAsync(data);
    }

    /// <summary>
    /// データのダウンロード
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void ClickRead(object sender, RoutedEventArgs e)
    {
        var q = from t in MobileService.GetTable<MobileData>()
                    select t;
        var lst = await q.ToListAsync();
        var item = lst.FirstOrDefault<MobileData>();
        System.Diagnostics.Debug.WriteLine( item.ID );
    }
}

[DataTable("mobiledata")]
public class MobileData
{
    [JsonProperty("id")]
    public string ID { get; set; }
    [JsonProperty("username")]
    public string UserName { get; set; }
    [JsonProperty("score")]
    public int Score { get; set; }
    [JsonProperty("modified")]
    public DateTime Modified { get; set; }
}
}
