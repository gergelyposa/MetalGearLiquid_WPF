using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using MetalGearLiquid.Model;

namespace MetalGearLiquid.ViewModel
{
    class MetalGearLiquidViewModel : ViewModelBase
    {
        #region Fields

        private MetalGearLiquidModel _model;
        private Boolean _paused;
        private Int32 _width;
        private Int32 _height;

        public Boolean Paused
        {
            get { return _paused; }
        }
        public Int32 Height
        {
            get { return _height; }
            set
            {
                if(_height != value)
                {
                    _height = value;
                    OnPropertyChanged();
                }
            }
        }

        public Int32 Width
        {
            get { return _width; }
            set
            {
                if (_width != value)
                {
                    _width = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Új játék kezdése parancs lekérdezése.
        /// </summary>
        public DelegateCommand NewGameSmallCommand { get; private set; }

        public DelegateCommand NewGameMediumCommand { get; private set; }

        public DelegateCommand NewGameBigCommand { get; private set; }

        /// <summary>
        /// A játékos léptetés parancsainak lekérdezése.
        /// </summary>
        public DelegateCommand PlayerStepUpCommand { get; private set; }

        public DelegateCommand PlayerStepDownCommand { get; private set; }

        public DelegateCommand PlayerStepRightCommand { get; private set; }

        public DelegateCommand PlayerStepLeftCommand { get; private set; }

        /// <summary>
        /// A játék szüneteltetése parancs lekérdezése.
        /// </summary>
        public DelegateCommand PauseGameCommand { get; private set; }

        /// <summary>
        /// Kilépés parancs lekérdezése.
        /// </summary>
        public DelegateCommand ExitCommand { get; private set; }

        /// <summary>
        /// Játékmező gyűjtemény lekérdezése.
        /// </summary>
        public ObservableCollection<MetalGearLiquidField> Fields { get; set; }

        /// <summary>
        /// Játékidő lekérdezése.
        /// </summary>
        public String GameTime { get { return TimeSpan.FromSeconds(_model.time).ToString("g"); } }

        #endregion

        #region Events

        /// <summary>
        /// Új játék eseménye.
        /// </summary>
        public event EventHandler NewGameSmall;

        public event EventHandler NewGameMedium;

        public event EventHandler NewGameBig;

        /// <summary>
        /// Játék szüneteltetésének eseménye.
        /// </summary>
        public event EventHandler PauseGame;

        /// <summary>
        /// Játkos léptetéseinek eseménye.
        /// </summary>
        public event EventHandler PlayerStepUp;

        public event EventHandler PlayerStepDown;

        public event EventHandler PlayerStepRight;

        public event EventHandler PlayerStepLeft;

        /// <summary>
        /// Játékból való kilépés eseménye.
        /// </summary>
        public event EventHandler ExitGame;

        #endregion

        #region Constructors

        public MetalGearLiquidViewModel(MetalGearLiquidModel model)
        {
            _model = model;
            _model.GameAdvanced += new EventHandler<MetalGearLiquidEventArgs>(GameAdvanced);
            _model.GameOver += new EventHandler<MetalGearLiquidEventArgs>(Model_GameOver);

            NewGameSmallCommand = new DelegateCommand(param => OnNewGameSmall());
            NewGameMediumCommand = new DelegateCommand(param => OnNewGameMedium());
            NewGameBigCommand = new DelegateCommand(param => OnNewGameBig());
             

            PauseGameCommand = new DelegateCommand(param => OnGamePause());

            PlayerStepUpCommand = new DelegateCommand(param => OnPlayerStepUp());
            PlayerStepDownCommand = new DelegateCommand(param => OnPlayerStepDown());
            PlayerStepRightCommand = new DelegateCommand(param => OnPlayerStepRight());
            PlayerStepLeftCommand = new DelegateCommand(param => OnPlayerStepLeft());

            ExitCommand = new DelegateCommand(param => OnExitGame());

            _paused = false;
        }

        #endregion

        #region Public Methods

        public void GenerateTable()
        {
            // játéktábla létrehozása
            Fields = new ObservableCollection<MetalGearLiquidField>();
            for (Int32 i = 0; i < _model._table.TableSize.x; i++)  // inicializáljuk a mezőket
            {
                for (Int32 j = 0; j < _model._table.TableSize.y; j++)
                {
                    Fields.Add(new MetalGearLiquidField
                    {
                        Field = _model._table[i, j],
                        IsSpotPoint = false,
                        X = i,
                        Y = j,
                        Number = i * _model._table.TableSize.y + j // a mező sorszáma, amelyet felhasználunk az azonosításhoz
                    });
                }
            }
            Height = _model._table.TableSize.x;
            Width = _model._table.TableSize.y;
            _paused = false;
            OnPropertyChanged("GameTime");
            OnPropertyChanged("Fields");
        }

        public void PauseTheGame()
        {
            if (_paused)
                _paused = false;
            else
                _paused = true;
        }

        public void Navigate_KeyDown(char e)
        {
            if (!_paused)
            {
                int u = 0;
                int s = 0;
                switch (e)
                {
                    case 'w':
                        u = 1;
                        break;
                    case 's':
                        u = -1;
                        break;
                    case 'a':
                        s = 1;
                        break;
                    case 'd':
                        s = -1;
                        break;
                }
                if (_model.step(e, Persistence.FieldType.Player, _model._table.Snek.x, _model._table.Snek.y).x != -1)
                {
                    Fields[((_model._table.Snek.x + u) * _model._table.TableSize.y) + (_model._table.Snek.y + s)].Field = Persistence.FieldType.Floor;
                    Fields[_model._table.Snek.x * _model._table.TableSize.y + _model._table.Snek.y].Field = Persistence.FieldType.Player;
                    
                }
            }
            RefreshTable();
        }

        #endregion

        #region Private methods

        private void RefreshTable()
        {
            foreach(MetalGearLiquidField field in Fields)
            {
                field.Field = _model._table[field.X, field.Y];
                foreach(Persistence.Pair spot in _model._table.spotPoints)
                {
                    if (field.X == spot.x && field.Y == spot.y)
                        field.IsSpotPoint = true;
                }
            }
            OnPropertyChanged("GameTime");
        }

        #endregion

        #region Game event handlers

        private void GameAdvanced(Object sender, MetalGearLiquidEventArgs e)
        {
            foreach (MetalGearLiquidField field in Fields)
            {
                field.IsSpotPoint = false;
                field.Field = _model._table[field.X, field.Y];
                foreach (Persistence.Pair spot in _model._table.spotPoints)
                {
                    if (field.X == spot.x && field.Y == spot.y)
                        field.IsSpotPoint = true;
                }
            }
            for (int i = 0; i < _model._table.maxGuards; i++)
            {
                int u = 0;
                int s = 0;
                switch (_model._table.Guards[i].faceTow)
                {
                    case 'w':
                        u = 1;
                        break;
                    case 's':
                        u = -1;
                        break;
                    case 'a':
                        s = 1;
                        break;
                    case 'd':
                        s = -1;
                        break;
                }
                Fields[(_model._table.Guards[i].pos.x * _model._table.TableSize.y) + _model._table.Guards[i].pos.y].Field = Persistence.FieldType.Guard;
                Fields[((_model._table.Guards[i].pos.x + u) * _model._table.TableSize.y) + (_model._table.Guards[i].pos.y + s)].Field = Persistence.FieldType.Floor;
            }
            OnPropertyChanged("Fields");
            OnPropertyChanged("GameTime");
        }

        private void Table_Created(object sender, MetalGearLiquidEventArgs e)
        {
            RefreshTable();
        }
        private void Model_GameOver(object sender, MetalGearLiquidEventArgs e)
        {
            _paused = true;
        }
        #endregion

        #region Event Methods

        private void OnNewGameSmall()
        {
            if (NewGameSmall != null)
                NewGameSmall(this, EventArgs.Empty);
        }

        private void OnNewGameMedium()
        {
            if (NewGameMedium != null)
                NewGameMedium(this, EventArgs.Empty);
        }

        private void OnNewGameBig()
        {
            if (NewGameBig != null)
                NewGameBig(this, EventArgs.Empty);
        }

        private void OnGamePause()
        {
            if (PauseGame != null)
                PauseGame(this, EventArgs.Empty);
        }

        private void OnPlayerStepUp()
        {
            if (NewGameBig != null)
                PlayerStepUp(this, EventArgs.Empty);
        }

        private void OnPlayerStepDown()
        {
            if (NewGameBig != null)
                PlayerStepDown(this, EventArgs.Empty);
        }

        private void OnPlayerStepRight()
        {
            if (NewGameBig != null)
                PlayerStepRight(this, EventArgs.Empty);
        }

        private void OnPlayerStepLeft()
        {
            if (NewGameBig != null)
                PlayerStepLeft(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            if (ExitGame != null)
                ExitGame(this, EventArgs.Empty);
        }

        #endregion
    }
}
