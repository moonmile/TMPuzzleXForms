using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TMPuzzle.Core.Test
{
    /// <summary>
    /// DoSample の概要の説明
    /// </summary>
    [TestClass]
    public class DoSample
    {
        public DoSample()
        {
            //
            // TODO: コンストラクター ロジックをここに追加します
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        class View
        {
            public void UpdateScore() { }
            public XY SelectMark() { return new XY { X = 0, Y = 0 }; }
        }

        [TestMethod]
        public void TestSample()
        {
            var _model = new DataModel();
            var _logic = new Logic(_model);
            var _view = new View();
            while (true)
            {
                // 残り移動数を減らす
                _model.RestMove--;
                if (_model.RestMove == 0)
                    break;

                // ひとつ目を選択
                XY m1 = _view.SelectMark();
                // ふたつ目を選択
                XY m2 = _view.SelectMark();
                // 選択したマークを入れ替え
                _logic.SwapMark(m1, m2);
                while (true)
                {
                    // マッチをチェック
                    int cnt = _logic.CheckMatch();
                    // マッチがなければおしまい
                    if (cnt == 0)
                        break;
                    // マッチしたマークを消す
                    _logic.RemoveMatch();
                    // スコア表示を更新
                    _view.UpdateScore();
                    // マークを落とす
                    _logic.DropMark();
                    // マークを降らせる
                    _logic.RainMark();
                }
            }
        }
    }
}
