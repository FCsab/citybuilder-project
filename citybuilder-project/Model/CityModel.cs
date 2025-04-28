using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace citybuilder_project.Model
{
    public class CityModel : INotifyPropertyChanged
    {
        // Starting money
        private int _money = 100000;
        private int _income = 0;
        private int _population = 0;
        private int _housingCapacity = 0;
        private int _powerProduction = 0;
        private int _powerConsumption = 0;
        private int _waterProduction = 0;
        private int _waterConsumption = 0;
        private int _negativeCycles = 0;
        private int _totalMaintenanceCost = 0;

        public int Money
        {
            get => _money;
            set
            {
                if (_money != value)
                {
                    _money = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Income)); // Income depends on money in some cases
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
                    OnPropertyChanged(nameof(AvailableHousing));
                    CalculateIncome();
                }
            }
        }

        public int HousingCapacity
        {
            get => _housingCapacity;
            private set
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
            private set
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
            private set
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
            private set
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
            private set
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
                    OnPropertyChanged(nameof(IsGameOver));
                }
            }
        }

        public bool IsGameOver => NegativeCycles >= 2;

        public void CalculateIncome()
        {
            // Each citizen contributes $5 to income
            int citizenIncome = Population * 5;

            // Calculate the final income
            Income = citizenIncome - _totalMaintenanceCost;
        }

        public void AddBuilding(Building building)
        {
            if (building == null)
                return;

            // Deduct cost from money
            Money -= building.Cost;

            // Add maintenance cost
            _totalMaintenanceCost += building.MaintenanceCost;

            // Update resources
            HousingCapacity += building.HousingCapacity;
            PowerProduction += building.PowerProduction;
            PowerConsumption += building.PowerConsumption;
            WaterProduction += building.WaterProduction;
            WaterConsumption += building.WaterConsumption;

            // Recalculate income
            CalculateIncome();

            // Ensure all properties get notified
            OnPropertyChanged(nameof(Money));
            OnPropertyChanged(nameof(Income));
            OnPropertyChanged(nameof(HousingCapacity));
            OnPropertyChanged(nameof(PowerProduction));
            OnPropertyChanged(nameof(PowerConsumption));
            OnPropertyChanged(nameof(WaterProduction));
            OnPropertyChanged(nameof(WaterConsumption));
            OnPropertyChanged(nameof(AvailableHousing));
            OnPropertyChanged(nameof(AvailablePower));
            OnPropertyChanged(nameof(AvailableWater));
        }

        public bool CanAfford(Building building)
        {
            if (building == null)
                return false;

            return Money >= building.Cost;
        }

        public bool CanSupport(Building building)
        {
            if (building == null)
                return false;

            // Check if we have enough resources for this building
            bool hasPower = AvailablePower >= building.PowerConsumption;
            bool hasWater = AvailableWater >= building.WaterConsumption;

            // Power plants need water but not power to check
            if (building.Type == BuildingType.SmallPowerPlant || building.Type == BuildingType.LargePowerPlant)
                return hasWater; // Power plants only need water

            // Water plants need power but not water to check
            if (building.Type == BuildingType.SmallWaterPlant || building.Type == BuildingType.LargeWaterPlant)
                return hasPower; // Water plants only need power

            // If we're placing a first house with no population yet, we can always build it
            if ((building.Type == BuildingType.SmallHouse ||
                 building.Type == BuildingType.MediumHouse ||
                 building.Type == BuildingType.LargeHouse) &&
                Population == 0)
                return true;

            // Houses need both power and water
            return hasPower && hasWater;
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public CityModel()
        {
            // Initialize with some starter resources to allow building initially
            PowerProduction = 100;
            WaterProduction = 100;
        }

    }

}
