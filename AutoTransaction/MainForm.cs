using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AutoTransaction.Common;
using AutoTransaction.DataModel;
using System.Windows.Automation;
using System.Threading;
namespace AutoTransaction
{
    public partial class MainForm : Form
    {
        //大智慧预警数据
        static Dictionary<string, DataItem> DZH_DataList = new Dictionary<string, DataItem>();
        //大智慧预警数据DataListView AutomationElementObj
        static AutomationElement DZH_uiElement;
        //中投证券买入按钮 AutomationElementObj
        static AutomationElement ZT_BuyButtonElement;
        //中投证券买入下单 证券代码框 AutomationElementObj
        static AutomationElement ZT_BuySecuritiesCode;
        //中投证券买入 买入数量 AutomationElementObj
        static AutomationElement ZT_BuyNum;
        //中投证券买入 持仓单DataListView AutomationElementObj
        static AutomationElement ZT_BuyListView;
        //中投证券买入下单按钮 AutomationElementObj
        static AutomationElement ZT_BuyOrder;
        //中投证券买入确认 AutomationElementObj
        static AutomationElement ZT_BuyConfirm;
        //中投证券可用资金 AutomationElementObj
        static AutomationElement ZT_CanUseMoney;
        //中投证券买价 AutomationElementObj
        static AutomationElement ZT_BuyPrice;
        //中投证券卖出按钮 AutomationElementObj
        static AutomationElement ZT_SaleButtonElement;
        //中投证券卖出下单 证券代码框 AutomationElementObj
        static AutomationElement ZT_SaleSecuritiesCode;
        //中投证券卖出 卖出数量 AutomationElementObj
        static AutomationElement ZT_SaleNum;
        //中投证券卖出 持仓单DataListView AutomationElementObj
        static AutomationElement ZT_SaleListView;
        //中投证券卖出下单按钮 AutomationElementObj
        static AutomationElement ZT_SaleOrder;
        //中投证券卖出确认 AutomationElementObj
        static AutomationElement ZT_SaleConfirm;
        //中投证券持仓按钮 AutomationElementObj
        static AutomationElement ZT_PositionOrder;
        //中投证券持仓输出按钮
        static AutomationElement ZT_Output;
        //中投证券持仓输出确认按钮
        static AutomationElement ZT_OutputSuess;
        //中投证券持仓单数据
        static Dictionary<string, DataItem> ZT_DataList = new Dictionary<string, DataItem>();

        static AutomationElement buyWindowsElement;
        //预警数据
        static BindingList<WarmingData> bdlist;
        //启动判断
        static bool isRun = false;
        //卖出公式参数A
        public static double[] A_param = new double[5];
        //卖出公式参数B
        public static double[] B_param = new double[3];
        //卖出公式参数C
        public static double[] C_param = new double[4];
        KeyboardHook k_hook;
        public MainForm()
        {
            k_hook = new KeyboardHook();
            k_hook.KeyDownEvent += new KeyEventHandler(hook_KeyDown);//钩住键按下
            k_hook.Start();//安装键盘钩子
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var config = new Form1();
            config.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            output("句柄加载中....");
            try
            {
                initialization();
                output("句柄加载成功!");
            }
            catch (Exception ex)
            {
                output("句柄加载异常请重新加载!");
            }
        }
        
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="_content">日志内容</param>
        public  void output(string _content)
        {
            if (textBox1.GetLineFromCharIndex(textBox1.Text.Length) > 100)
                textBox1.Text = "";
            textBox1.AppendText(DateTime.Now.ToString("HH:mm:ss  ") + _content + "\r\n");
            
        }

        /// <summary>
        /// 截取字符串中的数字
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <returns>截取数字字符串</returns>
        static public string IsNum(String str)
        {
            string ss = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (Char.IsNumber(str, i) == true)
                {
                    ss += str.Substring(i, 1);
                }
                else
                {
                    if (str.Substring(i, 1) == ",")
                    {
                        ss += str.Substring(i, 1);
                    }
                }

            }
            return ss;
        }

