using Gma.System.MouseKeyHook;
using System;
using System.Threading;
using System.Windows.Forms;
using Keyboard = Corale.Colore.Razer.Keyboard;
using Timer = System.Windows.Forms.Timer;

namespace LED_Hive
{
    public partial class MainForm : Form
    {
        //use a third party method to handle eventhandlers
        IKeyboardMouseEvents _events;

        // Keep thread alive until exit
        private bool _stayAwake = true;

        public static bool IsMouseButtonPressed;
        public static bool IsKeyboardKeyPressed;
        public static Keyboard.Key KeyboardKeyPressed;
        public static Keyboard.Key KeyboardKeyReleased;
        private static readonly Timer KeyboardWaveTimer = new Timer { Interval = 70 };


        public MainForm()
        {
            InitializeComponent();
            Corale.Colore.Core.Chroma.Instance.Initialize();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _events = Hook.GlobalEvents();
            _events.MouseDown += OnMouseDown;
            _events.MouseUp += OnMouseUp;
            _events.KeyDown += OnKeyDown;
            _events.KeyUp += OnKeyUp;

            WindowState = FormWindowState.Minimized;
            FormClosed += Main_FormClosed;

            var thread = new Thread(UpdateThread);
            thread.Start();

            KeyboardWaveTimer.Tick += KeyboardWaveTimer_Tick;
            KeyboardWaveTimer.Start();

        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            _events.MouseDown -= OnMouseDown;
            _events.MouseUp -= OnMouseUp;
            _events.KeyDown -= OnKeyDown;
            _events.KeyUp -= OnKeyUp;
            KeyboardWaveTimer.Stop();
            KeyboardWaveTimer.Tick -= KeyboardWaveTimer_Tick;
            _stayAwake = false;
        }

        private static void OnMouseDown(object sender, MouseEventArgs e)
        {
            IsMouseButtonPressed = true;
        }

        private static void OnMouseUp(object sender, MouseEventArgs e)
        {
            IsMouseButtonPressed = false;
        }

