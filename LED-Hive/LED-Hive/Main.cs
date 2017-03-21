using Corale.Colore.Core;
using System;
using System.Drawing;
using System.Windows.Forms;
using ColoreColor = Corale.Colore.Core.Color;
using KeyboardCustom = Corale.Colore.Razer.Keyboard.Effects.Custom;
using MouseCustom = Corale.Colore.Razer.Mouse.Effects.Custom;
using MousepadCustom = Corale.Colore.Razer.Mousepad.Effects.Custom;
using KeyboardKey = Corale.Colore.Razer.Keyboard.Key;



namespace LED_Hive
{
    class Main
    {
        //Timer for Animations on mousebuttonpresses
        private static DateTime _objTimer = DateTime.MinValue;

        //Timer for fading in the Highlight mouseposition
        private static DateTime _objTimer2 = DateTime.MinValue;

        //primary color for mousepad and mouse
        private static ColoreColor _mousePrimaryColor = new ColoreColor((byte)Color.Default.MousePrimaryRed, (byte)Color.Default.MousePrimaryGreen, (byte)Color.Default.MousePrimaryBlue);

        //primary color for the keyboard
        private static ColoreColor _keyboardPrimaryColor = new ColoreColor((byte)Color.Default.KeyboardPrimaryRed, (byte)Color.Default.KeyboardPrimaryGreen, (byte)Color.Default.KeyboardPrimaryBlue);

        //secondary color for mousepad and mouse
        private static ColoreColor _mouseSecondaryColor = new ColoreColor((byte)Color.Default.MouseSecondaryRed, (byte)Color.Default.MouseSecondaryGreen, (byte)Color.Default.MouseSecondaryBlue);

        //secondary color for the keyboard
        private static ColoreColor _keyboardSecondaryColor = new ColoreColor((byte)Color.Default.KeyboardSecondaryRed, (byte)Color.Default.KeyboardSecondaryGreen, (byte)Color.Default.KeyboardSecondaryBlue);

        //tertiary color for mousepad and mouse
        private static ColoreColor _mouseTertiaryColor = new ColoreColor((byte)Color.Default.MouseTertiaryRed, (byte)Color.Default.MouseTertiaryGreen, (byte)Color.Default.MouseTertiaryBlue);

        //reference to the current screen
        private static Screen _screen;

        //red value for animations on mousebuttonpresses
        private static float _flMouseAnimationRed = Color.Default.MousePrimaryRed;

        //green value for animations on mousebuttonpresses
        private static float _flMouseAnimationGreen = Color.Default.MousePrimaryGreen;

        //blue value for animations on mousebuttonpresses
        private static float _flMouseAnimationBlue = Color.Default.MousePrimaryBlue;

        //boolean used for the Animations on mousebuttonpresses
        private static bool _isAnimationReady = true;

        //custom grid for the mouse
        private static MouseCustom _customMouse = MouseCustom.Create();

        //custom grid for the mousepad
        private static MousepadCustom _customMousepad = MousepadCustom.Create();

        //custom grid for the keyboard
        private static KeyboardCustom _customKeyboard = KeyboardCustom.Create();

        //Key indicator for KeyboardWaveEffect()
        private static int _intWaveKey;



        public static void Start()
        {
            SetAllOnMousepad(_mousePrimaryColor);
            SetAllOnKeyboard(_keyboardPrimaryColor);
            SetAllOnMouse(_mousePrimaryColor);
        }

        public static void Update()
        {
            UpdateCursorPosition();
            KeyboardReactiveEffect();
            MouseAndMousepadReactive();
        }