        /// <summary>
        /// 获取实时股票数据(仅限深A)
        /// </summary>
        /// <param name="_stockCode">股票代码</param>
        /// <returns></returns>
        static string[] getSotckData(string _stockCode)
        {
            //ReadWarmingOrder()
            HttpClientTool post = HttpClientTool.GetInstance();
            var list = new Dictionary<string, string>();
            var data = "";
            if (_stockCode.StartsWith("0")||_stockCode.StartsWith("3"))
            {
                 data = post.doPost("http://hq.sinajs.cn/list=sz" + _stockCode, list);
            }
            else if(_stockCode.StartsWith("6"))
            {
                 data = post.doPost("http://hq.sinajs.cn/list=sh" + _stockCode, list);
            }
            else
            {
                return null;
            }
            data = data.Substring(data.IndexOf(","));
            
            data = data.Replace("\"", "");
            data = data.Replace(";", "");
            var datalist = data.Split(',');
            return datalist;
        }
        /// <summary>
        /// 股票买入
        /// </summary>
        /// <param name="_securitiesCode">证券代码</param>
        /// <param name="_num">数量</param>
        static void BuyOrder(string _securitiesCode)
        {
            var orderClick = new iAutomationElement();
            orderClick.InvokeButton(ZT_BuyButtonElement);
            Thread.Sleep(500);
            orderClick.WriteTextBox(ZT_BuySecuritiesCode, "\b\b\b\b\b\b");
            orderClick.WriteTextBox(ZT_BuySecuritiesCode, _securitiesCode);
            Thread.Sleep(500);
            var datalists = getSotckData(_securitiesCode);
            var sn = ZT_CanUseMoney.Current.Name.ToString().Replace(" ", "");
            var n = Convert.ToDecimal(sn);
            var m = Convert.ToDecimal(datalists[3]);
            var canbuynum = Convert.ToInt32(n / m);
            var _num = NumCalculation.GetBuyNum(datalists, canbuynum);
            _num = _num / 100 * 100;
            if(_num != 0)
            {
                orderClick.WriteTextBox(ZT_BuyNum, "\b\b\b\b\b\b");
                orderClick.WriteTextBox(ZT_BuyNum, _num.ToString());
                //orderClick.ButtonClick(ZT_BuyOrder);
                orderClick.InvokeButton(ZT_BuyOrder);
                
                GetConfirm("买入确认");
                if (ZT_BuyConfirm != null)
                {
                    orderClick.InvokeButton(ZT_BuyConfirm);
                    ZT_BuyConfirm = null;
                    Clickconfirm();
                }
                              

            }
            
        }
        /// <summary>
        /// 股票卖出
        /// </summary>
        /// <param name="_securitiesCode">证券代码</param>
        static void SaleOrder(string _securitiesCode)
        {
            var orderClick = new iAutomationElement();
            var datalist = getSotckData(_securitiesCode);
            var positiondata = ZT_DataList[_securitiesCode];
            var _num = NumCalculation.GetSaleNum(positiondata.data,datalist,A_param,B_param,C_param);
            _num =( Convert.ToInt32( _num) / 100 * 100 ).ToString();
            if (_num != "0")
            {
                orderClick.InvokeButton(ZT_SaleButtonElement);
                Thread.Sleep(500);
                orderClick.WriteTextBox(ZT_SaleSecuritiesCode, "\b\b\b\b\b\b");
                orderClick.WriteTextBox(ZT_SaleSecuritiesCode, _securitiesCode);
                Thread.Sleep(500);
                orderClick.WriteTextBox(ZT_SaleNum, "\b\b\b\b\b\b");
                orderClick.WriteTextBox(ZT_SaleNum, _num);
                Thread.Sleep(500);
                orderClick.InvokeButton(ZT_SaleOrder);
                
                GetConfirm("卖出确认");
                if(ZT_SaleConfirm != null)
                {
                    orderClick.InvokeButton(ZT_SaleConfirm);
                    ZT_SaleConfirm = null;
                    Clickconfirm();
                }
                

            }
            
        }

