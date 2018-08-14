using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoTransaction.DataModel
{
    public class WarmingData
    {
        //证券代码
        public string code { get; set; }

        //预警条件
        public string condition { get; set; }

        //预警时间
        public string time { get; set; }

        //预警价格

        public string price { get; set; }

        //现价/盈亏

        public string nowprice { get; set; }

    }
}
