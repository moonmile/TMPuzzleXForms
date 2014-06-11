using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMPuzzle.Core
{
    public class Logic
    {
        // 対象モデル
        public DataModel Model { get; set; }
        // ランダムオブジェクト
        public Random _rnd = new Random();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model"></param>
        public Logic(DataModel model)
        {
            this.Model = model;
        }

        /// <summary>
        /// スワップ可能かチェックする
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        /// <returns></returns>
        public bool CanSwap(XY m1, XY m2)
        {
            // 範囲チェック
            if (m1.X < 0 || DataModel.BOARD_X_MAX <= m1.X) return false;
            if (m1.Y < 0 || DataModel.BOARD_Y_MAX <= m1.Y) return false;
            if (m2.X < 0 || DataModel.BOARD_X_MAX <= m2.X) return false;
            if (m2.Y < 0 || DataModel.BOARD_Y_MAX <= m2.Y) return false;
            // 隣同士のみ交換可能
            if (m1.X == m2.X && (m1.Y == m2.Y - 1 || m1.Y == m2.Y + 1)) return true;
            if (m1.Y == m2.Y && (m1.X == m2.X - 1 || m1.X == m2.X + 1)) return true;

            return false;

        }

        /// <summary>
        /// 2つのマークを入れ替える
        /// </summary>
        /// <param name="m1"></param>
        /// <param name="m2"></param>
        public void SwapMark(XY m1, XY m2)
        {
            // 範囲チェック
            if (m1.X < 0 || DataModel.BOARD_X_MAX <= m1.X) return;
            if (m1.Y < 0 || DataModel.BOARD_Y_MAX <= m1.Y) return;
            if (m2.X < 0 || DataModel.BOARD_X_MAX <= m2.X) return;
            if (m2.Y < 0 || DataModel.BOARD_Y_MAX <= m2.Y) return;

            int tmp = Model.Board[m1.Y, m1.X];
            Model.Board[m1.Y, m1.X] = Model.Board[m2.Y, m2.X];
            Model.Board[m2.Y, m2.X] = tmp;
        }

        /// <summary>
        /// マッチ状態をチェックする
        /// </summary>
        /// <returns></returns>
        public int CheckMatch()
        {
            // 縦横3つ以上あれば消えるルール
            // 1.マッチ箇所を保持するボードへコピー
            Model.CopyBoard(Model.Board, Model.CheckBoard);
            // 2.左上から順番にチェック 
            for (int y = 0; y < DataModel.BOARD_Y_MAX; y++)
            {
                for (int x = 0; x < DataModel.BOARD_X_MAX; x++)
                {
                    // 4連の場合も重なって消える
                    int col = Model.Board[y, x];
                    // 横3連チェック
                    if (Model.GetCol(x - 1, y) == col &&
                         Model.GetCol(x + 1, y) == col)
                    {
                        Model.CheckBoard[y, x] = 0;
                        Model.CheckBoard[y, x - 1] = 0;
                        Model.CheckBoard[y, x + 1] = 0;
                    }
                    // 縦3連チェック
                    if (Model.GetCol(x, y - 1) == col &&
                         Model.GetCol(x, y + 1) == col)
                    {
                        Model.CheckBoard[y, x] = 0;
                        Model.CheckBoard[y - 1, x] = 0;
                        Model.CheckBoard[y + 1, x] = 0;
                    }
                }
            }
            // 3.空き数を返す
            int cnt = 0;
            foreach (int col in Model.CheckBoard)
            {
                if (col == 0)
                    cnt++;
            }
            return cnt;
        }



        /// <summary>
        /// 新しいマークを取得
        /// </summary>
        /// <returns></returns>
        private int getNewCol()
        {
            int col = _rnd.Next(DataModel.COLOR_MAX) + 1;
            return col;
        }

        /// <summary>
        /// マッチしたマークを消す
        /// </summary>
public void RemoveMatch()
{
#if true 
    for (int y = 0; y < DataModel.BOARD_Y_MAX; y++)
    {
        for (int x = 0; x < DataModel.BOARD_X_MAX; x++)
        {
            if (Model.CheckBoard[y, x] == 0)
            {
                // スコアを更新
                UpdateScore(x, y);
                // 消す
                Model.Board[y, x] = 0;
            }
        }
    }
#else
    // 同じ色をたくさん消すと得点をアップする
    for (int col = 1; col < DataModel.COLOR_MAX; col++)
    {
        int score = 0;
        for (int y = 0; y < DataModel.BOARD_Y_MAX; y++)
        {
            for (int x = 0; x < DataModel.BOARD_X_MAX; x++)
            {
                if (Model.Board[y, x] == col &&
                        Model.CheckBoard[y, x] == 0)
                {
                    score = score == 0? 1: score*2;
                    // 消す
                    Model.Board[y, x] = 0;
                }
            }
        }
        if (score > 0)
        {
            // スコアを更新
            Model.MatchCount[col - 1] += score;
            Model.Score += score;
        }
    }

#endif
}
        /// <summary>
        /// 空きが埋まるようにマークを落とす
        /// </summary>
        public void DropMark()
        {
            // 左下からチェック
            for (int x = 0; x < DataModel.BOARD_X_MAX; x++)
            {
                int y0 = DataModel.BOARD_Y_MAX - 1;
                for (int y = DataModel.BOARD_Y_MAX - 1; y >= 0; y--)
                {
                    if (Model.Board[y, x] != 0)
                    {
                        Model.Board[y0, x] = Model.Board[y, x];
                        y0--;
                    }
                }
                // 残りを0で埋める
                for (int y = y0; y >= 0; y--)
                {
                    Model.Board[y, x] = 0;
                }
            }
        }

        /// <summary>
        /// マークを上から落とす
        /// </summary>
        public void RainMark()
        {
            // 空きを埋める
            for (int y = 0; y < DataModel.BOARD_Y_MAX; y++)
            {
                for (int x = 0; x < DataModel.BOARD_X_MAX; x++)
                {
                    if (Model.Board[y, x] == 0)
                    {
                        Model.Board[y, x] = getNewCol();
                    }
                }
            }
        }

        /// <summary>
        /// スコアを更新する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void UpdateScore(int x, int y)
        {
            int col = Model.Board[y, x];
            if (col != 0)
            {
                // 空白以外
                Model.MatchCount[col - 1]++;
                Model.Score++;
            }
        }

        /// <summary>
        /// リセット
        /// </summary>
        public void Reset()
        {
            this.Model.Score = 0;
            for (int i = 0; i < Model.MatchCount.Length; i++) { Model.MatchCount[i] = 0; }
            for (int i = 0; i < DataModel.BOARD_X_MAX * DataModel.BOARD_Y_MAX; i++)
            {
                int col = this._rnd.Next(DataModel.COLOR_MAX) + 1;
                int y = i / DataModel.BOARD_X_MAX;
                int x = i - y * DataModel.BOARD_X_MAX;
                this.Model.Board[y, x] = col;
            }
            this.Model.RestMove = 10;
        }

    }
}
