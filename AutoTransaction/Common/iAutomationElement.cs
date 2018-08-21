using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using AutoTransaction.DataModel;

namespace AutoTransaction.Common
{
    public class iAutomationElement
    {
        //声明 API 函数
        public const uint LVM_FIRST = 0x1000;
        public const uint LVM_GETITEMCOUNT = LVM_FIRST + 4;
        public const uint LVM_GETITEMW = LVM_FIRST + 75;
        public const uint WM_CLOSE = 0x0010;
        [DllImport("user32.DLL")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        [DllImport("user32.DLL")]
        public static extern IntPtr FindWindow(string lpszClass, string lpszWindow);
        [DllImport("user32.DLL")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent,
            IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd,
            out uint dwProcessId);

        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess,
            bool bInheritHandle, uint dwProcessId);
        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RELEASE = 0x8000;

        public const uint MEM_RESERVE = 0x2000;
        public const uint PAGE_READWRITE = 4;

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

        public struct LVITEM
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public IntPtr pszText; // string 
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns;
            public IntPtr puColumns;
        }
        public static int LVIF_TEXT = 0x0001;


        //应用句柄
        AutomationElement exeHandle;
        //头节点
        public AutomationElement head { get; set; }
        //当前选择节点
        public AutomationElement node { get; set; }
       
        //windows UI树的根
        private AutomationElement root = AutomationElement.RootElement;
        //Root节点子级集合      
        List<AutomationElement> chridList = new List<AutomationElement>();
        //数据列表
        public Dictionary<string,DataItem> DataList = new Dictionary<string,DataItem>();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_name">UI标题</param>
        public iAutomationElement()
        {
            
        }

        /// <summary>
        /// 枚举root节点下所有结点，并存放到chridList
        /// </summary>
        public List<AutomationElement> enumRoot()
        {
            var elementList = root.FindAll(TreeScope.Children, PropertyCondition.TrueCondition);
            List<AutomationElement> list = new List<AutomationElement>();
            foreach (AutomationElement item in elementList)
            {
                if (item.Current.Name != null && item.Current.Name != "")
                {
                    list.Add(item);
                }
                              
            }
            return list;
        }

        /// <summary>
        /// 枚举节点下的所有UI
        /// </summary>
        /// <param name="_element">node节点</param>
        public List<AutomationElement> enumNode(AutomationElement _element)
        {
            var elementList = _element.FindAll(TreeScope.Children, PropertyCondition.TrueCondition);
            List<AutomationElement> list = new List<AutomationElement>();
            foreach (AutomationElement item in elementList)
            {
                list.Add(item);
            }
            return list;
          

        }
        /// <summary>
        /// 获取UI元素的父级
        /// </summary>
        /// <param name="_element"></param>
        /// <returns></returns>
        public AutomationElement getFather(AutomationElement _element)
        {
            var element = _element.FindAll(TreeScope.Parent, PropertyCondition.TrueCondition);
            return element[0];
        }

