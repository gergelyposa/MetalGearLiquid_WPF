using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetalGearLiquid.Persistence
{
    #region Structs
    public struct Pair
    {
        public Int32 x;
        public Int32 y;

        public Pair(Int32 _x, Int32 _y)
        {
            x = _x;
            y = _y;
        }
    };

    public struct Guard
    {
        public Pair pos;
        public char faceTow;

        public Guard(Pair p, char c)
        {
            pos = p;
            faceTow = c;
        }
    }
    public struct Field
    {
        public Int32 _x;
        public Int32 _y;
        public FieldType _type;

        public Field(Int32 x, Int32 y, FieldType type){
            _x = x;
            _y = y;
            _type = type;
        }
    }

    #endregion

    #region ENUM
    public enum FieldType { Player, Guard, Wall, Floor, Exit }

    #endregion
    public class MetalGearLiquidTable
    {
        #region Fields

        public Pair Snek;
        public Guard[] Guards;
        public Pair[] spotPoints;
        public int spotPointsCounter = 0;
        public int maxGuards;
        private Pair _tableSize;
        private Field[,] _fieldValues;
        public Int32 _elapsedTime = 0;

        #endregion

        #region Properties

        public Pair TableSize { get { return _tableSize; } set { _tableSize = value; } }
        public FieldType this[Int32 x, Int32 y] { get { return GetValue(x, y); } set { SetValue(x, y, value); } }

        #endregion

        #region Constructor

        public  MetalGearLiquidTable(Pair tableSize)
        {
            if (tableSize.x < 0 || tableSize.y < 0)
                throw new ArgumentOutOfRangeException("The table size is smaller than 0", "tableSize");

            _tableSize = tableSize;
            _fieldValues = new Field[tableSize.x, tableSize.y];
            _elapsedTime = 0;
            for (Int32 i = 0; i < TableSize.x; i++)
            {
                for (Int32 j = 0; j < TableSize.y; j++)
                {
                    SetValue(i, j, FieldType.Floor);
                }
            }

        }

        #endregion

        #region Public Functions
        public Int32 GetNumericFieldType(FieldType _Field)
        {
            if (_Field == FieldType.Player)
                return 0;
            else if (_Field == FieldType.Guard)
                return 1;
            else if (_Field == FieldType.Wall)
                return 2;
            else if (_Field == FieldType.Floor)
                return 3;
            else
                return 4;
        }
        public FieldType GetFieldTypeFromNumeric(Int32 _Field)
        {
            if (_Field == 0)
                return FieldType.Player;
            else if (_Field == 1)
                return FieldType.Guard;
            else if (_Field == 2)
                return FieldType.Wall;
            else if (_Field == 3)
                return FieldType.Floor;
            else
                return FieldType.Exit;
        }

        

        public void SetValue(Int32 x, Int32 y, FieldType value)
        {
            if (x < 0 || x >= _fieldValues.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= _fieldValues.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");
            _fieldValues[x, y] = new Field(x,y,value);
        }

        public FieldType GetValue(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fieldValues.GetLength(0))
                throw new ArgumentOutOfRangeException("x", "The X coordinate is out of range.");
            if (y < 0 || y >= _fieldValues.GetLength(1))
                throw new ArgumentOutOfRangeException("y", "The Y coordinate is out of range.");

            return _fieldValues[x, y]._type;
        }

        #endregion
    }
}
