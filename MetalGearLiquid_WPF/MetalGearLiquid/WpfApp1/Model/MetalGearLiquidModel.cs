using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using MetalGearLiquid.Persistence;
using System.Threading.Tasks;

namespace MetalGearLiquid.Model
{
    
    public class MetalGearLiquidModel
    {
        #region FIelds

        public MetalGearLiquidTable _table;
        private MetalGearLiquidDataAccess _dataAccess;
        public int time = 0;

        #endregion

        #region Event Handlers

        public event EventHandler<MetalGearLiquidEventArgs> GameAdvanced;
        public event EventHandler<MetalGearLiquidEventArgs> GameOver;
        public event EventHandler<MetalGearLiquidEventArgs> ClearSpotPoints;

        #endregion

        #region Constructor
        public MetalGearLiquidModel(MetalGearLiquidDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        #endregion

        #region Game Logic Functions

        public Pair step(char dir, FieldType _fieldType, Int32 x, Int32 y)
        {
            Pair figure = new Pair(x, y);
            if(dir == 'w')
            {
                if(_table[figure.x-1, figure.y] == FieldType.Floor || (_fieldType == FieldType.Player && _table[figure.x - 1, figure.y] == FieldType.Exit)) {
                    _table[figure.x, figure.y] = FieldType.Floor;
                    _table[figure.x - 1, figure.y] = _fieldType;
                    figure.x--;
                    if(_fieldType == FieldType.Player)
                    {
                        _table.Snek = figure;
                        if (isCaught() || gotOut())
                        {
                            if (GameOver != null)
                                GameOver(this, new MetalGearLiquidEventArgs(true, time, 'w'));
                        }
                    }
                    return figure;
                }
                else { return new Pair(-1, -1); }
            }
            else if (dir == 's')
            {
                if (_table[figure.x + 1, figure.y] == FieldType.Floor || (_fieldType == FieldType.Player && _table[figure.x + 1, figure.y] == FieldType.Exit))
                {
                    _table[figure.x, figure.y] = FieldType.Floor;
                    _table[figure.x + 1, figure.y] = _fieldType;
                    figure.x++;
                    if (_fieldType == FieldType.Player)
                    {
                        _table.Snek = figure;
                        if (isCaught() || gotOut())
                        {
                            if (GameOver != null)
                                GameOver(this, new MetalGearLiquidEventArgs(true, time, 's'));
                        }
                    }
                    return figure;
                }
                else { return new Pair(-1, -1); }
            }
            else if (dir == 'a')
            {
                if (_table[figure.x, figure.y - 1] == FieldType.Floor || (_fieldType == FieldType.Player && _table[figure.x, figure.y - 1] == FieldType.Exit))
                {
                    _table[figure.x, figure.y] = FieldType.Floor;
                    _table[figure.x, figure.y - 1] = _fieldType;
                    figure.y--;
                    if (_fieldType == FieldType.Player)
                    {
                        _table.Snek = figure;
                        if (isCaught() || gotOut())
                        {
                            if (GameOver != null)
                                GameOver(this, new MetalGearLiquidEventArgs(true, time, 'a'));
                        }
                    }
                    return figure;
                }
                else { return new Pair(-1, -1); }
            }
            else if (dir == 'd')
            {
                if (_table[figure.x, figure.y + 1] == FieldType.Floor || (_fieldType == FieldType.Player && _table[figure.x, figure.y + 1] == FieldType.Exit))
                {
                    _table[figure.x, figure.y] = FieldType.Floor;
                    _table[figure.x, figure.y + 1] = _fieldType;
                    figure.y++;
                    if (_fieldType == FieldType.Player)
                    {
                        _table.Snek = figure;
                        if (isCaught() || gotOut())
                        {
                            if (GameOver != null)
                                GameOver(this, new MetalGearLiquidEventArgs(true, time, 'd'));
                        }
                    }
                    return figure;
                }
                else { return new Pair(-1, -1); }
            }
            return new Pair(-1, -1);
        }

        public void CalculateSpotPoints()
        {
            Boolean partOfSpotFields;
            _table.spotPointsCounter = 0;
            _table.spotPoints = new Pair[_table.maxGuards * 24];
            int m;
            int n;
            foreach (Guard guard in _table.Guards){
                for(int i=guard.pos.x-2; i<=guard.pos.x+2; i++)
                {
                    for(int j=guard.pos.y-2; j<=guard.pos.y+2; j++)
                    {
                        partOfSpotFields = true;
                        if (i < 1 || j < 1 || i > _table.TableSize.x-2 || j > _table.TableSize.y-2) { }
                        else
                        {
                            for(int k=0; k<= Math.Abs(i - guard.pos.x); k++)
                            {
                                for(int l=0; l<= Math.Abs(j - guard.pos.y); l++)
                                {
                                    if (i - guard.pos.x > 0)
                                        m = k * -1;
                                    else
                                        m = k;
                                    if (j - guard.pos.y > 0)
                                        n = l * -1;
                                    else
                                        n = l;
                                    if (_table[i + m, j + n] == FieldType.Wall || (i == 0 && j == 0))
                                    {
                                        partOfSpotFields = false;
                                    }
                                }
                            }
                            if (partOfSpotFields)
                            {
                                _table.spotPoints[_table.spotPointsCounter] = new Pair(i, j);
                                _table.spotPointsCounter++;
                            }
                        }
                    }
                }
            }
        }

        public void GuardsStep()
        {
            var rand = new Random();
            for (int i=0; i<_table.maxGuards; i++)
            {
                Int32 newDirection;
                Pair newPos = step(_table.Guards[i].faceTow, FieldType.Guard, _table.Guards[i].pos.x, _table.Guards[i].pos.y);
                while (-1 == newPos.x)
                {
                    newDirection = rand.Next(5);
                    switch (newDirection)
                    {
                        case 0:
                            _table.Guards[i].faceTow = 'w';
                            break;
                        case 1:
                            _table.Guards[i].faceTow = 's';
                            break;
                        case 2:
                            _table.Guards[i].faceTow = 'a';
                            break;
                        case 3:
                            _table.Guards[i].faceTow = 'd';
                            break;
                    }
                    newPos = step(_table.Guards[i].faceTow, FieldType.Guard, _table.Guards[i].pos.x, _table.Guards[i].pos.y);
                }
                _table.Guards[i].pos = newPos;
            }
        }

        public void AdvanceTime()
        {
            if (ClearSpotPoints != null)
                ClearSpotPoints(this, new MetalGearLiquidEventArgs(false, time, 'n'));
            GuardsStep();
            CalculateSpotPoints();
            time++;
            Debug.WriteLine(time.ToString());
            if (GameAdvanced != null)
                GameAdvanced(this, new MetalGearLiquidEventArgs(false, time, 'n'));
            if (isCaught())
            {
                if (GameOver != null)
                    GameOver(this, new MetalGearLiquidEventArgs(true, time, 'n'));
            }
        }

        public Boolean gotOut()
        {
            if (_table.Snek.x < 1 || _table.Snek.y < 1 || _table.Snek.x > _table.TableSize.x - 2 || _table.Snek.y > _table.TableSize.y - 2)
            {
                return true;
            }
            return false;
        }
        public Boolean isCaught()
        {
            foreach(Pair spot in _table.spotPoints)
            {
                if(spot.Equals(_table.Snek))
                    return true;
            }
            return false;
        }

        public async Task LoadGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            _table = await _dataAccess.LoadAsync(path);
            time = _table._elapsedTime;
        }

        #endregion
    };
}