        /// <summary>
        /// 基础数据初始化
        /// </summary>
         void initialization()
        {
            //读取需要的句柄以及UIElement
            //1.读取预警列表UIElement     DZH_uiElement  
            //GetReadWaringListViewElement();
            //2.买入按钮UIElement
            GetZT_OrderButtonElement("买入");
            //3.卖出按钮UIElement
            GetZT_OrderButtonElement("卖出");
            //4.获取买入界面UIElement
            var click = new iAutomationElement();
            //点击买入按钮
            click.InvokeButton(ZT_BuyButtonElement);
            ////获取持仓单UIElement
            //GetZTViewListElement("买入下单");
            //获取买入界面证券代码TextBox UIElement
            GetZTSecodeElement("买入下单");
            //输入证券代码
            click.WriteTextBox(ZT_BuySecuritiesCode, "000001");
            //获取NumBoxUIElement
            GetNumboxElement("买入下单");
            //获取ZT_BuyOrderUIElement
            GetZTOrder("买入下单");
            //获取 ZT_CanUseMoneyUIElement
            GetCanUseMoney();
            //获取 ZT_BuyPriceUIElement
            //GetBuyPrice();
           
            //5.获取卖出界面UIElement           
            //点击买入按钮
            click.InvokeButton(ZT_SaleButtonElement);
            ////获取持仓单UIElement
            //GetZTViewListElement("卖出下单");
            //获取买入界面证券代码TextBox UIElement
            GetZTSecodeElement("卖出下单");
            //输入证券代码
            click.WriteTextBox(ZT_SaleSecuritiesCode, "000001");
            //获取NumBoxUIElement
            GetNumboxElement("卖出下单");
            //获取ZT_SaleOrderUIElement
            GetZTOrder("卖出下单");
            //获取持仓按钮 
            GetZT_PositionOrderButtonElement();
            //获取A参数
            string textA = IniFunc.GetString("Param", "A", "", Application.StartupPath + "\\config.ini").Trim();
            if (textA.Contains("|"))
            {

                string[] array = textA.Split(new char[]
                   {
                        '|'
                   });
                for (int i = 0; i < array.Count(); i++)
                    A_param[i] = Convert.ToDouble(array[i]);
                
            }
            //获取B参数
            string textB = IniFunc.GetString("Param", "B", "", Application.StartupPath + "\\config.ini").Trim();
            if (textB.Contains("|"))
            {

                string[] array = textB.Split(new char[]
                   {
                        '|'
                   });
                for (int i = 0; i < array.Count(); i++)
                    B_param[i] = Convert.ToDouble(array[i]);

            }
            //获取C参数
            string textC= IniFunc.GetString("Param", "C", "", Application.StartupPath + "\\config.ini").Trim();
            if (textC.Contains("|"))
            {

                string[] array = textC.Split(new char[]
                   {
                        '|'
                   });
                for (int i = 0; i < array.Count(); i++)
                    C_param[i] = Convert.ToDouble(array[i]);

            }
           

        }

       

