using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TMPuzzle.Core.Test
{
    public static class DataModelExtentions
    {
        /// <summary>
        /// 文字列でデータを設定する
        /// </summary>
        /// <param name="m"></param>
        /// <param name="data"></param>
        public static void SetBoard(this DataModel m, string data)
        {
            if (data.Length < DataModel.BOARD_X_MAX * DataModel.BOARD_Y_MAX) return;
            for (int y = 0; y < DataModel.BOARD_Y_MAX; y++)
            {
                for (int x = 0; x < DataModel.BOARD_X_MAX; x++)
                {
                    int col = int.Parse(data[y * DataModel.BOARD_Y_MAX + x].ToString());
                    m.Board[y, x] = col;
                }
            }
        }
        /// <summary>
        /// 文字列でデータを取得する
        /// </summary>
        /// <returns></returns>
        public static string GetBoard(this DataModel m)
        {
            string s = "";
            for (int y = 0; y < DataModel.BOARD_Y_MAX; y++)
            {
                for (int x = 0; x < DataModel.BOARD_X_MAX; x++)
                {
                    int col = m.Board[y, x];
                    s += col.ToString();
                }
            }
            return s;
        }
    }


    [TestClass]
    public class LogicTest
    {
        [TestMethod]
        public void TestCheckMatch1()
        {
            DataModel.BOARD_X_MAX = 4;
            DataModel.BOARD_Y_MAX = 4;
            var _model = new DataModel();
            var _logic = new Logic(_model);
            _model.SetBoard(
                "1234" +
                "2341" +
                "3412" +
                "4123");
            // マッチしない
            Assert.AreEqual(0, _logic.CheckMatch());

            _model.SetBoard(
                "1234" +
                "2221" +
                "3412" +
                "4123");
            // マッチする
            Assert.AreEqual(3, _logic.CheckMatch());

            _model.SetBoard(
                "1234" +
                "2221" +
                "3212" +
                "4123");
            // マッチする
            Assert.AreEqual(5, _logic.CheckMatch());
        }

        [TestMethod]
        public void TestCheckMatch2()
        {
            DataModel.BOARD_X_MAX = 4;
            DataModel.BOARD_Y_MAX = 4;
            var _model = new DataModel();
            var _logic = new Logic(_model);

            _model.SetBoard(
                "1234" +
                "2221" +
                "3211" +
                "4121");
            // マッチする
            Assert.AreEqual(8, _logic.CheckMatch());

            _logic.RemoveMatch();
            string data = _model.GetBoard();
            Assert.AreEqual(
                "1034" +
                "0000" +
                "3010" +
                "4120",
                data);
        }

        [TestMethod]
        public void TestDropMark1()
        {
            DataModel.BOARD_X_MAX = 4;
            DataModel.BOARD_Y_MAX = 4;
            var _model = new DataModel();
            var _logic = new Logic(_model);
            _model.SetBoard(
                "1234" +
                "2341" +
                "3412" +
                "4123");
            _logic.DropMark();
            Assert.AreEqual(
                "1234" +
                "2341" +
                "3412" +
                "4123",
                _model.GetBoard());

            _model.SetBoard(
                "1234" +
                "2341" +
                "3012" +
                "4123");
            _logic.DropMark();
            Assert.AreEqual(
                "1034" +
                "2241" +
                "3312" +
                "4123",
                _model.GetBoard());

        }

        [TestMethod]
        public void TestRainMark1()
        {
            DataModel.BOARD_X_MAX = 4;
            DataModel.BOARD_Y_MAX = 4;
            var _model = new DataModel();
            var _logic = new Logic(_model);

            _model.SetBoard(
                "1234" +
                "2341" +
                "3412" +
                "4123");
            _logic.RainMark();
            Assert.AreEqual(
                "1234" +
                "2341" +
                "3412" +
                "4123",
                _model.GetBoard());

            _logic._rnd = new Random(0);
            _model.SetBoard(
                "1204" +
                "2301" +
                "3412" +
                "4123");
            _logic.RainMark();
            Assert.AreEqual(
                "1244" +
                "2351" +
                "3412" +
                "4123",
                _model.GetBoard());

        }
    }
}
