using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MetalGearLiquid.Model;
using MetalGearLiquid.Persistence;
using MetalGearLiquid.ViewModel;
using MetalGearLiquid.View;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Input;

namespace MetalGearLiquid
{

    public partial class App : Application
    {
        #region Fields

        private MetalGearLiquidModel _model;
        private MetalGearLiquidViewModel _viewModel;
        private MainWindow _view;
        private DispatcherTimer _timer;
        private Boolean timerStarted = false;
        private Boolean isGameOver = false;

        #endregion

        #region Constructon
        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        #endregion

        #region Application event handlers
        private void App_Startup(object sender, StartupEventArgs e)
        {
            _model = new MetalGearLiquidModel(new MetalGearLiquidFileDataAccess());
            _model.GameOver += new EventHandler<MetalGearLiquidEventArgs>(Model_GameOver);

            _viewModel = new MetalGearLiquidViewModel(_model);
            _viewModel.NewGameSmall += new EventHandler(ViewModel_NewGameSmall);
            _viewModel.NewGameMedium += new EventHandler(ViewModel_NewGameMedium);
            _viewModel.NewGameBig += new EventHandler(ViewModel_NewGameBig);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.PauseGame += new EventHandler(VieModel_PauseGame);
            _viewModel.PlayerStepUp += new EventHandler(ViewModel_PlayerStepUp);
            _viewModel.PlayerStepDown += new EventHandler(ViewModel_PlayerStepDown);
            _viewModel.PlayerStepRight += new EventHandler(ViewModel_PlayerStepRight);
            _viewModel.PlayerStepLeft += new EventHandler(ViewModel_PlayerStepLeft);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing);
            _view.Show();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(Timer_Tick);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _model.AdvanceTime();
        }

        #endregion

        #region View event handlers

        private void View_Closing(object sender, CancelEventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Sudoku", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true; // töröljük a bezárást

                if (restartTimer) // ha szükséges, elindítjuk az időzítőt
                    _timer.Start();
            }
        }

        #endregion

        #region ViewModel event handlers

        private async void ViewModel_NewGameSmall(object sender, EventArgs e)
        {
            String path = "..\\..\\..\\Maps\\map1.txt";
            await _model.LoadGameAsync(path);
            _viewModel.GenerateTable();
            isGameOver = false;
            if (timerStarted) {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                timerStarted = true;
            }
            _timer.Start();
        }

        private async void ViewModel_NewGameMedium(object sender, EventArgs e)
        {
            String path = "..\\..\\..\\Maps\\map2.txt";
            await _model.LoadGameAsync(path);
            _viewModel.GenerateTable();
            isGameOver = false;
            if (timerStarted)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                timerStarted = true;
            }
            _timer.Start();
        }

        private async void ViewModel_NewGameBig(object sender, EventArgs e)
        {
            String path = "..\\..\\..\\Maps\\map3.txt";
            await _model.LoadGameAsync(path);
            _viewModel.GenerateTable();
            isGameOver = false;
            if (timerStarted)
            {
                _timer = new DispatcherTimer();
                _timer.Interval = TimeSpan.FromSeconds(1);
                timerStarted = true;
            }
            _timer.Start();
        }

        private void VieModel_PauseGame(object sender, EventArgs e)
        {
            if (!isGameOver)
            {
                if (_viewModel.Paused)
                    _timer.Start();
                else
                    _timer.Stop();
                _viewModel.PauseTheGame();
            }
        }

        private void ViewModel_PlayerStepUp(object sender, EventArgs e)
        {
            _viewModel.Navigate_KeyDown('w');
        }

        private void ViewModel_PlayerStepDown(object sender, EventArgs e)
        {
            _viewModel.Navigate_KeyDown('s');
        }

        private void ViewModel_PlayerStepRight(object sender, EventArgs e)
        {
            _viewModel.Navigate_KeyDown('d');
        }

        private void ViewModel_PlayerStepLeft(object sender, EventArgs e)
        {
            _viewModel.Navigate_KeyDown('a');
        }

        private void ViewModel_ExitGame(object sender, System.EventArgs e)
        {
            _view.Close(); // ablak bezárása
        }

        #endregion

        #region Model event handlers

        private void Model_GameOver(object sender, MetalGearLiquidEventArgs e)
        {
            _timer.Stop();
            char key = e.LastStep;
            switch (key)
            {
                case 'w':
                    _viewModel.Fields[((_model._table.Snek.x + 1) * _model._table.TableSize.y) + (_model._table.Snek.y)].Field = Persistence.FieldType.Floor;
                    _viewModel.Fields[_model._table.Snek.x * _model._table.TableSize.y + _model._table.Snek.y].Field = Persistence.FieldType.Player;
                    break;
                case 's':
                    _viewModel.Fields[((_model._table.Snek.x - 1) * _model._table.TableSize.y) + (_model._table.Snek.y)].Field = Persistence.FieldType.Floor;
                    _viewModel.Fields[_model._table.Snek.x * _model._table.TableSize.y + _model._table.Snek.y].Field = Persistence.FieldType.Player;
                    break;
                case 'a':
                    _viewModel.Fields[((_model._table.Snek.x) * _model._table.TableSize.y) + (_model._table.Snek.y + 1)].Field = Persistence.FieldType.Floor;
                    _viewModel.Fields[_model._table.Snek.x * _model._table.TableSize.y + _model._table.Snek.y].Field = Persistence.FieldType.Player;
                    break;
                case 'd':
                    _viewModel.Fields[((_model._table.Snek.x) * _model._table.TableSize.y) + (_model._table.Snek.y - 1)].Field = Persistence.FieldType.Floor;
                    _viewModel.Fields[_model._table.Snek.x * _model._table.TableSize.y + _model._table.Snek.y].Field = Persistence.FieldType.Player;
                    break;
            }
            if (_model._table.Snek.x < 1 || _model._table.Snek.y < 1 || _model._table.Snek.x > _model._table.TableSize.x - 2 || _model._table.Snek.y > _model._table.TableSize.y - 2)
            {
                MessageBox.Show("Excelent Snake!",
                                "Metal Gear Liquid");
            }
            else
            {
                MessageBox.Show("What's wrong? Snake? SNAAAAAAAAAAAKE?!",
                                "Metal Gear Liquid");
            }
            isGameOver = true;
        }

        #endregion
    }
}