        private static void OnKeyDown(object sender, KeyEventArgs e)
        {
            //find out which Key is pressed and return a value which the corale colore library can understand
            switch (e.KeyValue)
            {
                case 27:
                    KeyboardKeyPressed = Keyboard.Key.Escape;
                    break;
                case 112:
                    KeyboardKeyPressed = Keyboard.Key.F1;
                    break;
                case 113:
                    KeyboardKeyPressed = Keyboard.Key.F2;
                    break;
                case 114:
                    KeyboardKeyPressed = Keyboard.Key.F3;
                    break;
                case 115:
                    KeyboardKeyPressed = Keyboard.Key.F4;
                    break;
                case 116:
                    KeyboardKeyPressed = Keyboard.Key.F5;
                    break;
                case 117:
                    KeyboardKeyPressed = Keyboard.Key.F6;
                    break;
                case 118:
                    KeyboardKeyPressed = Keyboard.Key.F7;
                    break;
                case 119:
                    KeyboardKeyPressed = Keyboard.Key.F8;
                    break;
                case 120:
                    KeyboardKeyPressed = Keyboard.Key.F9;
                    break;
                case 121:
                    KeyboardKeyPressed = Keyboard.Key.F10;
                    break;
                case 122:
                    KeyboardKeyPressed = Keyboard.Key.F11;
                    break;
                case 123:
                    KeyboardKeyPressed = Keyboard.Key.F12;
                    break;
                case 44:
                    KeyboardKeyPressed = Keyboard.Key.PrintScreen;
                    break;
                case 145:
                    KeyboardKeyPressed = Keyboard.Key.Scroll;
                    break;
                case 19:
                    KeyboardKeyPressed = Keyboard.Key.Pause;
                    break;
                case 220:
                    KeyboardKeyPressed = Keyboard.Key.OemTilde;
                    break;
                case 49:
                    KeyboardKeyPressed = Keyboard.Key.D1;
                    break;
                case 50:
                    KeyboardKeyPressed = Keyboard.Key.D2;
                    break;
                case 51:
                    KeyboardKeyPressed = Keyboard.Key.D3;
                    break;
                case 52:
                    KeyboardKeyPressed = Keyboard.Key.D4;
                    break;
                case 53:
                    KeyboardKeyPressed = Keyboard.Key.D5;
                    break;
                case 54:
                    KeyboardKeyPressed = Keyboard.Key.D6;
                    break;
                case 55:
                    KeyboardKeyPressed = Keyboard.Key.D7;
                    break;
                case 56:
                    KeyboardKeyPressed = Keyboard.Key.D8;
                    break;
                case 57:
                    KeyboardKeyPressed = Keyboard.Key.D9;
                    break;
                case 48:
                    KeyboardKeyPressed = Keyboard.Key.D0;
                    break;
                case 219:
                    KeyboardKeyPressed = Keyboard.Key.OemMinus;
                    break;
                case 221:
                    KeyboardKeyPressed = Keyboard.Key.OemEquals;
                    break;
                case 8:
                    KeyboardKeyPressed = Keyboard.Key.Backspace;
                    break;
                case 45:
                    KeyboardKeyPressed = Keyboard.Key.Insert;
                    break;
                case 36:
                    KeyboardKeyPressed = Keyboard.Key.Home;
                    break;
                case 33:
                    KeyboardKeyPressed = Keyboard.Key.PageUp;
                    break;
                case 9:
                    KeyboardKeyPressed = Keyboard.Key.Tab;
                    break;
                case 81:
                    KeyboardKeyPressed = Keyboard.Key.Q;
                    break;
                case 87:
                    KeyboardKeyPressed = Keyboard.Key.W;
                    break;
                case 69:
                    KeyboardKeyPressed = Keyboard.Key.E;
                    break;
                case 82:
                    KeyboardKeyPressed = Keyboard.Key.R;
                    break;
                case 84:
                    KeyboardKeyPressed = Keyboard.Key.T;
                    break;
                case 90:
                    KeyboardKeyPressed = Keyboard.Key.Z;
                    break;
                case 85:
                    KeyboardKeyPressed = Keyboard.Key.U;
                    break;
                case 73:
                    KeyboardKeyPressed = Keyboard.Key.I;
                    break;
                case 79:
                    KeyboardKeyPressed = Keyboard.Key.O;
                    break;
                case 80:
                    KeyboardKeyPressed = Keyboard.Key.P;
                    break;
                case 186:
                    KeyboardKeyPressed = Keyboard.Key.OemLeftBracket;
                    break;
                case 187:
                    KeyboardKeyPressed = Keyboard.Key.OemRightBracket;
                    break;
                case 46:
                    KeyboardKeyPressed = Keyboard.Key.Delete;
                    break;
                case 35:
                    KeyboardKeyPressed = Keyboard.Key.End;
                    break;
                case 34:
                    KeyboardKeyPressed = Keyboard.Key.PageDown;
                    break;
                case 13:
                    KeyboardKeyPressed = Keyboard.Key.Enter;
                    break;
                case 20:
                    KeyboardKeyPressed = Keyboard.Key.CapsLock;
                    break;
                case 65:
                    KeyboardKeyPressed = Keyboard.Key.A;
                    break;
                case 83:
                    KeyboardKeyPressed = Keyboard.Key.S;
                    break;
                case 68:
                    KeyboardKeyPressed = Keyboard.Key.D;
                    break;
                case 70:
                    KeyboardKeyPressed = Keyboard.Key.F;
                    break;
                case 71:
                    KeyboardKeyPressed = Keyboard.Key.G;
                    break;
                case 72:
                    KeyboardKeyPressed = Keyboard.Key.H;
                    break;
                case 74:
                    KeyboardKeyPressed = Keyboard.Key.J;
                    break;
                case 75:
                    KeyboardKeyPressed = Keyboard.Key.K;
                    break;
                case 76:
                    KeyboardKeyPressed = Keyboard.Key.L;
                    break;
                case 192:
                    KeyboardKeyPressed = Keyboard.Key.OemSemicolon;
                    break;
                case 222:
                    KeyboardKeyPressed = Keyboard.Key.OemApostrophe;
                    break;
                case 191:
                    KeyboardKeyPressed = Keyboard.Key.OemBackslash;
                    break;
                case 160:
                    KeyboardKeyPressed = Keyboard.Key.LeftShift;
                    break;
                case 226:
                    KeyboardKeyPressed = Keyboard.Key.EurBackslash;
                    break;
                case 89:
                    KeyboardKeyPressed = Keyboard.Key.Y;
                    break;
                case 88:
                    KeyboardKeyPressed = Keyboard.Key.X;
                    break;
                case 67:
                    KeyboardKeyPressed = Keyboard.Key.C;
                    break;
                case 86:
                    KeyboardKeyPressed = Keyboard.Key.V;
                    break;
                case 66:
                    KeyboardKeyPressed = Keyboard.Key.B;
                    break;
                case 78:
                    KeyboardKeyPressed = Keyboard.Key.N;
                    break;
                case 77:
                    KeyboardKeyPressed = Keyboard.Key.M;
                    break;
                case 188:
                    KeyboardKeyPressed = Keyboard.Key.OemComma;
                    break;
                case 190:
                    KeyboardKeyPressed = Keyboard.Key.OemPeriod;
                    break;
                case 189:
                    KeyboardKeyPressed = Keyboard.Key.OemSlash;
                    break;
                case 161:
                    KeyboardKeyPressed = Keyboard.Key.RightShift;
                    break;
                case 162:
                    KeyboardKeyPressed = Keyboard.Key.LeftControl;
                    break;
                case 91:
                    KeyboardKeyPressed = Keyboard.Key.LeftWindows;
                    break;
                case 164:
                    KeyboardKeyPressed = Keyboard.Key.LeftAlt;
                    break;
                case 32:
                    KeyboardKeyPressed = Keyboard.Key.Space;
                    break;
                case 165:
                    KeyboardKeyPressed = Keyboard.Key.RightAlt;
                    break;
                case 93:
                    KeyboardKeyPressed = Keyboard.Key.RightMenu;
                    break;
                case 163:
                    KeyboardKeyPressed = Keyboard.Key.RightControl;
                    break;
                case 37:
                    KeyboardKeyPressed = Keyboard.Key.Left;
                    break;
                case 40:
                    KeyboardKeyPressed = Keyboard.Key.Down;
                    break;
                case 39:
                    KeyboardKeyPressed = Keyboard.Key.Right;
                    break;
                case 38:
                    KeyboardKeyPressed = Keyboard.Key.Up;
                    break;
            }

            IsKeyboardKeyPressed = true;
        }

