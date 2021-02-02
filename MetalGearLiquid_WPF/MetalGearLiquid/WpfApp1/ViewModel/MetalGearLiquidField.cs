using System;
using System.Collections.Generic;
using System.Text;

namespace MetalGearLiquid.ViewModel
{
    class MetalGearLiquidField : ViewModelBase
    {
        private Persistence.FieldType _field;
        private Boolean isSpotPoint;

        public Persistence.FieldType Field
        {
            get { return _field; }
            set
            {
                if (_field != value)
                {
                    _field = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsSpotPoint
        {
            get { return isSpotPoint; }
            set
            {
                if( isSpotPoint != value)
                {
                    isSpotPoint = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Vízszintes koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 X { get; set; }

        /// <summary>
        /// Függőleges koordináta lekérdezése, vagy beállítása.
        /// </summary>
        public Int32 Y { get; set; }

        /// <summary>
        /// Sorszám lekérdezése.
        /// </summary>
        public Int32 Number { get; set; }
    }
}
