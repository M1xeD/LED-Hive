using Corale.Colore.Core;
using System;
using System.Drawing;
using System.Windows.Forms;
using ColoreColor = Corale.Colore.Core.Color;
using KeyboardCustom = Corale.Colore.Razer.Keyboard.Effects.Custom;
using MouseCustom = Corale.Colore.Razer.Mouse.Effects.Custom;
using MousepadCustom = Corale.Colore.Razer.Mousepad.Effects.Custom;



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

        private static ColoreColor CalculateKeyboardWaveColors(int x)
        {
            ColoreColor objFadeColor1;
            ColoreColor objFadeColor2;
            ColoreColor objFadeColor3;

            int intFadeLevel = x;

            int r3 = 0, g3 = 0, b3 = 0;
            int r2 = 0, g2 = 0, b2 = 0;
            int r1 = 0, g1 = 0, b1 = 0;

            if (Color.Default.KeyboardPrimaryRed > Color.Default.KeyboardSecondaryRed)
            {
                r3 = (Color.Default.KeyboardPrimaryRed - Color.Default.KeyboardSecondaryRed) / 4;
                r3 = Color.Default.KeyboardPrimaryRed - r3;
            }
            else if (Color.Default.KeyboardPrimaryRed < Color.Default.KeyboardSecondaryRed)
            {
                r3 = (Color.Default.KeyboardSecondaryRed - Color.Default.KeyboardPrimaryRed) / 4;
                r3 = Color.Default.KeyboardPrimaryRed + r3;
            }

            if (Color.Default.KeyboardPrimaryGreen > Color.Default.KeyboardSecondaryGreen)
            {
                g3 = (Color.Default.KeyboardPrimaryGreen - Color.Default.KeyboardSecondaryGreen) / 4;
                g3 = Color.Default.KeyboardPrimaryGreen - g3;
            }
            else if (Color.Default.KeyboardPrimaryGreen < Color.Default.KeyboardSecondaryGreen)
            {
                g3 = (Color.Default.KeyboardSecondaryGreen - Color.Default.KeyboardPrimaryGreen) / 4;
                g3 = Color.Default.KeyboardPrimaryGreen + g3;
            }

            if (Color.Default.KeyboardPrimaryBlue > Color.Default.KeyboardSecondaryBlue)
            {
                b3 = (Color.Default.KeyboardPrimaryBlue - Color.Default.KeyboardSecondaryBlue) / 4;
                b3 = Color.Default.KeyboardPrimaryBlue - b3;
            }
            else if (Color.Default.KeyboardPrimaryBlue < Color.Default.KeyboardSecondaryBlue)
            {
                b3 = (Color.Default.KeyboardSecondaryBlue - Color.Default.KeyboardPrimaryBlue) / 4;
                b3 = Color.Default.KeyboardPrimaryBlue + b3;
            }

            objFadeColor3 = new ColoreColor((byte)r3, (byte)g3, (byte)b3);


            if (Color.Default.KeyboardPrimaryRed > Color.Default.KeyboardSecondaryRed)
            {
                r2 = (Color.Default.KeyboardPrimaryRed - Color.Default.KeyboardSecondaryRed) / 4;
                r2 = Color.Default.KeyboardPrimaryRed - (r2 * 2);
            }
            else if (Color.Default.KeyboardPrimaryRed < Color.Default.KeyboardSecondaryRed)
            {
                r2 = (Color.Default.KeyboardSecondaryRed - Color.Default.KeyboardPrimaryRed) / 4;
                r2 = Color.Default.KeyboardPrimaryRed + (r2 * 2);
            }

            if (Color.Default.KeyboardPrimaryGreen > Color.Default.KeyboardSecondaryGreen)
            {
                g2 = (Color.Default.KeyboardPrimaryGreen - Color.Default.KeyboardSecondaryGreen) / 4;
                g2 = Color.Default.KeyboardPrimaryGreen - (g2 * 2);
            }
            else if (Color.Default.KeyboardPrimaryGreen < Color.Default.KeyboardSecondaryGreen)
            {
                g2 = (Color.Default.KeyboardSecondaryGreen - Color.Default.KeyboardPrimaryGreen) / 4;
                g2 = Color.Default.KeyboardPrimaryGreen + (g2 * 2);
            }

            if (Color.Default.KeyboardPrimaryBlue > Color.Default.KeyboardSecondaryBlue)
            {
                b2 = (Color.Default.KeyboardPrimaryBlue - Color.Default.KeyboardSecondaryBlue) / 4;
                b2 = Color.Default.KeyboardPrimaryBlue - (b2 * 2);
            }
            else if (Color.Default.KeyboardPrimaryBlue < Color.Default.KeyboardSecondaryBlue)
            {
                b2 = (Color.Default.KeyboardSecondaryBlue - Color.Default.KeyboardPrimaryBlue) / 4;
                b2 = Color.Default.KeyboardPrimaryBlue + (b2 * 2);
            }

            objFadeColor2 = new ColoreColor((byte)r2, (byte)g2, (byte)b2);

            if (Color.Default.KeyboardPrimaryRed > Color.Default.KeyboardSecondaryRed)
            {
                r1 = (Color.Default.KeyboardPrimaryRed - Color.Default.KeyboardSecondaryRed) / 4;
                r1 = Color.Default.KeyboardPrimaryRed - (r1 * 3);
            }
            else if (Color.Default.KeyboardPrimaryRed < Color.Default.KeyboardSecondaryRed)
            {
                r1 = (Color.Default.KeyboardSecondaryRed - Color.Default.KeyboardPrimaryRed) / 4;
                r1 = Color.Default.KeyboardPrimaryRed + (r1 * 3);
            }

            if (Color.Default.KeyboardPrimaryGreen > Color.Default.KeyboardSecondaryGreen)
            {
                g1 = (Color.Default.KeyboardPrimaryGreen - Color.Default.KeyboardSecondaryGreen) / 4;
                g1 = Color.Default.KeyboardPrimaryGreen - (g1 * 3);
            }
            else if (Color.Default.KeyboardPrimaryGreen < Color.Default.KeyboardSecondaryGreen)
            {
                g1 = (Color.Default.KeyboardSecondaryGreen - Color.Default.KeyboardPrimaryGreen) / 4;
                g1 = Color.Default.KeyboardPrimaryGreen + (g1 * 3);
            }

            if (Color.Default.KeyboardPrimaryBlue > Color.Default.KeyboardSecondaryBlue)
            {
                b1 = (Color.Default.KeyboardPrimaryBlue - Color.Default.KeyboardSecondaryBlue) / 4;
                b1 = Color.Default.KeyboardPrimaryBlue - (b1 * 3);
            }
            else if (Color.Default.KeyboardPrimaryBlue < Color.Default.KeyboardSecondaryBlue)
            {
                b1 = (Color.Default.KeyboardSecondaryBlue - Color.Default.KeyboardPrimaryBlue) / 4;
                b1 = Color.Default.KeyboardPrimaryBlue + (b1 * 3);
            }

            objFadeColor1 = new ColoreColor((byte)r1, (byte)g1, (byte)b1);


            if (intFadeLevel == 1) return objFadeColor1;
            if (intFadeLevel == 2) return objFadeColor2;
            if (intFadeLevel == 3) return objFadeColor3;

            return ColoreColor.Black;
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
                    _flMouseAnimationRed = _flMouseAnimationRed + ((float)(Color.Default.MousePrimaryRed - Color.Default.MouseTertiaryRed) / 400);

                    if (_flMouseAnimationRed >= Color.Default.MousePrimaryRed) _flMouseAnimationRed = Color.Default.MousePrimaryRed;
                }
                else if (Color.Default.MousePrimaryRed < Color.Default.MouseTertiaryRed)
                {
                    _flMouseAnimationRed = _flMouseAnimationRed - ((float)(Color.Default.MouseTertiaryRed - Color.Default.MousePrimaryRed) / 400);

                    if (_flMouseAnimationRed <= Color.Default.MousePrimaryRed) _flMouseAnimationRed = Color.Default.MousePrimaryRed;
                }

                if (Color.Default.MousePrimaryGreen > Color.Default.MouseTertiaryGreen)
                {
                    _flMouseAnimationGreen = _flMouseAnimationGreen + ((float)(Color.Default.MousePrimaryGreen - Color.Default.MouseTertiaryGreen) / 400);

                    if (_flMouseAnimationGreen >= Color.Default.MousePrimaryGreen) _flMouseAnimationGreen = Color.Default.MousePrimaryGreen;
                }
                else if (Color.Default.MousePrimaryGreen < Color.Default.MouseTertiaryGreen)
                {
                    _flMouseAnimationGreen = _flMouseAnimationGreen - ((float)(Color.Default.MouseTertiaryGreen - Color.Default.MousePrimaryGreen) / 400);

                    if (_flMouseAnimationGreen <= Color.Default.MousePrimaryGreen) _flMouseAnimationGreen = Color.Default.MousePrimaryGreen;
                }

                if (Color.Default.MousePrimaryBlue > Color.Default.MouseTertiaryBlue)
                {
                    _flMouseAnimationBlue = _flMouseAnimationBlue + ((float)(Color.Default.MousePrimaryBlue - Color.Default.MouseTertiaryBlue) / 400);

                    if (_flMouseAnimationBlue >= Color.Default.MousePrimaryBlue) _flMouseAnimationBlue = Color.Default.MousePrimaryBlue;
                }
                else if (Color.Default.MousePrimaryBlue < Color.Default.MouseTertiaryBlue)
                {
                    _flMouseAnimationBlue = _flMouseAnimationBlue - ((float)(Color.Default.MouseTertiaryBlue - Color.Default.MousePrimaryBlue) / 400);

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
                        _flMouseAnimationRed = _flMouseAnimationRed + ((float)(Color.Default.MouseSecondaryRed - Color.Default.MousePrimaryRed) / 238);

                        if (_flMouseAnimationRed >= Color.Default.MouseSecondaryRed) _flMouseAnimationRed = Color.Default.MouseSecondaryRed;
                    }
                    else if (Color.Default.MouseSecondaryRed < Color.Default.MousePrimaryRed)
                    {
                        _flMouseAnimationRed = _flMouseAnimationRed - ((float)(Color.Default.MousePrimaryRed - Color.Default.MouseSecondaryRed) / 238);

                        if (_flMouseAnimationRed <= Color.Default.MouseSecondaryRed) _flMouseAnimationRed = Color.Default.MouseSecondaryRed;
                    }

                    if (Color.Default.MouseSecondaryGreen > Color.Default.MousePrimaryGreen)
                    {
                        _flMouseAnimationGreen = _flMouseAnimationGreen + ((float)(Color.Default.MouseSecondaryGreen - Color.Default.MousePrimaryGreen) / 238);

                        if (_flMouseAnimationGreen >= Color.Default.MouseSecondaryGreen) _flMouseAnimationGreen = Color.Default.MouseSecondaryGreen;
                    }
                    else if (Color.Default.MouseSecondaryGreen < Color.Default.MousePrimaryGreen)
                    {
                        _flMouseAnimationGreen = _flMouseAnimationGreen - ((float)(Color.Default.MousePrimaryGreen - Color.Default.MouseSecondaryGreen) / 238);

                        if (_flMouseAnimationGreen <= Color.Default.MouseSecondaryGreen) _flMouseAnimationGreen = Color.Default.MouseSecondaryGreen;
                    }

                    if (Color.Default.MouseSecondaryBlue > Color.Default.MousePrimaryBlue)
                    {
                        _flMouseAnimationBlue = _flMouseAnimationBlue + ((float)(Color.Default.MouseSecondaryBlue - Color.Default.MousePrimaryBlue) / 238);

                        if (_flMouseAnimationBlue >= Color.Default.MouseSecondaryBlue) _flMouseAnimationBlue = Color.Default.MouseSecondaryBlue;
                    }
                    else if (Color.Default.MouseSecondaryBlue < Color.Default.MousePrimaryBlue)
                    {
                        _flMouseAnimationBlue = _flMouseAnimationBlue - ((float)(Color.Default.MousePrimaryBlue - Color.Default.MouseSecondaryBlue) / 238);

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
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.S] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.A] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.CapsLock] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemBackslash] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemApostrophe] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemSemicolon] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.L] = _keyboardPrimaryColor;
                        break;
                    case 2:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.F] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.S] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.A] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.CapsLock] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemBackslash] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemApostrophe] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemSemicolon] = _keyboardPrimaryColor;
                        break;
                    case 3:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.T] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.F] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.S] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.A] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.CapsLock] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemBackslash] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemApostrophe] = _keyboardPrimaryColor;
                        break;
                    case 4:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D6] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.T] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.F] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.S] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.A] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.CapsLock] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemBackslash] = _keyboardPrimaryColor;
                        break;
                    case 5:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.Z] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D6] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.T] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.F] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.S] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.A] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.CapsLock] = _keyboardPrimaryColor;
                        break;
                    case 6:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.H] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.Z] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D6] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.T] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.F] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.S] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.A] = _keyboardPrimaryColor;
                        break;
                    case 7:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.N] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.H] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.Z] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D6] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.T] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.F] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.S] = _keyboardPrimaryColor;
                        break;
                    case 8:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.J] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.N] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.H] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.Z] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D6] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.T] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.F] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D] = _keyboardPrimaryColor;
                        break;
                    case 9:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.K] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.J] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.N] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.H] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.Z] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D6] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.T] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.F] = _keyboardPrimaryColor;
                        break;
                    case 10:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.L] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.K] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.J] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.N] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.H] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.Z] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D6] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.T] = _keyboardPrimaryColor;
                        break;
                    case 11:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemSemicolon] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.L] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.K] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.J] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.N] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.H] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.Z] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.D6] = _keyboardPrimaryColor;
                        break;
                    case 12:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemApostrophe] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemSemicolon] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.L] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.K] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.J] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.N] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.H] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.Z] = _keyboardPrimaryColor;
                        break;
                    case 13:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemBackslash] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemApostrophe] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemSemicolon] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.L] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.K] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.J] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.N] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.H] = _keyboardPrimaryColor;
                        break;
                    case 14:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.CapsLock] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemBackslash] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemApostrophe] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemSemicolon] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.L] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.K] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.J] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.N] = _keyboardPrimaryColor;
                        break;
                    case 15:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.A] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.CapsLock] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemBackslash] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemApostrophe] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemSemicolon] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.L] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.K] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.J] = _keyboardPrimaryColor;
                        break;
                    case 16:
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.S] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.A] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.CapsLock] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemBackslash] = _keyboardSecondaryColor;
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemApostrophe] = CalculateKeyboardWaveColors(1);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.OemSemicolon] = CalculateKeyboardWaveColors(2);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.L] = CalculateKeyboardWaveColors(3);
                        _customKeyboard[Corale.Colore.Razer.Keyboard.Key.K] = _keyboardPrimaryColor;
                        break;
                }
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
                MainForm.KeyboardKeyReleased = Corale.Colore.Razer.Keyboard.Key.Logo;
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
