using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using citybuilder_project.Model;
using Microsoft.Win32;
using System.IO;
using System.Text.Json;

namespace citybuilder_project.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private CityModel _city;
        private BuildingType _selectedBuildingType = BuildingType.None;
        private DispatcherTimer _gameTimer = null!;
        private DispatcherTimer _populationTimer = null!;
        private int _cycleCounter = 0;
        private Random _random = new Random();

        public ObservableCollection<CellModel> Cells { get; set; }
        public ObservableCollection<BuildingType> AvailableBuildings { get; set; }

        public const BuildingType RemoveBuildingOption = BuildingType.None;

        public CityModel City => _city;

        public BuildingType SelectedBuildingType
        {
            get => _selectedBuildingType;
            set
            {
                if (_selectedBuildingType != value)
                {
                    _selectedBuildingType = value;
                    OnPropertyChanged();
                    UpdateSelectedBuildingInfo();

                    if (value == RemoveBuildingOption)
                    {
                        GameStatus = "Removal mode active. Click a building to remove it.";
                    }
                    else
                    {
                        GameStatus = "Building placement mode active.";
                    }
                }
            }
        }

        private Building? _selectedBuilding;
        public Building? SelectedBuilding
        {
            get => _selectedBuilding;
            private set
            {
                _selectedBuilding = value;
                OnPropertyChanged();
            }
        }

        public ICommand PlaceBuildingCommand { get; set; }
        public ICommand CellClickCommand { get; }
        public ICommand StartGameCommand { get; }
        public ICommand ResetGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand LoadGameCommand { get; }

        private string _gameStatus = "Ready to start";
        public string GameStatus
        {
            get => _gameStatus;
            set
            {
                if (_gameStatus != value)
                {
                    _gameStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainViewModel()
        {
            _city = new CityModel();
            Cells = new ObservableCollection<CellModel>();

            AvailableBuildings = new ObservableCollection<BuildingType>
            {
                RemoveBuildingOption,
                BuildingType.SmallHouse,
                BuildingType.MediumHouse,
                BuildingType.LargeHouse,
                BuildingType.SmallPowerPlant,
                BuildingType.LargePowerPlant,
                BuildingType.SmallWaterPlant,
                BuildingType.LargeWaterPlant
            };

            InitializeGrid();

            PlaceBuildingCommand = new RelayCommand<CellModel>(PlaceBuilding, CanPlaceBuilding);
            CellClickCommand = new RelayCommand<CellModel>(OnCellClick);
            StartGameCommand = new RelayCommand<object>(_ => StartGame());
            ResetGameCommand = new RelayCommand<object>(_ => ResetGame());
            SaveGameCommand = new RelayCommand<object>(_ => SaveGame());
            LoadGameCommand = new RelayCommand<object>(_ => LoadGame());

            InitializeTimers();
        }

        private void InitializeGrid()
        {
            Cells.Clear();
            for (int row = 0; row < 15; row++)
            {
                for (int col = 0; col < 15; col++)
                {
                    Cells.Add(new CellModel { Row = row, Column = col, Color = "White", BuildingType = BuildingType.None });
                }
            }
        }

        private void InitializeTimers()
        {
            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };
            _gameTimer.Tick += OnGameTimerTick;

            _populationTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            _populationTimer.Tick += OnPopulationTimerTick;
        }

        private void StartGame()
        {
            _gameTimer.Start();
            _populationTimer.Start();
            GameStatus = "Game in progress";
        }

        private void ResetGame()
        {
            _gameTimer.Stop();
            _populationTimer.Stop();
            _city = new CityModel();
            InitializeGrid();
            _cycleCounter = 0;
            GameStatus = "Game reset. Ready to start.";
            OnPropertyChanged(nameof(City));
        }

        private void OnGameTimerTick(object? sender, EventArgs e)
        {
            _cycleCounter++;
            GameStatus = $"Game cycle: {_cycleCounter}";

            _city.Money += _city.Income;

            if (_city.Money < 0)
            {
                _city.NegativeCycles++;
                if (_city.IsGameOver)
                {
                    _gameTimer.Stop();
                    _populationTimer.Stop();
                    GameStatus = "Game Over! You've been in debt for 2 cycles.";
                }
            }
            else
            {
                _city.NegativeCycles = 0;
            }

            OnPropertyChanged(nameof(City));
        }

        private void OnPopulationTimerTick(object? sender, EventArgs e)
        {
            if (_city.AvailableHousing > 0)
            {
                int maxNewCitizens = Math.Max(1, (int)(_city.Population * 0.25));
                int actualNewCitizens = Math.Min(maxNewCitizens, _city.AvailableHousing);

                _city.Population += actualNewCitizens;

                if (actualNewCitizens > 0)
                {
                    GameStatus = $"{actualNewCitizens} new citizen(s) moved to your city!";
                }
            }
        }

        private void UpdateSelectedBuildingInfo()
        {
            if (_selectedBuildingType != BuildingType.None)
            {
                if (_selectedBuildingType != RemoveBuildingOption)
                {
                    SelectedBuilding = Building.GetBuildingByType(_selectedBuildingType);
                }
                else
                {
                    SelectedBuilding = new Building
                    {
                        Name = "Remove Building",
                        Color = "Red",
                        Type = RemoveBuildingOption
                    };
                }
            }
            else
            {
                SelectedBuilding = null;
            }
        }

        private void OnCellClick(CellModel cell)
        {
            if (cell == null) return;

            if (SelectedBuildingType == RemoveBuildingOption)
            {
                if (cell.BuildingType != BuildingType.None)
                {
                    RemoveBuilding(cell);
                }
                else
                {
                    GameStatus = "No building to remove at this location.";
                }
            }
            else
            {
                if (cell.BuildingType == BuildingType.None && SelectedBuildingType != BuildingType.None)
                {
                    PlaceBuilding(cell);
                }
                else if (cell.BuildingType != BuildingType.None)
                {
                    GameStatus = "Cannot place building here. Select 'Remove Building' option first.";
                }
            }
        }

        private bool CanPlaceBuilding(CellModel? cell)
        {
            if (cell == null || SelectedBuildingType == BuildingType.None)
                return false;

            if (SelectedBuildingType == RemoveBuildingOption)
                return cell.HasBuilding;

            if (cell.HasBuilding)
                return false;

            var building = Building.GetBuildingByType(SelectedBuildingType);

            return _city.CanAfford(building) && _city.CanSupport(building);
        }

        private void PlaceBuilding(CellModel cell)
        {
            if (cell != null && SelectedBuildingType != BuildingType.None)
            {
                var building = Building.GetBuildingByType(SelectedBuildingType);

                if (_city.CanAfford(building) && _city.CanSupport(building))
                {
                    cell.BuildingType = SelectedBuildingType;
                    cell.Color = building.Color;

                    _city.AddBuilding(building);

                    GameStatus = $"Placed {building.Name}";
                }
                else
                {
                    GameStatus = "Cannot place building. Check resources and money.";
                }
            }
        }

        private void RemoveBuilding(CellModel cell)
        {
            if (cell != null && cell.HasBuilding)
            {
                var building = Building.GetBuildingByType(cell.BuildingType);

                _city.RemoveBuilding(building);

                cell.BuildingType = BuildingType.None;
                cell.Color = "White";

                GameStatus = $"Removed {building.Name} and refunded ${building.Cost * 0.7:F0}";
            }
        }

        // --- Save/Load Functionality ---

        private void SaveGame()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "City Builder Save (*.json)|*.json",
                DefaultExt = "json"
            };
            if (dialog.ShowDialog() == true)
            {
                var saveData = new SaveGameData
                {
                    City = _city,
                    Cells = new ObservableCollection<CellModel>(Cells)
                };
                var options = new JsonSerializerOptions { WriteIndented = true };
                File.WriteAllText(dialog.FileName, JsonSerializer.Serialize(saveData, options));
                GameStatus = "Game saved successfully.";
            }
        }

        private void LoadGame()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "City Builder Save (*.json)|*.json",
                DefaultExt = "json"
            };
            if (dialog.ShowDialog() == true)
            {
                var json = File.ReadAllText(dialog.FileName);
                var saveData = JsonSerializer.Deserialize<SaveGameData>(json);
                if (saveData != null)
                {
                    _city = saveData.City;
                    Cells.Clear();
                    foreach (var cell in saveData.Cells)
                        Cells.Add(cell);
                    OnPropertyChanged(nameof(City));
                    GameStatus = "Game loaded successfully.";
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Helper class for serialization
    public class SaveGameData
    {
        public CityModel City { get; set; }
        public ObservableCollection<CellModel> Cells { get; set; }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;
        private readonly Predicate<T>? _canExecute;

        public RelayCommand(Action<T> execute, Predicate<T>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter == null && typeof(T).IsValueType)
                return false;

            return _canExecute == null || _canExecute((T)parameter!);
        }

        public void Execute(object? parameter) => _execute((T)parameter!);

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
