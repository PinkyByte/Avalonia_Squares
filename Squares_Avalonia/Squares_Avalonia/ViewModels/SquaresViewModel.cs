using System;
using Avalonia.Media;
using Squares.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace Squares_Avalonia.ViewModels
{
    public class SquaresViewModel : ViewModelBase
    {
        #region Fields

        private SquaresModel _model;

        #endregion

        #region Properties

        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public ObservableCollection<SquaresField> HorFields { get; private set; }
        public ObservableCollection<SquaresField> VerFields { get; private set; }
        public int PlayerOneScore { get; private set; }
        public int PlayerTwoScore { get; private set; }
        public bool IsGameSmall
        {
            get { return _model.GameSize == SquaresModel.GameSizeSmall; }
            set
            {
                if (_model.GameSize == SquaresModel.GameSizeSmall)
                {
                    return;
                }

                _model.GameSize = SquaresModel.GameSizeSmall;
                OnPropertyChanged(nameof(IsGameSmall));
                OnPropertyChanged(nameof(IsGameMedium));
                OnPropertyChanged(nameof(IsGameLarge));
            }
        }
        public bool IsGameMedium
        {
            get { return _model.GameSize == SquaresModel.GameSizeMedium; }
            set
            {
                if (_model.GameSize == SquaresModel.GameSizeMedium)
                {
                    return;
                }

                _model.GameSize = SquaresModel.GameSizeMedium;
                OnPropertyChanged(nameof(IsGameSmall));
                OnPropertyChanged(nameof(IsGameMedium));
                OnPropertyChanged(nameof(IsGameLarge));
            }
        }
        public bool IsGameLarge
        {
            get { return _model.GameSize == SquaresModel.GameSizeLarge; }
            set
            {
                if (_model.GameSize == SquaresModel.GameSizeLarge)
                {
                    return;
                }

                _model.GameSize = SquaresModel.GameSizeLarge;
                OnPropertyChanged(nameof(IsGameSmall));
                OnPropertyChanged(nameof(IsGameMedium));
                OnPropertyChanged(nameof(IsGameLarge));
            }
        }

        #endregion

        #region Events

        public event EventHandler? NewGame;
        public event EventHandler? SaveGame;
        public event EventHandler? LoadGame;

        #endregion

        #region Constructor

        public SquaresViewModel(SquaresModel model)
        {
            _model = model;
            _model.TableChange += new EventHandler<TableEventArgs>(Model_TableChange);
            _model.ScoreIncrease += new EventHandler<PlayerEventArgs>(Model_ScoreIncrease);

            NewGameCommand = new DelegateCommand(param => OnNewGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());

            VerFields = new ObservableCollection<SquaresField>();
            HorFields = new ObservableCollection<SquaresField>();
            CreateTable();
        }

        #endregion

        #region Public methods

        public void CreateTable()
        {
            PlayerOneScore = _model.PlayerOneScore();
            PlayerTwoScore = _model.PlayerTwoScore();

            OnPropertyChanged(nameof(PlayerOneScore));
            OnPropertyChanged(nameof(PlayerTwoScore));
            for (int i = 0; i < _model.Table.Columns.GetLength(0); i++)
            {
                for (int j = 0; j < _model.Table.Columns.GetLength(1); j++)
                {
                    VerFields.Add(new SquaresField
                    {
                        IsDrawn = _model.Table.Columns[i, j],
                        IsEnabled = !_model.Table.Columns[i, j],
                        Width = (_model.GameSize == 9 ? 5 : 10),
                        Height = (_model.GameSize == 9 ? 20 : 40),
                        X = i,
                        Y = j,
                        IsVertical = true,
                        Top = (_model.GameSize == 9 ? 15 + 35 * i : 30 + 70 * i),
                        Left = (_model.GameSize == 9 ? 5 + 35 * j : 10 + 70 * j),
                        DrawCommand = new DelegateCommand(param =>
                        {
                            if (param is Tuple<int, int, bool> xyv)
                            {
                                DrawLine(xyv.Item1, xyv.Item2, xyv.Item3);
                            }
                        })
                    });
                }
            }
            for (int i = 0; i < _model.Table.Rows.GetLength(0); i++)
            {
                for (int j = 0; j < _model.Table.Rows.GetLength(1); j++)
                {
                    HorFields.Add(new SquaresField
                    {
                        IsDrawn = _model.Table.Rows[i, j],
                        IsEnabled = !_model.Table.Rows[i, j],
                        X = i,
                        Y = j,
                        IsVertical = false,
                        Height = (_model.GameSize == 9 ? 5 : 10),
                        Width = (_model.GameSize == 9 ? 20 : 40),
                        Top = (_model.GameSize == 9 ? 5 + 35 * i : 10 + 70 * i),
                        Left = (_model.GameSize == 9 ? 15 + 35 * j : 30 + 70 * j),
                        DrawCommand = new DelegateCommand(param =>
                        {
                            if (param is Tuple<int, int, bool> xyv)
                            {
                                DrawLine(xyv.Item1, xyv.Item2, xyv.Item3);
                            }
                        })
                    });
                }
            }
        }
        public void ClearTable()
        {
            VerFields.Clear();
            HorFields.Clear();
        }

        #endregion

        #region Private Methods

        private void DrawLine(int x, int y, bool isVertical)
        {
            _model.DrawLine(x, y, isVertical);
        }

        #endregion

        #region Event handlers

        private void Model_TableChange(object? sender, TableEventArgs e)
        {
            if (e.IsVertical)
            {
                SquaresField field = VerFields.Single(f => e.X == f.X && e.Y == f.Y)!;
                field.IsDrawn = true;
                field.IsEnabled = false;
            }
            else
            {
                SquaresField field = HorFields.Single(f => e.X == f.X && e.Y == f.Y)!;
                field.IsDrawn = true;
                field.IsEnabled = false;
            }
        }

        private void Model_ScoreIncrease(object? sender, PlayerEventArgs e)
        {
            if (e.Player.Color == "kék")
            {
                PlayerOneScore += e.Score;
                OnPropertyChanged(nameof(PlayerOneScore));
            }
            else
            {
                PlayerTwoScore += e.Score;
                OnPropertyChanged(nameof(PlayerTwoScore));
            }
        }

        #endregion

        #region Event methods

        private void OnNewGame()
        {
            _model.NewGame();
            ClearTable();
            CreateTable();
            NewGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