        public static void Quit()
        {
            try
            {
                Keyboard.Instance.Clear();
                Mouse.Instance.Clear();
                Mousepad.Instance.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static void UpdateMouseColors(int primred, int primgreen, int primblue)
        {
            Color.Default.MousePrimaryRed = primred;
            Color.Default.MousePrimaryGreen = primgreen;
            Color.Default.MousePrimaryBlue = primblue;
            Color.Default.Save();
            _mousePrimaryColor = new ColoreColor((byte)Color.Default.MousePrimaryRed, (byte)Color.Default.MousePrimaryGreen, (byte)Color.Default.MousePrimaryBlue);
        }



        private static float Lerp(float from, float to, float value)
        {
            if (value < 0.0f)
                return from;
            if (value > 1.0f)
                return to;
            return (to - from) * value + from;
        }

        private static float InverseLerp(float from, float to, float value)
        {
            if (from < to)
            {
                if (value < from)
                    return 0.0f;
                if (value > to)
                    return 1.0f;
            }
            else
            {
                if (value < to)
                    return 1.0f;
                if (value > from)
                    return 0.0f;
            }
            return (value - from) / (to - from);
        }

        private static int GetIndex()
        {
            var halfWidth = _screen.Bounds.Width * 0.5f;
            var x = _screen.WorkingArea.Left;
            var y = _screen.WorkingArea.Top;
            if ((Cursor.Position.X - x) < halfWidth)
            {
                return (int)Lerp(8, 14, InverseLerp(_screen.Bounds.Height, 0, Cursor.Position.Y - y));
            }
            {
                return (int)Lerp(7, 1, InverseLerp(_screen.Bounds.Height, 0, Cursor.Position.Y - y));
            }
        }


        private static void UpdateCursorPosition()
        {
            _screen = Screen.FromRectangle(new Rectangle(Cursor.Position.X, Cursor.Position.Y, 1, 1));
        }

        private static void HighlightMousePosition(ColoreColor color)
        {

            for (var i = 0; i < Corale.Colore.Razer.Mousepad.Constants.MaxLeds; ++i)
            {
                if (i >= (GetIndex() - 1) &&
                    i <= (GetIndex() + 1))
                {
                    _customMousepad[i] = color;
                }
                else
                {
                    _customMousepad[i] = _mousePrimaryColor;
                }
            }
            Chroma.Instance.Mousepad.SetCustom(_customMousepad);

        }

        private static ColoreColor CalculateKeyboardWaveColors(int amount, int fadeNumber)
        {
            var red = (Math.Max(Color.Default.KeyboardPrimaryRed, Color.Default.KeyboardSecondaryRed) -
                       Math.Min(Color.Default.KeyboardPrimaryRed, Color.Default.KeyboardSecondaryRed)) / (amount + 1);
            if (Color.Default.KeyboardPrimaryRed > Color.Default.KeyboardSecondaryRed)
                red = Color.Default.KeyboardPrimaryRed - (red * fadeNumber);
            else
                red = Color.Default.KeyboardPrimaryRed + (red * fadeNumber);

            var green = (Math.Max(Color.Default.KeyboardPrimaryGreen, Color.Default.KeyboardSecondaryGreen) -
                         Math.Min(Color.Default.KeyboardPrimaryGreen, Color.Default.KeyboardSecondaryGreen)) / (amount + 1);
            if (Color.Default.KeyboardPrimaryGreen > Color.Default.KeyboardSecondaryGreen)
                green = Color.Default.KeyboardPrimaryGreen - (green * fadeNumber);
            else
                green = Color.Default.KeyboardPrimaryGreen + (green * fadeNumber);

            var blue = (Math.Max(Color.Default.KeyboardPrimaryBlue, Color.Default.KeyboardSecondaryBlue) -
                        Math.Min(Color.Default.KeyboardPrimaryBlue, Color.Default.KeyboardSecondaryBlue)) / (amount + 1);
            if (Color.Default.KeyboardPrimaryBlue > Color.Default.KeyboardSecondaryBlue)
                blue = Color.Default.KeyboardPrimaryBlue - (blue * fadeNumber);
            else
                blue = Color.Default.KeyboardPrimaryBlue + (blue * fadeNumber);


            return new ColoreColor((byte)red, (byte)green, (byte)blue);

        }


        private static void MouseAndMousepadReactive()
        {
            //events when MouseButton pressed
            if (MainForm.IsMouseButtonPressed)
            {
                SetAllOnMouse(_mouseTertiaryColor);
                _flMouseAnimationRed = Color.Default.MouseTertiaryRed;
                _flMouseAnimationGreen = Color.Default.MouseTertiaryGreen;
                _flMouseAnimationBlue = Color.Default.MouseTertiaryBlue;

                HighlightMousePosition(_mouseTertiaryColor);
                _objTimer = DateTime.Now + TimeSpan.FromMilliseconds(1000);
                _isAnimationReady = false;
            }

            //events when MouseButton released
            else if (_objTimer > DateTime.Now)
            {
                var color = new ColoreColor((byte)_flMouseAnimationRed, (byte)_flMouseAnimationGreen, (byte)_flMouseAnimationBlue);


                if (Color.Default.MousePrimaryRed > Color.Default.MouseTertiaryRed)
                {
                    _flMouseAnimationRed = _flMouseAnimationRed + ((float)(Color.Default.MousePrimaryRed - Color.Default.MouseTertiaryRed) / 100);

                    if (_flMouseAnimationRed >= Color.Default.MousePrimaryRed) _flMouseAnimationRed = Color.Default.MousePrimaryRed;
                }
                else if (Color.Default.MousePrimaryRed < Color.Default.MouseTertiaryRed)
                {
                    _flMouseAnimationRed = _flMouseAnimationRed - ((float)(Color.Default.MouseTertiaryRed - Color.Default.MousePrimaryRed) / 100);

                    if (_flMouseAnimationRed <= Color.Default.MousePrimaryRed) _flMouseAnimationRed = Color.Default.MousePrimaryRed;
                }

                if (Color.Default.MousePrimaryGreen > Color.Default.MouseTertiaryGreen)
                {
                    _flMouseAnimationGreen = _flMouseAnimationGreen + ((float)(Color.Default.MousePrimaryGreen - Color.Default.MouseTertiaryGreen) / 100);

                    if (_flMouseAnimationGreen >= Color.Default.MousePrimaryGreen) _flMouseAnimationGreen = Color.Default.MousePrimaryGreen;
                }
                else if (Color.Default.MousePrimaryGreen < Color.Default.MouseTertiaryGreen)
                {
                    _flMouseAnimationGreen = _flMouseAnimationGreen - ((float)(Color.Default.MouseTertiaryGreen - Color.Default.MousePrimaryGreen) / 100);

                    if (_flMouseAnimationGreen <= Color.Default.MousePrimaryGreen) _flMouseAnimationGreen = Color.Default.MousePrimaryGreen;
                }

                if (Color.Default.MousePrimaryBlue > Color.Default.MouseTertiaryBlue)
                {
                    _flMouseAnimationBlue = _flMouseAnimationBlue + ((float)(Color.Default.MousePrimaryBlue - Color.Default.MouseTertiaryBlue) / 100);

                    if (_flMouseAnimationBlue >= Color.Default.MousePrimaryBlue) _flMouseAnimationBlue = Color.Default.MousePrimaryBlue;
                }
                else if (Color.Default.MousePrimaryBlue < Color.Default.MouseTertiaryBlue)
                {
                    _flMouseAnimationBlue = _flMouseAnimationBlue - ((float)(Color.Default.MouseTertiaryBlue - Color.Default.MousePrimaryBlue) / 100);

                    if (_flMouseAnimationBlue <= Color.Default.MousePrimaryBlue) _flMouseAnimationBlue = Color.Default.MousePrimaryBlue;
                }

                SetAllOnMousepad(color);
                SetAllOnMouse(color);
            }

            //setting values after mousebutton release animations are done
            else if (_objTimer != DateTime.MinValue)
            {
                _flMouseAnimationRed = Color.Default.MousePrimaryRed;
                _flMouseAnimationGreen = Color.Default.MousePrimaryGreen;
                _flMouseAnimationBlue = Color.Default.MousePrimaryBlue;
                _objTimer = DateTime.MinValue;
                _objTimer2 = DateTime.Now + TimeSpan.FromMilliseconds(1000);
            }

            //return to default mode on mouse and mousepad
            else
            {
                //return to primary color on the mouse
                SetAllOnMouse(_mousePrimaryColor);

                //Fade back in "highlight Mouseposition"
                if (_objTimer2 > DateTime.Now)
                {
                    HighlightMousePosition(new ColoreColor((byte)_flMouseAnimationRed, (byte)_flMouseAnimationGreen, (byte)_flMouseAnimationBlue));

                    if (Color.Default.MouseSecondaryRed > Color.Default.MousePrimaryRed)
                    {
                        _flMouseAnimationRed = _flMouseAnimationRed + ((float)(Color.Default.MouseSecondaryRed - Color.Default.MousePrimaryRed) / 100);

                        if (_flMouseAnimationRed >= Color.Default.MouseSecondaryRed) _flMouseAnimationRed = Color.Default.MouseSecondaryRed;
                    }
                    else if (Color.Default.MouseSecondaryRed < Color.Default.MousePrimaryRed)
                    {
                        _flMouseAnimationRed = _flMouseAnimationRed - ((float)(Color.Default.MousePrimaryRed - Color.Default.MouseSecondaryRed) / 100);

                        if (_flMouseAnimationRed <= Color.Default.MouseSecondaryRed) _flMouseAnimationRed = Color.Default.MouseSecondaryRed;
                    }

                    if (Color.Default.MouseSecondaryGreen > Color.Default.MousePrimaryGreen)
                    {
                        _flMouseAnimationGreen = _flMouseAnimationGreen + ((float)(Color.Default.MouseSecondaryGreen - Color.Default.MousePrimaryGreen) / 100);

                        if (_flMouseAnimationGreen >= Color.Default.MouseSecondaryGreen) _flMouseAnimationGreen = Color.Default.MouseSecondaryGreen;
                    }
                    else if (Color.Default.MouseSecondaryGreen < Color.Default.MousePrimaryGreen)
                    {
                        _flMouseAnimationGreen = _flMouseAnimationGreen - ((float)(Color.Default.MousePrimaryGreen - Color.Default.MouseSecondaryGreen) / 100);

                        if (_flMouseAnimationGreen <= Color.Default.MouseSecondaryGreen) _flMouseAnimationGreen = Color.Default.MouseSecondaryGreen;
                    }

                    if (Color.Default.MouseSecondaryBlue > Color.Default.MousePrimaryBlue)
                    {
                        _flMouseAnimationBlue = _flMouseAnimationBlue + ((float)(Color.Default.MouseSecondaryBlue - Color.Default.MousePrimaryBlue) / 100);

                        if (_flMouseAnimationBlue >= Color.Default.MouseSecondaryBlue) _flMouseAnimationBlue = Color.Default.MouseSecondaryBlue;
                    }
                    else if (Color.Default.MouseSecondaryBlue < Color.Default.MousePrimaryBlue)
                    {
                        _flMouseAnimationBlue = _flMouseAnimationBlue - ((float)(Color.Default.MousePrimaryBlue - Color.Default.MouseSecondaryBlue) / 100);

                        if (_flMouseAnimationBlue <= Color.Default.MouseSecondaryBlue) _flMouseAnimationBlue = Color.Default.MouseSecondaryBlue;
                    }
                }

                //resetting values after animation is done
                else if (_objTimer2 != DateTime.MinValue)
                {
                    _objTimer2 = DateTime.MinValue;
                    _isAnimationReady = true;
                }

                else if (_isAnimationReady)
                {
                    //highlight the Mouseposition in secondary color
                    HighlightMousePosition(_mouseSecondaryColor);
                }

            }
        }


        public static void KeyboardWaveEffect()
        {
            _intWaveKey += 1;
            if (_intWaveKey <= 16)
            {
                switch (_intWaveKey)
                {
                    case 1:
                        _customKeyboard[KeyboardKey.T] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.F] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.D] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.S] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.A] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.CapsLock] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.OemBackslash] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.OemApostrophe] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.OemSemicolon] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.L] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.K] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.J] = _keyboardPrimaryColor;
                        break;
                    case 2:
                        _customKeyboard[KeyboardKey.D6] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.T] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.F] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.D] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.S] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.A] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.CapsLock] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.OemBackslash] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.OemApostrophe] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.OemSemicolon] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.L] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.K] = _keyboardPrimaryColor;
                        break;
                    case 3:
                        _customKeyboard[KeyboardKey.Z] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.D6] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.T] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.F] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.D] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.S] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.A] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.CapsLock] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.OemBackslash] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.OemApostrophe] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.OemSemicolon] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.L] = _keyboardPrimaryColor;
                        break;
                    case 4:
                        _customKeyboard[KeyboardKey.H] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.Z] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.D6] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.T] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.F] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.D] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.S] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.A] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.CapsLock] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.OemBackslash] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.OemApostrophe] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.OemSemicolon] = _keyboardPrimaryColor;
                        break;
                    case 5:
                        _customKeyboard[KeyboardKey.N] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.H] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.Z] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.D6] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.T] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.F] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.D] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.S] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.A] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.CapsLock] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.OemBackslash] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.OemApostrophe] = _keyboardPrimaryColor;
                        break;
                    case 6:
                        _customKeyboard[KeyboardKey.J] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.N] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.H] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.Z] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.D6] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.T] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.F] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.D] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.S] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.A] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.CapsLock] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.OemBackslash] = _keyboardPrimaryColor;
                        break;
                    case 7:
                        _customKeyboard[KeyboardKey.K] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.J] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.N] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.H] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.Z] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.D6] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.T] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.F] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.D] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.S] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.A] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.CapsLock] = _keyboardPrimaryColor;
                        break;
                    case 8:
                        _customKeyboard[KeyboardKey.L] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.K] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.J] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.N] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.H] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.Z] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.D6] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.T] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.F] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.D] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.S] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.A] = _keyboardPrimaryColor;
                        break;
                    case 9:
                        _customKeyboard[KeyboardKey.OemSemicolon] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.L] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.K] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.J] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.N] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.H] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.Z] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.D6] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.T] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.F] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.D] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.S] = _keyboardPrimaryColor;
                        break;
                    case 10:
                        _customKeyboard[KeyboardKey.OemApostrophe] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.OemSemicolon] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.L] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.K] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.J] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.N] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.H] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.Z] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.D6] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.T] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.F] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.D] = _keyboardPrimaryColor;
                        break;
                    case 11:
                        _customKeyboard[KeyboardKey.OemBackslash] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.OemApostrophe] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.OemSemicolon] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.L] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.K] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.J] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.N] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.H] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.Z] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.D6] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.T] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.F] = _keyboardPrimaryColor;
                        break;
                    case 12:
                        _customKeyboard[KeyboardKey.CapsLock] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.OemBackslash] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.OemApostrophe] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.OemSemicolon] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.L] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.K] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.J] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.N] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.H] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.Z] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.D6] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.T] = _keyboardPrimaryColor;
                        break;
                    case 13:
                        _customKeyboard[KeyboardKey.A] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.CapsLock] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.OemBackslash] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.OemApostrophe] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.OemSemicolon] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.L] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.K] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.J] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.N] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.H] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.Z] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.D6] = _keyboardPrimaryColor;
                        break;
                    case 14:
                        _customKeyboard[KeyboardKey.S] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.A] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.CapsLock] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.OemBackslash] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.OemApostrophe] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.OemSemicolon] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.L] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.K] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.J] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.N] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.H] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.Z] = _keyboardPrimaryColor;
                        break;
                    case 15:
                        _customKeyboard[KeyboardKey.D] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.S] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.A] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.CapsLock] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.OemBackslash] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.OemApostrophe] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.OemSemicolon] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.L] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.K] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.J] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.N] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.H] = _keyboardPrimaryColor;
                        break;
                    case 16:
                        _customKeyboard[KeyboardKey.F] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.D] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.S] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.A] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.CapsLock] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.OemBackslash] = _keyboardSecondaryColor;
                        _customKeyboard[KeyboardKey.OemApostrophe] = CalculateKeyboardWaveColors(5, 5);
                        _customKeyboard[KeyboardKey.OemSemicolon] = CalculateKeyboardWaveColors(5, 4);
                        _customKeyboard[KeyboardKey.L] = CalculateKeyboardWaveColors(5, 3);
                        _customKeyboard[KeyboardKey.K] = CalculateKeyboardWaveColors(5, 2);
                        _customKeyboard[KeyboardKey.J] = CalculateKeyboardWaveColors(5, 1);
                        _customKeyboard[KeyboardKey.N] = _keyboardPrimaryColor;
                        break;
                }
                Chroma.Instance.Keyboard.SetCustom(_customKeyboard);
            }
            else
            {
                _intWaveKey = 0;
            }
        }

        private static void KeyboardReactiveEffect()
        {

            if (MainForm.IsKeyboardKeyPressed)
            {
                Chroma.Instance.Keyboard[MainForm.KeyboardKeyPressed] = _keyboardSecondaryColor;
            }
            else
            {
                Chroma.Instance.Keyboard[MainForm.KeyboardKeyReleased] = _keyboardPrimaryColor;
                MainForm.KeyboardKeyReleased = KeyboardKey.Logo;
            }
        }

        private static void SetAllOnMousepad(ColoreColor color)
        {
            for (int g = 0; g < Corale.Colore.Razer.Mousepad.Constants.MaxLeds; g++)
            {
                _customMousepad[g] = color;
            }
            Chroma.Instance.Mousepad.SetCustom(_customMousepad);
        }

        private static void SetAllOnKeyboard(ColoreColor color)
        {
            for (int r = 0; r < Corale.Colore.Razer.Keyboard.Constants.MaxRows; r++)
            {
                for (int c = 0; c < Corale.Colore.Razer.Keyboard.Constants.MaxColumns; c++)
                {
                    _customKeyboard[r, c] = color;
                }
            }

            Chroma.Instance.Keyboard.SetCustom(_customKeyboard);

        }

        private static void SetAllOnMouse(ColoreColor color)
        {
            for (int i = 0; i < Corale.Colore.Razer.Mouse.Constants.MaxLeds; i++)
            {
                _customMouse[i] = color;
            }
            Chroma.Instance.Mouse.SetCustom(_customMouse);
        }

    }
}