        /// <summary>
        /// 获取确认交易按钮 UIElement
        /// </summary>
        /// <param name="_type">"买入确认" or "卖出确认"</param>
        static void GetConfirm(string _type)
        {
            try
            {
                var uielement = new iAutomationElement();
                var elementlist = uielement.enumRoot();
                elementlist = uielement.FindByName("中投证券", elementlist);
                elementlist = uielement.enumNode(elementlist[0]);
                if (elementlist.Count > 1)
                {
                    foreach (AutomationElement item in elementlist)
                    {
                        var list = uielement.enumDescendants(item, _type);
                        if (list.Count > 0)
                        {
                            if (_type == "买入确认")
                            {
                                ZT_BuyConfirm = list[0];
                            }
                            else if (_type == "卖出确认")
                            {
                                ZT_SaleConfirm = list[0];
                            }
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }
        
        static void Clickconfirm()
        {
            var uielement = new iAutomationElement();
            var elementlist = uielement.enumRoot();
            elementlist = uielement.FindByName("中投证券", elementlist);
            //uielement.ESCclick(elementlist[0]);
            elementlist = uielement.enumNode(elementlist[0]);
            if (elementlist.Count > 1)
            {
                foreach (AutomationElement item in elementlist)
                {

                    var list = uielement.enumDescendants(item, "提示");
                    
                    if (list.Count > 0)
                    {
                        Console.WriteLine("ture");
                        buyWindowsElement = TreeWalker.RawViewWalker.GetParent(list[0]);
                        elementlist = uielement.enumNode(buyWindowsElement);
                        elementlist = uielement.FindByName("确认", elementlist);
                        var orderClick = new iAutomationElement();
                        orderClick.InvokeButton(elementlist[0]);
                        //return;


                    }
                }
            }
        }

        /// <summary>
        /// 大智慧预警表读取
        /// </summary>
        void GetReadWaringListViewElement()
        {
            var uielement = new iAutomationElement();
            var elemlentlist = uielement.enumRoot();
            elemlentlist = uielement.FindByName("大智慧", elemlentlist);
            if (elemlentlist.Count > 0)
            {
                elemlentlist = uielement.enumNode(elemlentlist[0]);
                elemlentlist = uielement.FindByName("预警", elemlentlist);
                if(elemlentlist.Count > 0)
                {
                    elemlentlist = uielement.enumNode(elemlentlist[0]);
                    elemlentlist = uielement.FindByName("List2", elemlentlist);
                    if (elemlentlist.Count > 0)
                    {
                        DZH_uiElement = elemlentlist[0];
                        DZH_DataList = uielement.GetViewList(elemlentlist[0], 5);
                    }
                    else
                    {
                        output("预警列表加载失败");
                        return;
                    }
                   
                }
                else
                {
                    output("预警列表未打开");
                    return;
                }
                
            }        
            else
            {
                output("未找到大智慧句柄");
                return;
            }
                
                
           

        }

        /// <summary>
        /// 获取持仓单UIElement
        /// 调用前需要点击买入or卖出按钮切换界面
        /// </summary>
        /// <param name="_type"  >"买入下单"or "卖出下单"</param>
        static void GetZTViewListElement(string _type)
        {
            var uielement = new iAutomationElement();
            var elementlist = uielement.enumRoot();
            elementlist = uielement.FindByName("中投证券", elementlist);
            elementlist = uielement.enumNode(elementlist[0]);
            if (elementlist.Count > 1)
            {
                foreach (AutomationElement item in elementlist)
                {
                    var list = uielement.enumDescendants(item, _type);
                    if (list.Count > 0)
                    {
                        buyWindowsElement = TreeWalker.RawViewWalker.GetParent(list[0]);
                        elementlist = uielement.enumNode(buyWindowsElement);
                        
                        elementlist = uielement.FindByClassName("SysListView32", elementlist);
                        if (_type == "买入下单")
                        {
                            ZT_BuyListView = elementlist[0];
                        }
                        else if (_type == "卖出下单")
                        {
                            ZT_SaleListView = elementlist[0];
                        }

                        ZT_DataList = uielement.GetViewList(elementlist[0], 19);

                    }
                }
            }
        }



        /// <summary>
        /// 中投证券买入or点击
        /// </summary>
        /// <param name="_type">"买入"or"卖出"</param>
        static void GetZT_OrderButtonElement(string _type)
        {
            var uielement = new iAutomationElement();
            var elementlist = uielement.enumRoot();
            elementlist = uielement.FindByName("中投证券", elementlist);
            elementlist = uielement.enumNode(elementlist[0]);
            if (elementlist.Count > 1)
            {
                foreach (AutomationElement item in elementlist)
                {
                    var list = uielement.enumDescendants(item, "锁定");
                    if (list.Count > 0)
                    {
                        buyWindowsElement = TreeWalker.RawViewWalker.GetParent(list[0]);
                        elementlist = uielement.enumNode(buyWindowsElement);
                        elementlist = uielement.FindByName(_type, elementlist);
                        if (_type == "买入")
                        {
                            ZT_BuyButtonElement = elementlist[0];
                        }
                        else if (_type == "卖出")
                        {
                            ZT_SaleButtonElement = elementlist[0];
                        }

                        //uielement.InvokeButton(elementlist[0]);
                    }
                }
            }
        }

        /// <summary>
        /// 获取NumBox的UIElement
        /// </summary>
        /// <param name="_type">"买入下单" or "卖出下单"</param>
        static void GetNumboxElement(string _type)
        {
            var uielement = new iAutomationElement();
            var elementlist = uielement.enumRoot();
            elementlist = uielement.FindByName("中投证券", elementlist);
            elementlist = uielement.enumNode(elementlist[0]);
            if (elementlist.Count > 1)
            {
                foreach (AutomationElement item in elementlist)
                {
                    var list = uielement.enumDescendants(item, _type);
                    if (list.Count > 0)
                    {
                        buyWindowsElement = TreeWalker.RawViewWalker.GetParent(list[0]);
                        elementlist = uielement.enumNode(buyWindowsElement);
                        elementlist = uielement.FindByClassName("Edit", elementlist);
                        foreach (var i in elementlist)
                        {
                            if (i.Current.Name.ToString() == "" || i.Current.Name.ToString() == string.Empty)
                            {
                                if (_type == "买入下单")
                                {
                                    ZT_BuyNum = i;
                                }
                                else if (_type == "卖出下单")
                                {
                                    ZT_SaleNum = i;
                                }
                            }
                        }
                    }
                }
            }
        }

      

        /// <summary>
        /// 获取当前可用资金
        /// </summary>
        static void GetCanUseMoney()
        {
            var uielement = new iAutomationElement();
            var elementlist = uielement.enumRoot();
            elementlist = uielement.FindByName("中投证券", elementlist);
            elementlist = uielement.enumNode(elementlist[0]);
            if (elementlist.Count > 1)
            {
                foreach (AutomationElement item in elementlist)
                {
                    var list = uielement.enumDescendants(item, "买入下单");
                    if (list.Count > 0)
                    {
                        buyWindowsElement = TreeWalker.RawViewWalker.GetParent(list[0]);
                        elementlist = uielement.enumNode(buyWindowsElement);
                        elementlist = uielement.FindByClassName("Static", elementlist);
                        ZT_CanUseMoney = elementlist[7];
                        Console.WriteLine(ZT_CanUseMoney.Current.Name);
                    }
                }
            }
        }

        /// <summary>
        /// 获取证券代码TextBoxUIElement
        /// </summary>
        /// <param name="_type">"买入下单" or "卖出下单"</param>
        static void GetZTSecodeElement(string _type)
        {
            var uielement = new iAutomationElement();
            var elementlist = uielement.enumRoot();
            elementlist = uielement.FindByName("中投证券", elementlist);
            elementlist = uielement.enumNode(elementlist[0]);
            if (elementlist.Count > 1)
            {
                foreach (AutomationElement item in elementlist)
                {
                    var list = uielement.enumDescendants(item, _type);
                    if (list.Count > 0)
                    {
                        buyWindowsElement = TreeWalker.RawViewWalker.GetParent(list[0]);
                        elementlist = uielement.enumNode(buyWindowsElement);
                        elementlist = uielement.FindByClassName("AfxWnd42", elementlist);
                        if (_type == "买入下单")
                        {
                            ZT_BuySecuritiesCode = elementlist[0];
                        }
                        else if (_type == "卖出下单")
                        {
                            ZT_SaleSecuritiesCode = elementlist[0];
                        }
                        //uielement.WriteTextBox(elementlist[0], "\b\b\b\b\b\b");
                        //uielement.WriteTextBox(elementlist[0], "000005");
                    }
                }
            }

        }

        /// <summary>
        /// 获取下单按钮的UIELement
        /// </summary>
        /// <param name="_type">"买入下单" or "卖出下单"</param>
        static void GetZTOrder(string _type)
        {
            var uielement = new iAutomationElement();
            var elementlist = uielement.enumRoot();
            elementlist = uielement.FindByName("中投证券", elementlist);
            elementlist = uielement.enumNode(elementlist[0]);
            if (elementlist.Count > 1)
            {
                foreach (AutomationElement item in elementlist)
                {
                    var list = uielement.enumDescendants(item, _type);
                    if (list.Count > 0)
                    {

                        if (_type == "买入下单")
                        {
                            ZT_BuyOrder = list[0];
                        }
                        else if (_type == "卖出下单")
                        {
                            ZT_SaleOrder = list[0];
                        }
                        //uielement.WriteTextBox(elementlist[0], "\b\b\b\b\b\b");
                        //uielement.WriteTextBox(elementlist[0], "000005");
                    }
                }
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {

            start();
        }

        void start()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            Start.Enabled = false;
            isRun = true;
            bdlist = new BindingList<WarmingData>();
            DZH_DataList.Clear();
            //var data = new iAutomationElement();
            //DZH_DataList = data.GetViewList(DZH_uiElement, 5);
            //foreach (var i in DZH_DataList)
            //{
            //    var item = new WarmingData();
            //    item.code = i.Value.data[0];
            //    item.condition = i.Value.data[1];
            //    item.time = i.Value.data[2];
            //    item.price = i.Value.data[3];
            //    item.nowprice = i.Value.data[4];
            //    item.flag = "";
            //    bdlist.Add(item);

            //}
            //dataGridView1.DataSource = bdlist;
            UpdatePostion();

            Thread orderThread = new Thread(AutoOrder);
            orderThread.IsBackground = true;
            orderThread.Start();
            //Thread updateT = new Thread(updateWarming);
            //updateT.IsBackground = true;
            //updateT.Start();
        }
        /// <summary>
        /// 保存配置文件
        /// </summary>
        static public  void SaveInifile()
        {
            string textA = "";
            string textB = "";
            string textC = "";
            textA = A_param[0].ToString() + "|" + A_param[1].ToString() + "|" + A_param[2].ToString() + "|" + A_param[3].ToString() + "|" + A_param[4].ToString();
            textB = B_param[0].ToString() + "|" + B_param[1].ToString() + "|" + B_param[2].ToString();
            textC = C_param[0].ToString() + "|" + C_param[1].ToString() + "|" + C_param[2].ToString() + "|" + C_param[3].ToString();
            IniFunc.WriteString("Param", "A", textA, Application.StartupPath + "\\config.ini");
            IniFunc.WriteString("Param", "B", textB, Application.StartupPath + "\\config.ini");
            IniFunc.WriteString("Param", "C", textC, Application.StartupPath + "\\config.ini");
        }
        
        /// <summary>
        /// 自动下单
        /// </summary>
        void AutoOrder()
        {
            int count = 0;
            while (isRun)
            {
                 PositionDetection();
                 if (count < bdlist.Count && warmingRun)
                 {
                    var data = new WarmingData();
                    data.code = bdlist[count].code;
                    data.condition = bdlist[count].condition;
                    var code = IsNum(data.code);

                    if (data.condition.Contains("买入") && bdlist[count].flag == "")
                    {
                        BuyOrder(code);
                        bdlist[count].flag = "1";
                    }
                    else if (data.condition.Contains("卖出") && bdlist[count].flag == "")
                    {
                        SaleOrder(code);
                        bdlist[count].flag = "1";
                    }
                    else
                    {

                    }
                    count++;
                 }
                else
                {
                    count = 0;
                }
            }
        }
        /// <summary>
        /// 更新预警列表
        /// </summary>
        void updateWarming()
        {
            while (isRun)
            {
                BindingList<WarmingData> listA = new BindingList<WarmingData>();
                var data = new iAutomationElement();
                var list = data.GetViewList(DZH_uiElement, 5);
                foreach (var i in list)
                {
                    var item = new WarmingData();
                    item.code = i.Value.data[0];
                    item.condition = i.Value.data[1];
                    item.time = i.Value.data[2];
                    item.price = i.Value.data[3];
                    item.nowprice = i.Value.data[4];
                    item.flag = "";
                    listA.Add(item);
                    //var code = IsNum(i.Value.data[0]);
                    //BuyOrder(code);
                }

                var query = listA.Where(p =>
                {
                    if ((!bdlist.Any(s => s.code == p.code)) && bdlist.Any(s => s.condition == p.condition)) return true;
                    return false;
                });

                foreach (var item in query)
                {
                    bdlist.Add(item);
                }
                Thread.Sleep(5000);
            }
            

        }
        /// <summary>
        /// 获取持仓按钮
        /// </summary>
        static void GetZT_PositionOrderButtonElement()
        {
            var uielement = new iAutomationElement();
            var elementlist = uielement.enumRoot();
            elementlist = uielement.FindByName("中投证券", elementlist);
            elementlist = uielement.enumNode(elementlist[0]);
            if (elementlist.Count > 1)
            {
                foreach (AutomationElement item in elementlist)
                {
                    var list = uielement.enumDescendants(item, "锁定");
                    if (list.Count > 0)
                    {
                        buyWindowsElement = TreeWalker.RawViewWalker.GetParent(list[0]);
                        elementlist = uielement.enumNode(buyWindowsElement);
                        elementlist = uielement.FindByName("持仓", elementlist);

                        ZT_PositionOrder = elementlist[0];
                        //uielement.InvokeButton(elementlist[0]);
                    }
                }
            }
        }
        /// <summary>
        /// 获取输出AutomationElement
        /// </summary>
        static void GetZT_OutPutElement()
        {
            var uielement = new iAutomationElement();
            var elementlist = uielement.enumRoot();
            elementlist = uielement.FindByName("中投证券", elementlist);
            elementlist = uielement.enumNode(elementlist[0]);
            if (elementlist.Count > 1)
            {
                foreach (AutomationElement item in elementlist)
                {

                    var list = uielement.enumDescendants(item, "修改成本");
                    if (list.Count > 0)
                    {
                        buyWindowsElement = TreeWalker.RawViewWalker.GetParent(list[0]);
                        elementlist = uielement.enumNode(buyWindowsElement);
                        elementlist = uielement.FindByName("输 出", elementlist);
                        ZT_Output = elementlist[0];
                        //uielement.WriteTextBox(elementlist[0], "\b\b\b\b\b\b");
                        //uielement.WriteTextBox(elementlist[0], "000005");
                    }
                }
            }
        }
        /// <summary>
        /// 获取
        /// </summary>
        static void GetZT_OutPutSuessElement()
        {
            var uielement = new iAutomationElement();
            var elementlist = uielement.enumRoot();
            elementlist = uielement.FindByName("中投证券", elementlist);
            elementlist = uielement.enumNode(elementlist[0]);
            if (elementlist.Count > 1)
            {
                foreach (AutomationElement item in elementlist)
                {

                    var list = uielement.enumDescendants(item, "输出");
                    if (list.Count > 0)
                    {
                        buyWindowsElement = TreeWalker.RawViewWalker.GetParent(list[0]);
                        elementlist = uielement.enumNode(buyWindowsElement);
                        elementlist = uielement.FindByName("确  定", elementlist);
                        ZT_OutputSuess = elementlist[0];
                       
                    }
                }
            }
        }
        
        /// <summary>
        /// 更新持仓单信息
        /// </summary>
        void UpdatePostion()
        {
            
                var click = new iAutomationElement();
                if (ZT_PositionOrder != null)
                {
                    click.InvokeButton(ZT_PositionOrder);
                    GetZT_OutPutElement();
                    if (ZT_Output != null)
                    {
                        click.InvokeButton(ZT_Output);
                        GetZT_OutPutSuessElement();
                        if (ZT_OutputSuess!=null)
                        {
                        click.InvokeButton(ZT_OutputSuess);
                        Thread.Sleep(500);
                        var uielement = new iAutomationElement();
                        var elementlist = uielement.enumRoot();
                        elementlist = uielement.FindByClassName("Notepad", elementlist);

                        uielement.CloseTextBook(elementlist[0]);
                        var list = ReadText.Read();
                        foreach (var item in list)
                        {
                            string[] array = item.Split(new char[]
                           {
                            '|'
                           });
                            var data = new DataItem();
                            data.data = array;
                            if (!ZT_DataList.ContainsKey(array[0]))
                            {
                                ZT_DataList.Add(array[0], data);
                            }
                            else
                            {
                                //若标记为空更新全部数据
                                if (ZT_DataList[array[0]].data[18] == "")
                                {
                                    ZT_DataList[array[0]] = data;
                                }                       
                                else
                                {
                                    data.data[18] = "1";
                                    ZT_DataList[array[0]] = data;
                                }

                              
                            }
                                    
                                
                            
                        }
                       

                        }
                    }
               
                }
               
               
                
            
        }

        /// <summary>
        /// 持仓检测
        /// </summary>
        void PositionDetection()
        {
            if (postionRun && isRun)
            {
                UpdatePostion();
                foreach (var item in ZT_DataList)
                {

                    if(postionRun && isRun && item.Value.data[18]=="")
                    {

                        var code = item.Key;                     
                        
                        var orderClick = new iAutomationElement();
                        var datalist = getSotckData(code);
                        var positiondata = ZT_DataList[code];
                        var _num = NumCalculation.GetSaleNum(positiondata.data, datalist, A_param, B_param, C_param);
                        _num = (Convert.ToInt32(_num) / 100 * 100).ToString();
                        if (_num != "0")
                        {
                            //数量不为0 数据标记 下次持仓检测不做
                            ZT_DataList[code].data[18] = "1";
                            orderClick.InvokeButton(ZT_SaleButtonElement);
                            Thread.Sleep(500);
                            orderClick.WriteTextBox(ZT_SaleSecuritiesCode, "\b\b\b\b\b\b");
                            orderClick.WriteTextBox(ZT_SaleSecuritiesCode, code);
                            Thread.Sleep(500);
                            orderClick.WriteTextBox(ZT_SaleNum, "\b\b\b\b\b\b");
                            orderClick.WriteTextBox(ZT_SaleNum, _num);
                            Thread.Sleep(500);
                            orderClick.InvokeButton(ZT_SaleOrder);
                            GetConfirm("卖出确认");
                            if (ZT_SaleConfirm != null)
                            {
                                orderClick.InvokeButton(ZT_SaleConfirm);
                                ZT_SaleConfirm = null;
                                Clickconfirm();
                            }

                        }
                    }
                    

                }
            }
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            button2.Enabled = true;
            Start.Enabled = true;
            isRun = false;
        }

        bool postionRun = false;

        bool warmingRun = false;

        private void button4_Click(object sender, EventArgs e)
        {
            if (!postionRun)
            {
                postionRun = true;
                button4.Text = "结束持仓检测";

            }
            else
            {
                postionRun = false;
                button4.Text = "持仓检测";
            }
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (!warmingRun)
            {
                warmingRun = true;
                button5.Text = "结束预警检测";

            }
            else
            {
                warmingRun = false;
                button5.Text = "预警检测";
            }
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F8 && postionRun == true)
            {
                postionRun = false;
                button4.Text = "持仓检测";
            }
            else if (e.KeyData == Keys.F8 && postionRun == false)
            {
                postionRun = true;
                button4.Text = "结束持仓检测";
            }
            if (e.KeyData == Keys.F9 && warmingRun == true)
            {
                warmingRun = false;
                button5.Text = "预警检测";
            }
            else if (e.KeyData == Keys.F9 && warmingRun == false)
            {
                warmingRun = true;
                button5.Text = "结束预警检测";
            }
            if (e.KeyData == Keys.F7 && isRun == true)
            {
                button1.Enabled = true;
                button2.Enabled = true;
                Start.Enabled = true;
                isRun = false;

            }
            else if (e.KeyData == Keys.F7 && isRun == false)
            {

                button1.Enabled = false;
                button2.Enabled = false;
                Start.Enabled = false;
                isRun = true;
                start();
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F8 && postionRun == true)
            {
                postionRun = false;
                button4.Text = "持仓检测";
            }
            else if (e.KeyData == Keys.F8 && postionRun == false)
            {
                postionRun = true;
                button4.Text = "结束持仓检测";
            }
            if (e.KeyData == Keys.F9&& warmingRun == true)
            {
                warmingRun = false;
                button5.Text = "预警检测";
            }
            else if (e.KeyData == Keys.F9 && warmingRun == false)
            {
                warmingRun = true;
                button5.Text = "结束预警检测";
            }
            if (e.KeyData == Keys.F7 && isRun == true)
            {
                button1.Enabled = true;
                button2.Enabled = true;
                Start.Enabled = true;
                isRun = false;

            }
            else if(e.KeyData == Keys.F7 && isRun == false)
            {
               
                button1.Enabled = false;
                button2.Enabled = false;
                Start.Enabled = false;
                isRun = true;
                start();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            k_hook.Stop();
        }
    }
}
