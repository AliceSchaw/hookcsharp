
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Windows.Input;

namespace hookcs
{
    class hook
    {

       #region 第一步:声明API函数
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        [DllImport("kernel32.dll")]
        static extern int GetCurrentThreadId();
        #endregion
        #region 第二步：声明，定义委托
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        static int hKeyboardHook = 0;//如果hKeyboardHook不为0则说明钩子安装成功
        #endregion

        #region 第三步：编写钩子子程
        //钩子子程就是钩子所要做的事情。


        public class Win32Api
        {
            #region 常数和结构
            public const int WM_KEYDOWN = 0x100;
            public const int WM_KEYUP = 0x101;
            public const int WM_SYSKEYDOWN = 0x104;
            public const int WM_SYSKEYUP = 0x105;
            public const int WH_KEYBOARD_LL = 13;

            [StructLayout(LayoutKind.Sequential)] //声明键盘钩子的封送结构类型 
            public class KeyboardHookStruct
            {
                public int vkCode; //表示一个在1到254间的虚似键盘码 
                public int scanCode; //表示硬件扫描码 
                public int flags;
                public int time;
                public int dwExtraInfo;
            }
            #endregion
        }
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
            {
            Win32Api.KeyboardHookStruct msg = (Win32Api.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.KeyboardHookStruct));
                if (nCode >= 0)
                {
                    if (Win32Api.WM_KEYDOWN == wParam)
                    {
                         if (65 == msg.vkCode)
                        {
                            return 1;
                        }
                    }
                    else
                        return 0;
                }
                return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);

            }
        
        #endregion
        // 安装钩子 
        public bool Start()
        {
            if (hKeyboardHook == 0)
            {
                //WH_KEYBOARD_LL = 13 
                hKeyboardHook = SetWindowsHookEx(13, new HookProc(KeyboardHookProc), Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            }
            return (hKeyboardHook != 0);
        }
        // 卸载钩子 
        public bool Stop()
        {
            if (hKeyboardHook != 0)
            {
                return UnhookWindowsHookEx(hKeyboardHook);
            }
            return true;
        }
    }
}