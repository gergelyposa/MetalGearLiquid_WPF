using System;
using System.Collections.Generic;
using System.Text;

namespace MetalGearLiquid.Model
{

    /// <summary>
    /// MetalGearLiquid eseményargumentum típusa
    /// </summary>
    public class MetalGearLiquidEventArgs : EventArgs
    {
        private Int32 _time;
        private Boolean _isWon;
        private char _lastStep;

        /// <summary>
        /// Idő lekérdezése.
        /// </summary>
        public Int32 Time { get { return _time; } }

        /// <summary>
        /// Győzelem lekérdezése.
        /// </summary>
        public Boolean IsWon { get { return _isWon; } }

        /// <summary>
        /// Utolsó lépés irányának lekérdezése, az utolsó lépés utáni szép megjelenés érdekében.
        /// </summary>
        public char LastStep { get { return _lastStep; } }

        public MetalGearLiquidEventArgs(Boolean IsWon, Int32 Time, char Step)
        {
            _isWon = IsWon;
            _time = Time;
            _lastStep = Step;
        }
    }
}
