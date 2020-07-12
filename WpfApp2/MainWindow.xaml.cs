using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApp2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Messenger.Default.Register<Key>(this, "KeyDownButton", KeyDownButton);
        }

        private void KeyDownButton(Key obj)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                tb1.Text += obj;
            }));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //注册快捷键
            RegisterHotKey();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //释放快捷键
            UnregisterHotKey();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region  快捷键相关
            wpfHwnd = new WindowInteropHelper(this).Handle;

            var hWndSource = HwndSource.FromHwnd(wpfHwnd);
            //添加处理程序
            if (hWndSource != null) hWndSource.AddHook(MainWindowProc);
            hotKeyDic.Add("Q", Win32.GlobalAddAtom("Q"));
            hotKeyDic.Add("P", Win32.GlobalAddAtom("P"));
            #endregion
        }

        #region 快捷键相关

        IntPtr wpfHwnd;
        /// <summary>
        /// 注册快捷集合
        /// </summary>
        readonly Dictionary<string, short> hotKeyDic = new Dictionary<string, short>();
        #region 取消注册热键
        /// <summary>
        /// 取消注册热键
        /// </summary>
        /// <param name="obj"></param>
        private void UnregisterHotKey()
        {
            Win32.UnregisterHotKey(wpfHwnd, hotKeyDic["Q"]);
            Win32.UnregisterHotKey(wpfHwnd, hotKeyDic["P"]);
        }
        #endregion

        #region 注册热键
        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="obj"></param>
        private void RegisterHotKey()
        {
            Win32.RegisterHotKey(wpfHwnd, hotKeyDic["Q"], Win32.KeyModifiers.None, (int)System.Windows.Forms.Keys.Q);
            Win32.RegisterHotKey(wpfHwnd, hotKeyDic["P"], Win32.KeyModifiers.None, (int)System.Windows.Forms.Keys.P);
        }
        #endregion

        #region 响应快捷键事件
        /// <summary>
        /// 响应快捷键事件
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        private IntPtr MainWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case Win32.WmHotkey:
                    {
                        int sid = wParam.ToInt32();
                        if (sid == hotKeyDic["Q"])
                        {
                            Messenger.Default.Send<Key>(Key.Q, "KeyDownButton");
                        }
                        else if (sid == hotKeyDic["P"])
                        {
                            Messenger.Default.Send<Key>(Key.P, "KeyDownButton");
                        }
                        handled = true;
                        break;
                    }
            }
            return IntPtr.Zero;
        }
        #endregion

        #endregion
    }
}
