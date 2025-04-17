using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace citybuilder_project.Model
{
    public class CityModel : INotifyPropertyChanged
    {
        private int _money = 10000; // Starting money
        private int _income = 0;
        private int _population = 0;
        private int _housingCapacity = 0;
        private int _powerProduction = 0;
        private int _powerConsumption = 0;
        private int _waterProduction = 0;
        private int _waterConsumption = 0;
        private int _negativeCycles = 0;

        public int Money
        {
            get => _money;
            set
            {
                if (_money != value)
                {
                    _money = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Income
        {
            get => _income;
            private set
            {
                if (_income != value)
                {
                    _income = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Population
        {
            get => _population;
            set
            {
                if (_population != value)
                {
                    _population = value;
                    OnPropertyChanged();
                    CalculateIncome();
                }
            }
        }

        public int HousingCapacity
        {
            get => _housingCapacity;
            set
            {
                if (_housingCapacity != value)
                {
                    _housingCapacity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AvailableHousing));
                }
            }
        }

        public int AvailableHousing => HousingCapacity - Population;

        public int PowerProduction
        {
            get => _powerProduction;
            set
            {
                if (_powerProduction != value)
                {
                    _powerProduction = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AvailablePower));
                }
            }
        }

        public int PowerConsumption
        {
            get => _powerConsumption;
            set
            {
                if (_powerConsumption != value)
                {
                    _powerConsumption = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AvailablePower));
                }
            }
        }

        public int AvailablePower => PowerProduction - PowerConsumption;

        public int WaterProduction
        {
            get => _waterProduction;
            set
            {
                if (_waterProduction != value)
                {
                    _waterProduction = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AvailableWater));
                }
            }
        }

        public int WaterConsumption
        {
            get => _waterConsumption;
            set
            {
                if (_waterConsumption != value)
                {
                    _waterConsumption = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AvailableWater));
                }
            }
        }

        public int AvailableWater => WaterProduction - WaterConsumption;

        public int NegativeCycles
        {
            get => _negativeCycles;
            set
            {
                if (_negativeCycles != value)
                {
                    _negativeCycles = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsGameOver => NegativeCycles >= 2;

        private int _totalMaintenanceCost = 0;

        public void CalculateIncome()
        {
            // Each citizen provides 50 income
            int citizenIncome = Population * 50;
            Income = citizenIncome - _totalMaintenanceCost;
        }

        public void AddBuilding(Building building)
        {
            HousingCapacity += building.HousingCapacity;
            PowerProduction += building.PowerProduction;
            PowerConsumption += building.PowerConsumption;
            WaterProduction += building.WaterProduction;
            WaterConsumption += building.WaterConsumption;

            _totalMaintenanceCost += building.MaintenanceCost;
            CalculateIncome();

            Money -= building.Cost;
        }

        public bool CanAfford(Building building)
        {
            return Money >= building.Cost;
        }

        public bool CanSupport(Building building)
        {
            // Check if we have enough power and water for this new building
            return (AvailablePower >= building.PowerConsumption) &&
                   (AvailableWater >= building.WaterConsumption);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
