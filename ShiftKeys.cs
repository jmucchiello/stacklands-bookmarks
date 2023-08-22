using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BookmarksModNS
{
    internal class ShiftKeys
    {
        public enum Result { None, Meta, OtherMetas };
        public enum Mode { Shift, Ctrl, Alt };

        public static Mode defaultMode = Mode.Shift;

        public static Result TestShiftStatus(InputController controller)
        {
            return GetTest(defaultMode)(controller);
        }

        private static Func<InputController, Result> pcShiftCheck = delegate (InputController controller) {
            if (controller.GetKey(Key.LeftAlt) || controller.GetKey(Key.RightAlt) ||
                controller.GetKey(Key.LeftWindows) || controller.GetKey(Key.RightWindows) ||
                controller.GetKey(Key.LeftCtrl) || controller.GetKey(Key.RightCtrl)) return Result.OtherMetas;
            if (controller.GetKey(Key.LeftShift) || controller.GetKey(Key.RightShift)) return Result.Meta;
            return Result.None;
        };

        private static Func<InputController, Result> macShiftCheck = delegate (InputController controller) {
            if (controller.GetKey(Key.LeftApple) || controller.GetKey(Key.RightApple) ||
                controller.GetKey(Key.LeftCtrl) || controller.GetKey(Key.RightCtrl) ||
                controller.GetKey(Key.LeftCommand) || controller.GetKey(Key.RightCommand)) return Result.OtherMetas;
            if (controller.GetKey(Key.LeftShift) || controller.GetKey(Key.RightShift)) return Result.Meta;
            return Result.None;
        };

        private static Func<InputController, Result> pcControlCheck = delegate (InputController controller) {
            if (controller.GetKey(Key.LeftAlt) || controller.GetKey(Key.RightAlt) ||
                controller.GetKey(Key.LeftWindows) || controller.GetKey(Key.RightWindows) ||
                controller.GetKey(Key.LeftShift) || controller.GetKey(Key.RightShift)) return Result.OtherMetas;
            if (controller.GetKey(Key.LeftCtrl) || controller.GetKey(Key.RightCtrl)) return Result.Meta;
            return Result.None;
        };

        private static Func<InputController, Result> macCommandCheck = delegate(InputController controller)  {
            if (controller.GetKey(Key.LeftApple) || controller.GetKey(Key.RightApple) ||
                controller.GetKey(Key.LeftCtrl) || controller.GetKey(Key.RightCtrl) ||
                controller.GetKey(Key.LeftShift) || controller.GetKey(Key.RightShift)) return Result.OtherMetas;
            if (controller.GetKey(Key.LeftCommand) || controller.GetKey(Key.RightCommand)) return Result.Meta;
            return Result.None;
        };

        private static Func<InputController, Result> cachedFunc;
        private static Mode cachedMode;

        private static Func<InputController, Result> GetTest(Mode mode)
        {
            if (cachedFunc == null || cachedMode != mode)
            {
                RuntimePlatform platform = Application.platform;
                cachedFunc = mode switch
                {
                    Mode.Shift => platform == RuntimePlatform.WindowsPlayer ? pcShiftCheck : macShiftCheck,
                    Mode.Ctrl => platform == RuntimePlatform.WindowsPlayer ? pcControlCheck : macCommandCheck,
                    _ => pcShiftCheck,
                };
                cachedMode = mode;
//                BookmarksMod.instance.Log($"ShiftKeys set {cachedMode} {cachedFunc}");
            }
            return cachedFunc;
        }

        public static void Reset() { cachedFunc = null; }
    }
}
