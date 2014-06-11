using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TMPuzzle.Core
{
    /// <summary>
    /// 位置を示す補助クラス
    /// </summary>
    public struct XY
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
#if false // INotifyPropertyChanged なし
    /// <summary>
    /// パズル用データモデル
    /// </summary>
    public class DataModel
    {
        public static int BOARD_X_MAX = 7;
        public static int BOARD_Y_MAX = 5;
        public static int COLOR_MAX = 5;

        // 駒ボード
        public int[,] Board { get; set; }
        // 駒ボード（チェック用）
        public int[,] CheckBoard { get; set; }
        // 選択した駒1,2
        public XY SelMark1 { get; set; }
        public XY SelMark2 { get; set; }
        // 残り移動回数
        public int RestMove { get; set; }
        // マッチした数
        public int[] MatchCount { get; set; }
        // ユーザー名
        public string UserName { get; set; }
        // 現在のスコア
        public int Score { get; set; }
        // 最高点
        public int HighScore { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataModel()
        {
            this.Board = new int[BOARD_Y_MAX, BOARD_X_MAX];
            this.CheckBoard = new int[BOARD_Y_MAX, BOARD_X_MAX];
            this.MatchCount = new int[COLOR_MAX];
        }
    }
#else // INotifyPropertyChanged あり
    /// <summary>
    /// パズル用データモデル
    /// </summary>
    public class DataModel : BindableBase
    {
        public static int BOARD_X_MAX = 7;
        public static int BOARD_Y_MAX = 5;
        public static int COLOR_MAX = 5;

        // 駒ボード
        public int[,] Board { get; set; }
        // 駒ボード（チェック用）
        public int[,] CheckBoard { get; set; }
        // 選択した駒1,2
        public XY SelMark1 { get; set; }
        public XY SelMark2 { get; set; }
        // 残り移動回数
        private int _RestMove;
        public int RestMove
        {
            get { return _RestMove; }
            set { this.SetProperty(ref this._RestMove, value); }
        }
        // マッチした数
        public int[] MatchCount { get; set; }
        // ユーザー名
        private string _UserName = "";
        public string UserName
        {
            get { return _UserName; }
            set { this.SetProperty(ref this._UserName, value); }
        }
        // 現在のスコア
        private int _Score;
        public int Score
        {
            get { return _Score; }
            set { this.SetProperty(ref this._Score, value); }
        }
        // 最高点
        private int _HighScore;
        public int HighScore
        {
            get { return _HighScore; }
            set { this.SetProperty(ref this._HighScore, value); }
        }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataModel()
        {
            this.Board = new int[BOARD_Y_MAX, BOARD_X_MAX];
            this.CheckBoard = new int[BOARD_Y_MAX, BOARD_X_MAX];
            this.MatchCount = new int[COLOR_MAX];
        }

        /// <summary>
        /// ボード間のコピー
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public void CopyBoard(int[,] src, int[,] dest)
        {
            for (int y = 0; y < BOARD_Y_MAX; y++)
                for (int x = 0; x < BOARD_X_MAX; x++)
                    dest[y, x] = src[y, x];
        }

        /// <summary>
        /// 範囲チェック付でマークを取得する
        /// 範囲外の時は-1を返す
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int GetCol(int x, int y)
        {
            if (x < 0 || DataModel.BOARD_X_MAX <= x) return -1;
            if (y < 0 || DataModel.BOARD_Y_MAX <= y) return -1;
            return this.Board[y, x];
        }
    }
#endif

    /// <summary>
    /// シリアライズ用のクラス
    /// </summary>
    public class MyData
    {
        public int[] Board;
        public int RestMove;
        public int[] MatchCount;
        public string UserName;
        public int Score;
        public int HighScore;
        public DateTime Modified;
        public string ID;

        /// <summary>
        /// データモデルにコピー
        /// </summary>
        /// <param name="dest"></param>
        public void CopyTo(DataModel dest)
        {
            int i = 0;
            for (int y = 0; y < DataModel.BOARD_Y_MAX; y++)
                for (int x = 0; x < DataModel.BOARD_X_MAX; x++, i++)
                    dest.Board[y, x] = this.Board[i];
            dest.RestMove = this.RestMove;
            for (int x = 0; x < this.MatchCount.Length; x++)
                dest.MatchCount[x] = this.MatchCount[x];
            dest.UserName = this.UserName;
            dest.Score = this.Score;
            dest.HighScore = this.HighScore;
        }
        /// <summary>
        /// データモデルからコピー
        /// </summary>
        /// <param name="src"></param>
        public void CopyFrom(DataModel src)
        {
            this.Board = new int[src.Board.Length];
            int i = 0;
            for (int y = 0; y < DataModel.BOARD_Y_MAX; y++)
                for (int x = 0; x < DataModel.BOARD_X_MAX; x++, i++)
                    this.Board[i] = src.Board[y, x];
            this.RestMove = src.RestMove;
            this.MatchCount = new int[src.MatchCount.Length];
            src.MatchCount.CopyTo(this.MatchCount, 0);
            this.UserName = src.UserName;
            this.Score = src.Score;
            this.HighScore = src.HighScore;
            this.Modified = DateTime.Now;
        }
    }
}
