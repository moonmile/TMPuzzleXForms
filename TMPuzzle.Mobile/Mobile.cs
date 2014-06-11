using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMPuzzle.Core
{
    public class MobileData
    {
        public string ID { get; set; }
        public string UserName { get; set; }
        public int Score { get; set; }
        public DateTime Modified { get; set; }
    }
    public class Mobile
    {
        MobileServiceClient MobileService;

        public Mobile ()
        {
            this.MobileService =
            new MobileServiceClient(
                "https://xamarinmobile.azure-mobile.net/",
                "fndyrAMMktHwVULfHioZwAFJFWomGu30");
        }

        /// <summary>
        /// データ更新
        /// </summary>
        /// <param name="data"></param>
        public async Task Update(MobileData data)
        {
            data.ID = null;
            data.Modified = DateTime.Now;
            var t = MobileService.GetTable<MobileData>();
            await t.InsertAsync(data);
        }
        /// <summary>
        /// データ取得
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<MobileData> Read(string username)
        {
            var q = from t in MobileService.GetTable<MobileData>()
                    where t.UserName == username
                    orderby t.Score descending
                    select t;
            var lst = await q.ToListAsync();
            return lst.Count == 0 ? new MobileData() : lst.First<MobileData>();
        }
    }
}
