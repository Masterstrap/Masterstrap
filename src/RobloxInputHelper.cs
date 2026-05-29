using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Masterstrap.Services
{
    internal static class RobloxInputHelper
    {
        private const int InputKeyboard = 1;
        private const uint KeyeventfKeyup = 0x0002;
        private const uint KeyeventfScancode = 0x0008;
        private const byte VkControl = 0x11;
        private const byte VkLControl = 0xA2;
        private const byte VkRControl = 0xA3;
        private const byte VkShift = 0x10;
        private const byte VkLShift = 0xA0;
        private const byte VkRShift = 0xA1;
        private const byte VkN = 0x4E;
        private const uint WmKeydown = 0x0100;
        private const uint WmKeyup = 0x0101;

        [StructLayout(LayoutKind.Sequential)]
        private struct Input
        {
            public uint type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct InputUnion
        {
            [FieldOffset(0)] public KeyboardInput ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        public static bool TrySendPlayerNamesToggle(int robloxPid)
        {
            if (robloxPid <= 0)
                return false;

            IntPtr hwnd = FindRobloxGameWindow((uint)robloxPid);
            if (hwnd == IntPtr.Zero)
            {
                RuntimeDiagLog.Append("[PlayerNames] Input: Roblox window (class Roblox) not found.");
                return false;
            }

            try
            {
                ReleaseAllModifiers();
                Thread.Sleep(60);

                FocusRobloxWindow(hwnd);
                Thread.Sleep(150);

                bool sendInputOk = TrySendChordSendInput();
                RuntimeDiagLog.Append($"[PlayerNames] SendInput Ctrl+Shift+N: {sendInputOk} hwnd=0x{hwnd.ToInt64():X}");

                if (!sendInputOk)
                {
                    Thread.Sleep(80);
                    FocusRobloxWindow(hwnd);
                    Thread.Sleep(120);
                    sendInputOk = TrySendChordSendInput();
                    RuntimeDiagLog.Append($"[PlayerNames] SendInput retry: {sendInputOk}");
                }

                if (sendInputOk)
                    return true;

                bool postOk = TrySendChordPostMessage(hwnd);
                RuntimeDiagLog.Append($"[PlayerNames] PostMessage Ctrl+Shift+N: {postOk}");
                if (postOk)
                    return true;

                RuntimeDiagLog.Append("[PlayerNames] Input: SendInput and PostMessage failed.");
                return false;
            }
            catch (Exception ex)
            {
                RuntimeDiagLog.Append("[PlayerNames] Input failed: " + ex.Message);
                return false;
            }
        }

        public static int GetRobloxPlayerPid()
        {
            try
            {
                var procs = Process.GetProcessesByName("RobloxPlayerBeta");
                return procs.Length > 0 ? procs[0].Id : 0;
            }
            catch
            {
                return 0;
            }
        }

        private static void ReleaseAllModifiers()
        {
            try
            {
                var releases = new Input[6];
                int idx = 0;
                releases[idx++] = KeyUpVk(VkControl);
                releases[idx++] = KeyUpVk(VkLControl);
                releases[idx++] = KeyUpVk(VkRControl);
                releases[idx++] = KeyUpVk(VkShift);
                releases[idx++] = KeyUpVk(VkLShift);
                releases[idx++] = KeyUpVk(VkRShift);
                SendInput((uint)idx, releases, Marshal.SizeOf<Input>());
            }
            catch (Exception ex)
            {
                RuntimeDiagLog.Append("[PlayerNames] ReleaseAllModifiers: " + ex.Message);
            }
        }

        private static Input KeyUpVk(byte vk)
        {
            return new Input
            {
                type = InputKeyboard,
                u = new InputUnion
                {
                    ki = new KeyboardInput
                    {
                        wVk = vk,
                        wScan = (ushort)MapVirtualKey(vk, 0),
                        dwFlags = KeyeventfKeyup
                    }
                }
            };
        }

        private static IntPtr FindRobloxGameWindow(uint processId)
        {
            IntPtr robloxClass = IntPtr.Zero;
            IntPtr anyVisible = IntPtr.Zero;

            EnumWindows((hWnd, _) =>
            {
                GetWindowThreadProcessId(hWnd, out uint pid);
                if (pid != processId || !IsWindowVisible(hWnd))
                    return true;

                if (anyVisible == IntPtr.Zero)
                    anyVisible = hWnd;

                var sb = new StringBuilder(64);
                if (GetClassName(hWnd, sb, sb.Capacity) > 0 &&
                    sb.ToString().Equals("Roblox", StringComparison.OrdinalIgnoreCase))
                {
                    robloxClass = hWnd;
                    return false;
                }

                return true;
            }, IntPtr.Zero);

            return robloxClass != IntPtr.Zero ? robloxClass : anyVisible;
        }

        private static void FocusRobloxWindow(IntPtr hwnd)
        {
            IntPtr fg = GetForegroundWindow();
            GetWindowThreadProcessId(fg, out uint fgThread);
            GetWindowThreadProcessId(hwnd, out uint targetThread);

            bool attached = false;
            if (fgThread != 0 && targetThread != 0 && fgThread != targetThread)
                attached = AttachThreadInput(fgThread, targetThread, true);

            try
            {
                SetForegroundWindow(hwnd);
            }
            finally
            {
                if (attached)
                    AttachThreadInput(fgThread, targetThread, false);
            }
        }

        private static bool TrySendChordSendInput()
        {
            try
            {
                SendChordScancode(VkControl, VkShift, VkN);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool TrySendChordPostMessage(IntPtr hwnd)
        {
            try
            {
                PostKeyMsg(hwnd, VkControl, false);
                PostKeyMsg(hwnd, VkShift, false);
                PostKeyMsg(hwnd, VkN, false);
                PostKeyMsg(hwnd, VkN, true);
                PostKeyMsg(hwnd, VkShift, true);
                PostKeyMsg(hwnd, VkControl, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void PostKeyMsg(IntPtr hwnd, byte vk, bool keyUp)
        {
            uint scan = MapVirtualKey(vk, 0);
            uint lParam = 1u | (scan << 16);
            if (keyUp)
                lParam |= 0xC0000000u;

            uint msg = keyUp ? WmKeyup : WmKeydown;
            PostMessage(hwnd, msg, (IntPtr)vk, (IntPtr)lParam);
            Thread.Sleep(15);
        }

        private static void SendChordScancode(byte mod1, byte mod2, byte key)
        {
            var inputs = new Input[6];
            int i = 0;
            inputs[i++] = KeyDownScancode(mod1);
            inputs[i++] = KeyDownScancode(mod2);
            inputs[i++] = KeyDownScancode(key);
            inputs[i++] = KeyUpScancode(key);
            inputs[i++] = KeyUpScancode(mod2);
            inputs[i++] = KeyUpScancode(mod1);
            SendInput((uint)i, inputs, Marshal.SizeOf<Input>());
        }

        private static Input KeyDownScancode(byte vk)
        {
            ushort scan = (ushort)MapVirtualKey(vk, 0);
            return new Input
            {
                type = InputKeyboard,
                u = new InputUnion
                {
                    ki = new KeyboardInput
                    {
                        wVk = 0,
                        wScan = scan,
                        dwFlags = KeyeventfScancode
                    }
                }
            };
        }

        private static Input KeyUpScancode(byte vk)
        {
            ushort scan = (ushort)MapVirtualKey(vk, 0);
            return new Input
            {
                type = InputKeyboard,
                u = new InputUnion
                {
                    ki = new KeyboardInput
                    {
                        wVk = 0,
                        wScan = scan,
                        dwFlags = KeyeventfScancode | KeyeventfKeyup
                    }
                }
            };
        }
    }
}
