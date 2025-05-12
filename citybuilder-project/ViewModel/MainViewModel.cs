using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using citybuilder_project.Model;

namespace citybuilder_project.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private CityModel _city;
        private BuildingType _selectedBuildingType = BuildingType.None;
        private DispatcherTimer _gameTimer = null!; // Mark as null-forgiving since we initialize in InitializeTimers
        private DispatcherTimer _populationTimer = null!; // Mark as null-forgiving since we initialize in InitializeTimers
        private int _cycleCounter = 0;
        private Random _random = new Random();

        // Remove the _isRemovalMode field as we don't need it anymore

        public ObservableCollection<CellModel> Cells { get; set; }
        public ObservableCollection<BuildingType> AvailableBuildings { get; set; }

        // Add RemoveBuilding to the available building types
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

                    // Set appropriate message based on selected building type
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

        // Remove IsRemovalMode property as we'll use SelectedBuildingType == RemoveBuildingOption instead

        public ICommand PlaceBuildingCommand { get; set; }
        public ICommand CellClickCommand { get; }
        // Remove ToggleRemovalModeCommand
        public ICommand StartGameCommand { get; }
        public ICommand ResetGameCommand { get; }

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
                RemoveBuildingOption, // Add the Remove Building option at the top
                BuildingType.SmallHouse,
                BuildingType.MediumHouse,
                BuildingType.LargeHouse,
                BuildingType.SmallPowerPlant,
                BuildingType.LargePowerPlant,
                BuildingType.SmallWaterPlant,
                BuildingType.LargeWaterPlant
            };

            // Initialize the 15x15 grid
            InitializeGrid();

            // Initialize commands
            PlaceBuildingCommand = new RelayCommand<CellModel>(PlaceBuilding, CanPlaceBuilding);
            CellClickCommand = new RelayCommand<CellModel>(OnCellClick);
            // Remove ToggleRemovalModeCommand
            StartGameCommand = new RelayCommand<object>(_ => StartGame());
            ResetGameCommand = new RelayCommand<object>(_ => ResetGame());

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
                Interval = TimeSpan.FromSeconds(10)  // Income every 10 seconds
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

            // Add income to money
            _city.Money += _city.Income;

            // Check for game over condition
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
                // Calculate potential new citizens (up to 25% of current population)
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
                // Only create a building object for actual buildings, not for the remove option
                if (_selectedBuildingType != RemoveBuildingOption)
                {
                    SelectedBuilding = Building.GetBuildingByType(_selectedBuildingType);
                }
                else
                {
                    // For removal option, set a special "building" to represent removal
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

            // Check if we're in removal mode (SelectedBuildingType == RemoveBuildingOption)
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

            // If we're in removal mode, we can "place" (remove) if there's a building
            if (SelectedBuildingType == RemoveBuildingOption)
                return cell.HasBuilding;

            // Check if cell already has a building
            if (cell.HasBuilding)
                return false;

            var building = Building.GetBuildingByType(SelectedBuildingType);

            // Check if player can afford and support this building
            return _city.CanAfford(building) && _city.CanSupport(building);
        }

        private void PlaceBuilding(CellModel cell)
        {
            if (cell != null && SelectedBuildingType != BuildingType.None)
            {
                var building = Building.GetBuildingByType(SelectedBuildingType);

                // Check if player can afford and support this building
                if (_city.CanAfford(building) && _city.CanSupport(building))
                {
                    // Apply the building
                    cell.BuildingType = SelectedBuildingType;
                    cell.Color = building.Color;

                    // Update city model
                    _city.AddBuilding(building);

                    // Show status message
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

                // Update city model
                _city.RemoveBuilding(building);

                // Clear the cell
                cell.BuildingType = BuildingType.None;
                cell.Color = "White";

                // Show status message with refund amount
                GameStatus = $"Removed {building.Name} and refunded ${building.Cost * 0.7:F0}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
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
