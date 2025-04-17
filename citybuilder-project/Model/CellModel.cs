using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace citybuilder_project.Model
{
    public enum BuildingType
    {
        None,
        SmallHouse,
        MediumHouse,
        LargeHouse,
        SmallPowerPlant,
        LargePowerPlant,
        SmallWaterPlant,
        LargeWaterPlant
    }

    public class CellModel : INotifyPropertyChanged
    {
        public int Row { get; set; }
        public int Column { get; set; }

        private string _color = "White";
        public string Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }

        private BuildingType _buildingType = BuildingType.None;
        public BuildingType BuildingType
        {
            get => _buildingType;
            set
            {
                if (_buildingType != value)
                {
                    _buildingType = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasBuilding));
                }
            }
        }

        public bool HasBuilding => BuildingType != BuildingType.None;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