        /// <summary>
        /// 根据名称获取_element节点(包括子代)下集合
        /// </summary>
        /// <param name="_element">节点元素对象</param>
        /// <param name="_name">查找名称</param>
        /// <returns>获得的集合</returns>
        public List<AutomationElement> enumDescendants(AutomationElement _element,string _name)
        {
            var elementList = _element.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty,_name));
            List<AutomationElement> list = new List<AutomationElement>();
            foreach (AutomationElement item in elementList)
            {
                list.Add(item);
            }
            return list;
        }

        /// <summary>
        /// 在_childList集合中查找名称与_name相似的集合
        /// </summary>
        /// <param name="_name">UI节点元素名称</param>
        /// <param name="_srcChildList">UI节点集</param>
        /// <returns>返回相似的节点集合</returns>

        public List<AutomationElement> FindByName(string _name,List<AutomationElement> _srcChildList)
        {
            var v = _srcChildList.FindAll(a => a.Current.Name.Contains(_name));
            if(v.Count()!=0)
                return v as List<AutomationElement>;
            return null;
            
        }

        /// <summary>
        /// 在_childList集合中查找名称与_classname相似的集合
        /// </summary>
        /// <param name="_classname">UI节点元素名称</param>
        /// <param name="_srcChildList">UI节点集</param>
        /// <returns></returns>
        public List<AutomationElement> FindByClassName(string _classname, List<AutomationElement> _srcChildList)
        {

            var v = _srcChildList.FindAll(a => a.Current.ClassName.Contains(_classname));
            if (v.Count() != 0)
                return v as List<AutomationElement> ;
            return null;

        }

        

        /// <summary>
        /// 在root下通过UI元素名,类名搜素，返回一个AutomationElement对象
        /// </summary>
        /// <param name="_name">UI元素名</param>
        /// <param name="_className">类名</param>
        /// <returns></returns>
        public AutomationElement SearchWindowsByName(string _name, string _className)
        {
            try
            {
                var exeUiElement = root.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NativeWindowHandleProperty, exeHandle));
                //var exeUiElement = root.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.NameProperty, "大智慧"));
                if (exeUiElement != null)
                {
                    var conditions = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, _name),
                    new PropertyCondition(AutomationElement.ClassNameProperty, _className));
                    var chridUiElement = exeUiElement.FindFirst(TreeScope.Subtree, conditions);
                    if (chridUiElement != null)
                    {
                        return chridUiElement;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
           
            
        }


        /// <summary>
        /// 获取ViewList中的内容
        /// </summary>
        /// <param name="_element">AutomationElement对象</param>
        public Dictionary<string, DataItem> GetViewList(AutomationElement _element,int _column)
        {
            //获取list表里的行数
            int count = (int)SendMessage((IntPtr)_element.Current.NativeWindowHandle, LVM_GETITEMCOUNT, 0, 0);

            
            uint vProcessId;
            //根据句柄获取进程号
            GetWindowThreadProcessId((IntPtr)_element.Current.NativeWindowHandle, out vProcessId);

            IntPtr vProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ |
                PROCESS_VM_WRITE, false, vProcessId);
            IntPtr vPointer = VirtualAllocEx(vProcess, IntPtr.Zero, 4096,
                MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);

            for (int i = 0; i < count; i++)
            {
                DataItem item = new DataItem();
                item.data = new string[_column];
                for (int j = 0; j < _column; j++)
                {
                    byte[] vBuffer = new byte[256];
                    LVITEM[] vItem = new LVITEM[1];
                    vItem[0].mask = LVIF_TEXT;
                    vItem[0].iItem = i;
                    vItem[0].iSubItem = j;
                    vItem[0].cchTextMax = vBuffer.Length;
                    vItem[0].pszText = (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(LVITEM)));
                    uint vNumberOfBytesRead = 0;
                    WriteProcessMemory(vProcess, vPointer,
                        Marshal.UnsafeAddrOfPinnedArrayElement(vItem, 0),
                        Marshal.SizeOf(typeof(LVITEM)), ref vNumberOfBytesRead);
                    SendMessage((IntPtr)_element.Current.NativeWindowHandle, LVM_GETITEMW, i, vPointer.ToInt32());
                    ReadProcessMemory(vProcess,
                        (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(LVITEM))),
                        Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0),
                        vBuffer.Length, ref vNumberOfBytesRead);

                    string vText = Marshal.PtrToStringUni(
                        Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0));
                    item.data[j] = vText;
                }
                if (!DataList.Keys.Contains(item.data[0]))
                {
                    DataList.Add(item.data[0],item);
                }
                else
                {
                    DataList[item.data[0]] = item; 
                }
                
            }
            return DataList;
        }


        /// <summary>
        /// 模拟点击
        /// </summary>
        /// <param name="e"></param>
        public  void InvokeButton(AutomationElement _element)
        {
            InvokePattern invoke = (InvokePattern)_element.GetCurrentPattern(InvokePattern.Pattern);
            invoke.Invoke();
        }
        /// <summary>
        /// 模拟写入
        /// </summary>
        /// <param name="_element"></param>
        /// <param name="_text"></param>
        public void WriteTextBox(AutomationElement _element,string _text)
        {
            byte[] arry = System.Text.Encoding.ASCII.GetBytes(_text);
            foreach(var item in arry) 
                SendMessage((IntPtr)_element.Current.NativeWindowHandle, 0x0102, item, 0);
        }

        public void CloseTextBook(AutomationElement _element)
        {
            SendMessage((IntPtr)_element.Current.NativeWindowHandle, WM_CLOSE, 0, 0);
        }

    }
        
       

    
}