        private static void OnKeyUp(object sender, KeyEventArgs e)
        {
            // find out which Key is released and return a value which the corale colore library can understand
            switch (e.KeyValue)
            {
                case 27:
                    KeyboardKeyReleased = Keyboard.Key.Escape;
                    break;
                case 112:
                    KeyboardKeyReleased = Keyboard.Key.F1;
                    break;
                case 113:
                    KeyboardKeyReleased = Keyboard.Key.F2;
                    break;
                case 114:
                    KeyboardKeyReleased = Keyboard.Key.F3;
                    break;
                case 115:
                    KeyboardKeyReleased = Keyboard.Key.F4;
                    break;
                case 116:
                    KeyboardKeyReleased = Keyboard.Key.F5;
                    break;
                case 117:
                    KeyboardKeyReleased = Keyboard.Key.F6;
                    break;
                case 118:
                    KeyboardKeyReleased = Keyboard.Key.F7;
                    break;
                case 119:
                    KeyboardKeyReleased = Keyboard.Key.F8;
                    break;
                case 120:
                    KeyboardKeyReleased = Keyboard.Key.F9;
                    break;
                case 121:
                    KeyboardKeyReleased = Keyboard.Key.F10;
                    break;
                case 122:
                    KeyboardKeyReleased = Keyboard.Key.F11;
                    break;
                case 123:
                    KeyboardKeyReleased = Keyboard.Key.F12;
                    break;
                case 44:
                    KeyboardKeyReleased = Keyboard.Key.PrintScreen;
                    break;
                case 145:
                    KeyboardKeyReleased = Keyboard.Key.Scroll;
                    break;
                case 19:
                    KeyboardKeyReleased = Keyboard.Key.Pause;
                    break;
                case 220:
                    KeyboardKeyReleased = Keyboard.Key.OemTilde;
                    break;
                case 49:
                    KeyboardKeyReleased = Keyboard.Key.D1;
                    break;
                case 50:
                    KeyboardKeyReleased = Keyboard.Key.D2;
                    break;
                case 51:
                    KeyboardKeyReleased = Keyboard.Key.D3;
                    break;
                case 52:
                    KeyboardKeyReleased = Keyboard.Key.D4;
                    break;
                case 53:
                    KeyboardKeyReleased = Keyboard.Key.D5;
                    break;
                case 54:
                    KeyboardKeyReleased = Keyboard.Key.D6;
                    break;
                case 55:
                    KeyboardKeyReleased = Keyboard.Key.D7;
                    break;
                case 56:
                    KeyboardKeyReleased = Keyboard.Key.D8;
                    break;
                case 57:
                    KeyboardKeyReleased = Keyboard.Key.D9;
                    break;
                case 48:
                    KeyboardKeyReleased = Keyboard.Key.D0;
                    break;
                case 219:
                    KeyboardKeyReleased = Keyboard.Key.OemMinus;
                    break;
                case 221:
                    KeyboardKeyReleased = Keyboard.Key.OemEquals;
                    break;
                case 8:
                    KeyboardKeyReleased = Keyboard.Key.Backspace;
                    break;
                case 45:
                    KeyboardKeyReleased = Keyboard.Key.Insert;
                    break;
                case 36:
                    KeyboardKeyReleased = Keyboard.Key.Home;
                    break;
                case 33:
                    KeyboardKeyReleased = Keyboard.Key.PageUp;
                    break;
                case 9:
                    KeyboardKeyReleased = Keyboard.Key.Tab;
                    break;
                case 81:
                    KeyboardKeyReleased = Keyboard.Key.Q;
                    break;
                case 87:
                    KeyboardKeyReleased = Keyboard.Key.W;
                    break;
                case 69:
                    KeyboardKeyReleased = Keyboard.Key.E;
                    break;
                case 82:
                    KeyboardKeyReleased = Keyboard.Key.R;
                    break;
                case 84:
                    KeyboardKeyReleased = Keyboard.Key.T;
                    break;
                case 90:
                    KeyboardKeyReleased = Keyboard.Key.Z;
                    break;
                case 85:
                    KeyboardKeyReleased = Keyboard.Key.U;
                    break;
                case 73:
                    KeyboardKeyReleased = Keyboard.Key.I;
                    break;
                case 79:
                    KeyboardKeyReleased = Keyboard.Key.O;
                    break;
                case 80:
                    KeyboardKeyReleased = Keyboard.Key.P;
                    break;
                case 186:
                    KeyboardKeyReleased = Keyboard.Key.OemLeftBracket;
                    break;
                case 187:
                    KeyboardKeyReleased = Keyboard.Key.OemRightBracket;
                    break;
                case 46:
                    KeyboardKeyReleased = Keyboard.Key.Delete;
                    break;
                case 35:
                    KeyboardKeyReleased = Keyboard.Key.End;
                    break;
                case 34:
                    KeyboardKeyReleased = Keyboard.Key.PageDown;
                    break;
                case 13:
                    KeyboardKeyReleased = Keyboard.Key.Enter;
                    break;
                case 20:
                    KeyboardKeyReleased = Keyboard.Key.CapsLock;
                    break;
                case 65:
                    KeyboardKeyReleased = Keyboard.Key.A;
                    break;
                case 83:
                    KeyboardKeyReleased = Keyboard.Key.S;
                    break;
                case 68:
                    KeyboardKeyReleased = Keyboard.Key.D;
                    break;
                case 70:
                    KeyboardKeyReleased = Keyboard.Key.F;
                    break;
                case 71:
                    KeyboardKeyReleased = Keyboard.Key.G;
                    break;
                case 72:
                    KeyboardKeyReleased = Keyboard.Key.H;
                    break;
                case 74:
                    KeyboardKeyReleased = Keyboard.Key.J;
                    break;
                case 75:
                    KeyboardKeyReleased = Keyboard.Key.K;
                    break;
                case 76:
                    KeyboardKeyReleased = Keyboard.Key.L;
                    break;
                case 192:
                    KeyboardKeyReleased = Keyboard.Key.OemSemicolon;
                    break;
                case 222:
                    KeyboardKeyReleased = Keyboard.Key.OemApostrophe;
                    break;
                case 191:
                    KeyboardKeyReleased = Keyboard.Key.OemBackslash;
                    break;
                case 160:
                    KeyboardKeyReleased = Keyboard.Key.LeftShift;
                    break;
                case 226:
                    KeyboardKeyReleased = Keyboard.Key.EurBackslash;
                    break;
                case 89:
                    KeyboardKeyReleased = Keyboard.Key.Y;
                    break;
                case 88:
                    KeyboardKeyReleased = Keyboard.Key.X;
                    break;
                case 67:
                    KeyboardKeyReleased = Keyboard.Key.C;
                    break;
                case 86:
                    KeyboardKeyReleased = Keyboard.Key.V;
                    break;
                case 66:
                    KeyboardKeyReleased = Keyboard.Key.B;
                    break;
                case 78:
                    KeyboardKeyReleased = Keyboard.Key.N;
                    break;
                case 77:
                    KeyboardKeyReleased = Keyboard.Key.M;
                    break;
                case 188:
                    KeyboardKeyReleased = Keyboard.Key.OemComma;
                    break;
                case 190:
                    KeyboardKeyReleased = Keyboard.Key.OemPeriod;
                    break;
                case 189:
                    KeyboardKeyReleased = Keyboard.Key.OemSlash;
                    break;
                case 161:
                    KeyboardKeyReleased = Keyboard.Key.RightShift;
                    break;
                case 162:
                    KeyboardKeyReleased = Keyboard.Key.LeftControl;
                    break;
                case 91:
                    KeyboardKeyReleased = Keyboard.Key.LeftWindows;
                    break;
                case 164:
                    KeyboardKeyReleased = Keyboard.Key.LeftAlt;
                    break;
                case 32:
                    KeyboardKeyReleased = Keyboard.Key.Space;
                    break;
                case 165:
                    KeyboardKeyReleased = Keyboard.Key.RightAlt;
                    break;
                case 93:
                    KeyboardKeyReleased = Keyboard.Key.RightMenu;
                    break;
                case 163:
                    KeyboardKeyReleased = Keyboard.Key.RightControl;
                    break;
                case 37:
                    KeyboardKeyReleased = Keyboard.Key.Left;
                    break;
                case 40:
                    KeyboardKeyReleased = Keyboard.Key.Down;
                    break;
                case 39:
                    KeyboardKeyReleased = Keyboard.Key.Right;
                    break;
                case 38:
                    KeyboardKeyReleased = Keyboard.Key.Up;
                    break;
                default:
                    KeyboardKeyReleased = Keyboard.Key.Logo;
                    break;
            }
            IsKeyboardKeyPressed = false;
        }

        [MTAThread]
        private void UpdateThread()
        {
            try
            {
                Main.Start();
                while (_stayAwake)
                {
                    Main.Update();
                    Thread.Sleep(10);
                }
                Main.Quit();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private void KeyboardWaveTimer_Tick(object sender, EventArgs e)
        {
            Main.KeyboardWaveEffect();
        }

    }
}
