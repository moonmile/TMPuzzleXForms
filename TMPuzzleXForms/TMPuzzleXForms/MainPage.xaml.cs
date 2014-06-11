using System;
using System.Collections.Generic;
using Xamarin.Forms;
using TMPuzzle.Core;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;

namespace TMPuzzleXForms
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        DataModel _model;
        Logic _logic;
        Mobile _mobile;
        Image[] _marks;         // マーク群
        Image _sel1, _sel2;     // 選択したマーク
        ImageSource[] _mk;      // マーク画像
        Dictionary<Element, int> _tags;

        // 画面ロード時
//        public async void OnCreate()
        protected async override void OnAppearing()
        {
 	        base.OnAppearing();
            
            _model = new DataModel();
            _logic = new Logic(_model);

            _mk = new ImageSource[DataModel.COLOR_MAX + 1];
            _mk[0] = ImageSource.FromFile("MarkNone.png");
            _mk[1] = ImageSource.FromFile("MarkBlue.png");
            _mk[2] = ImageSource.FromFile("MarkRed.png");
            _mk[3] = ImageSource.FromFile("MarkGreen.png");
            _mk[4] = ImageSource.FromFile("MarkOrange.png");
            _mk[5] = ImageSource.FromFile("MarkPurple.png");

            // Prepare for tap recognition
            TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer
            {
                TappedCallback = img_Click
            };
            // 表示用マークにイベントをつける
            _marks = new Image[DataModel.BOARD_X_MAX * DataModel.BOARD_Y_MAX];
            _tags = new Dictionary<Element, int>();
            for (int i = 0; i < DataModel.BOARD_X_MAX * DataModel.BOARD_Y_MAX; i++)
            {
                var img = _marks[i] = this.FindByName<Image>(string.Format("mark{0}", i));
                img.GestureRecognizers.Add(tapGestureRecognizer);
                _tags[img] = i;
            }
            var se = new XmlSerializer(typeof(MyData));
            try
            {
#if __IOS__ || __ANDROID__ 
                var documents =
                    System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                var file = System.IO.Path.Combine(documents, "mydata.xml");
                using (var stream = System.IO.File.OpenRead(file))
                {
                    var m = se.Deserialize(stream) as MyData;
                    m.CopyTo(_model);
                }
#else
                using (var stream = await Windows.Storage.ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(
                    "mydata.xml"))
                {
                    var m = se.Deserialize(stream) as MyData;
                    m.CopyTo(_model);
                }
#endif
            }
            catch
            {
                // 各スコアを0にする
                this._logic.Reset();
            }
            // 画面を更新
            UpdateView();
            // モバイルサービスへ接続
#if __IOS__ || __ANDROID__
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
#endif
            _mobile = new Mobile();
            // ハイスコアを取得
            var data = await _mobile.Read(_model.UserName);
            _model.HighScore = data.Score;
            this.textHighScore.Text = _model.HighScore.ToString();
            this.textUserName.TextChanged += (_, __) => { _model.UserName = this.textUserName.Text; };
        }



        // マークを選択時
        bool _flag = false;
        /// <summary>
        /// マークを選択時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void img_Click(View view, object args)
        {
            // 再入禁止
            if (_flag == true) return;
            _flag = true;
            Image mark = view as Image;
            if (mark == _sel1)
            {
                _sel1 = null; mark.Opacity = 1.0;
            }
            else if (mark == _sel2)
            {
                _sel2 = null; mark.Opacity = 1.0;
            }
            else
            {
                if (_sel1 == null)
                {
                    mark.Opacity = 0.7;
                    _sel1 = mark;
                }
                else
                {
                    mark.Opacity = 0.7;
                    _sel2 = mark;
                    // カウンタを減らす
                    _model.RestMove--;
                    if (_model.RestMove >= 0)
                    {

                        // ここでスワップ＆チェック...
                        await LogicCheck();
                        _sel1.Opacity = 1.0;
                        _sel2.Opacity = 1.0;
                        _sel1 = _sel2 = null;
                    }
                    // 状態をローカルストレージに保存
                    var se = new XmlSerializer(typeof(MyData));
#if __IOS__ || __ANDROID__
                    var documents =
                        System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                    var file = System.IO.Path.Combine(documents, "mydata.xml");
                    using (var stream = System.IO.File.OpenWrite(file))
                    {
                        var m = new MyData();
                        m.CopyFrom(_model);
                        stream.SetLength(0);
                        stream.Position = 0;
                        se.Serialize(stream, m);
                    }
#else
                    using (var stream = await Windows.Storage.ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                        "mydata.xml", Windows.Storage.CreationCollisionOption.OpenIfExists))
                    {
                        var m = new MyData();
                        m.CopyFrom(_model);
                        stream.SetLength(0);
                        stream.Position = 0;
                        se.Serialize(stream, m);
                    }
#endif

                    if (_model.RestMove == 0)
                    {
                        // 終了した場合はモバイルサービスへ登録
                        await _mobile.Update(new MobileData
                        {
                            UserName = _model.UserName,
                            Score = _model.Score
                        });
                    }
                }
            }
            _flag = false;
        }
        private async Task LogicCheck()
        {
            await Wait();
            // スワップ
            int y1 = _tags[_sel1] / DataModel.BOARD_X_MAX;
            int x1 = _tags[_sel1] - y1 * DataModel.BOARD_X_MAX;
            int y2 = _tags[_sel2] / DataModel.BOARD_X_MAX;
            int x2 = _tags[_sel2] - y2 * DataModel.BOARD_X_MAX;
            var m1 = new XY { X = x1, Y = y1 };
            var m2 = new XY { X = x2, Y = y2 };
            _logic.SwapMark(m1, m2);
            // 画面を更新
            UpdateView();
            await Wait();

            while (true)
            {

                // マークをチェックする
                int cnt = _logic.CheckMatch();
                UpdateView();
                await Wait();
                if (cnt == 0) break;

                // マッチしたマークを消す
                _logic.RemoveMatch();
                UpdateView();
                await Wait();
                // スコア表示を更新
                // _view.UpdateScore();
                // マークを落とす
                _logic.DropMark();
                UpdateView();
                await Wait();
                // マークを降らせる
                _logic.RainMark();
                UpdateView();
                await Wait();
            }
        }

        /// <summary>
        /// ビューを更新
        /// </summary>
        private void UpdateView()
        {
            for (int y = 0; y < DataModel.BOARD_Y_MAX; y++)
            {
                for (int x = 0; x < DataModel.BOARD_X_MAX; x++)
                {
                    _marks[y * DataModel.BOARD_X_MAX + x].Source = _mk[_model.Board[y, x]];
                }
            }
            // スコアを更新
            this.textScoreBlue.Text = this._model.MatchCount[0].ToString();
            this.textScoreRed.Text = this._model.MatchCount[1].ToString();
            this.textScoreGreen.Text = this._model.MatchCount[2].ToString();
            this.textScoreOrange.Text = this._model.MatchCount[3].ToString();
            this.textScorePurple.Text = this._model.MatchCount[4].ToString();
            this.textScore.Text = string.Format("{0}", this._model.Score * 10);
            // 残り移動数を更新
            this.textRestMove.Text = _model.RestMove.ToString();
        }

        /// <summary>
        /// 再描画のためのウェイト
        /// </summary>
        /// <param name="sc"></param>
        /// <returns></returns>
        private Task Wait(double sc = 1.0)
        {
            return Task.Delay((int)(sc * 1000));
        }

        protected void buttonClick(object s, EventArgs e)
        {
            // 各スコアを0にする
            this._logic.Reset();
            // 画面を更新
            UpdateView();
        }
    }
}
