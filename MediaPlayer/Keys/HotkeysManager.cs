using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MediaPlayer.Keys
{
    public class HotkeysManager
    {
        public delegate void HotkeyEvent(GlobalHotkey hotkey);

        public static event HotkeyEvent? HotkeyFired;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelKeyboardProc LowLevelProc = HookCallback;

        private static List<GlobalHotkey> Hotkeys { get; set; }

        private const int WH_KEYBOARD_LL = 13;

        private static IntPtr HookID = IntPtr.Zero;

        public static bool IsHookSetup { get; private set; }


        static HotkeysManager()
        {
            Hotkeys = new List<GlobalHotkey>();
        }

        public static void SetupSystemHook()
        {
            HookID = SetHook(LowLevelProc);
            IsHookSetup = true;
        }

        public static void ShutdownSystemHook()
        {
            UnhookWindowsHookEx(HookID);
            IsHookSetup = false;
        }

  
        public static void AddHotkey(GlobalHotkey hotkey)
        {
            Hotkeys.Add(hotkey);
        }

        public static void RemoveHotkey(GlobalHotkey hotkey)
        {
            Hotkeys.Remove(hotkey);
        }

        private static void CheckHotkeys()
        {
            foreach (GlobalHotkey hotkey in Hotkeys)
            {
                if (Keyboard.Modifiers == hotkey.modifierKeys && Keyboard.IsKeyDown(hotkey.Key))
                {
                    if (hotkey.canExecute)
                    {
                        hotkey.Callback.Invoke();
                        HotkeyFired?.Invoke(hotkey);
                    }
                }
            }
        }
        public static List<GlobalHotkey> FindHotkeys(ModifierKeys modifier, Key key)
        {
            List<GlobalHotkey> hotkeys = new List<GlobalHotkey>();
            foreach (GlobalHotkey hotkey in Hotkeys)
            {
                if (hotkey.Key == key && hotkey.modifierKeys == modifier)
                {
                    hotkeys.Add(hotkey);
                }
            }
            return hotkeys;
        }

        public static void AddHotkey(ModifierKeys modifier, Key key, Action callbackMethod, bool canExecute = true)
        {
            AddHotkey(new GlobalHotkey(modifier, key, callbackMethod, canExecute));
        }

        public static void RemoveHotkey(ModifierKeys modifier, Key key, bool removeAllOccourances = false)
        {
            List<GlobalHotkey> originalHotkeys = Hotkeys;
            List<GlobalHotkey> toBeRemoved = FindHotkeys(modifier, key);

            if (toBeRemoved.Count > 0)
            {
                if (removeAllOccourances)
                {
                    foreach (GlobalHotkey hotkey in toBeRemoved)
                    {
                        originalHotkeys.Remove(hotkey);
                    }

                    Hotkeys = originalHotkeys;
                }
                else
                {
                    RemoveHotkey(toBeRemoved[0]);
                }
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            {
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                CheckHotkeys();
            }

            return CallNextHookEx(HookID, nCode, wParam, lParam);
        }

        #region Native Methods

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion
    }
}
