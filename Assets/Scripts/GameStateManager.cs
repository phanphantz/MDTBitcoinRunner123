using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

    public static class GameStateManager
    {
        public static bool IsPause => isPause;
        private static bool isPause;

        public static void SetIsPause(bool value)
        {
            isPause = value;
        }

    }
