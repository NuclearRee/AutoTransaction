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

        //中投证券持仓单数据
        static Dictionary<string, DataItem> ZT_DataList = new Dictionary<string, DataItem>();

        static AutomationElement buyWindowsElement;
        public MainForm()
        {
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
        public void output(string _content)
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
            var data = post.doPost("http://hq.sinajs.cn/list=sz" + _stockCode, list);
            data = data.Substring(data.IndexOf("\""));
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
            orderClick.WriteTextBox(ZT_BuyNum, "\b\b\b\b\b\b");
            orderClick.WriteTextBox(ZT_BuyNum, _num.ToString());
            Thread.Sleep(500);
            orderClick.InvokeButton(ZT_BuyOrder);
            if (ZT_BuyConfirm == null)
                GetConfirm("买入确认");
        }
        /// <summary>
        /// 股票卖出
        /// </summary>
        /// <param name="_securitiesCode">证券代码</param>
        /// <param name="_num">数量</param>
        static void SaleOrder(string _securitiesCode, string _num)
        {
            var orderClick = new iAutomationElement();
            orderClick.InvokeButton(ZT_SaleButtonElement);
            Thread.Sleep(500);
            orderClick.WriteTextBox(ZT_SaleSecuritiesCode, "\b\b\b\b\b\b");
            orderClick.WriteTextBox(ZT_SaleSecuritiesCode, _securitiesCode);
            Thread.Sleep(500);
            orderClick.WriteTextBox(ZT_SaleNum, "\b\b\b\b\b\b");
            orderClick.WriteTextBox(ZT_SaleNum, _num);
            Thread.Sleep(500);
            orderClick.InvokeButton(ZT_SaleOrder);
            if (ZT_SaleConfirm == null)
                GetConfirm("卖出确认");
        }

        /// <summary>
        /// 基础数据初始化
        /// </summary>
        static void initialization()
        {
            //读取需要的句柄以及UIElement
            //1.读取预警列表UIElement     DZH_uiElement  
            GetReadWaringListViewElement();
            //2.买入按钮UIElement
            GetZT_OrderButtonElement("买入");
            //3.卖出按钮UIElement
            GetZT_OrderButtonElement("卖出");
            //4.获取买入界面UIElement
            var click = new iAutomationElement();
            //点击买入按钮
            click.InvokeButton(ZT_BuyButtonElement);
            //获取持仓单UIElement
            GetZTViewListElement("买入下单");
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
            //5.获取卖出界面UIElement           
            //点击买入按钮
            click.InvokeButton(ZT_SaleButtonElement);
            //获取持仓单UIElement
            GetZTViewListElement("卖出下单");
            //获取买入界面证券代码TextBox UIElement
            GetZTSecodeElement("卖出下单");
            //输入证券代码
            click.WriteTextBox(ZT_SaleSecuritiesCode, "000001");
            //获取NumBoxUIElement
            GetNumboxElement("卖出下单");
            //获取ZT_SaleOrderUIElement
            GetZTOrder("卖出下单");
        }

        /// <summary>
        /// 获取确认交易按钮 UIElement
        /// </summary>
        /// <param name="_type">"买入确认" or "卖出确认"</param>
        static void GetConfirm(string _type)
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

        /// <summary>
        /// 大智慧预警表读取
        /// </summary>
        static void GetReadWaringListViewElement()
        {
            var uielement = new iAutomationElement();
            var elemlentlist = uielement.enumRoot();
            elemlentlist = uielement.FindByName("大智慧", elemlentlist);
            elemlentlist = uielement.enumNode(elemlentlist[0]);
            elemlentlist = uielement.FindByName("预警", elemlentlist);
            //foreach (AutomationElement item in elemlentlist)
            //{
            //    Console.WriteLine(item.Current.Name + "" + item.Current.ClassName);
            //}
            elemlentlist = uielement.enumNode(elemlentlist[0]);
            elemlentlist = uielement.FindByName("List2", elemlentlist);
            DZH_uiElement = elemlentlist[0];
            DZH_DataList = uielement.GetViewList(elemlentlist[0], 5);

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
                        //foreach (var count in elementlist)
                        //    Console.WriteLine(count.Current.ClassName + " " + count.Current.Name);
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

                        //uielement.WriteTextBox(elementlist[0], "\b\b\b\b\b\b");
                        //uielement.WriteTextBox(elementlist[0], "000005");
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

        }
    }
}
