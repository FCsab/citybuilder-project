namespace citybuilder_project.Model
{
    public class Building
    {
        public string Name { get; set; } = "";
        public BuildingType Type { get; set; }
        public int Cost { get; set; }
        public int MaintenanceCost { get; set; }
        public int WaterConsumption { get; set; }
        public int PowerConsumption { get; set; }
        public int HousingCapacity { get; set; }
        public int WaterProduction { get; set; }
        public int PowerProduction { get; set; }

        // Visual properties
        public string Color { get; set; } = "Gray";

        public static Building GetBuildingByType(BuildingType type)
        {
            return type switch
            {
                BuildingType.SmallHouse => new Building
                {
                    Name = "Small House",
                    Type = BuildingType.SmallHouse,
                    Cost = 1000,
                    MaintenanceCost = 10,
                    WaterConsumption = 10,
                    PowerConsumption = 10,
                    HousingCapacity = 10,
                    Color = "LightGreen"
                },
                BuildingType.MediumHouse => new Building
                {
                    Name = "Medium House",
                    Type = BuildingType.MediumHouse,
                    Cost = 2000,
                    MaintenanceCost = 25,
                    WaterConsumption = 25,
                    PowerConsumption = 25,
                    HousingCapacity = 30,
                    Color = "Green"
                },
                BuildingType.LargeHouse => new Building
                {
                    Name = "Large House",
                    Type = BuildingType.LargeHouse,
                    Cost = 5000,
                    MaintenanceCost = 60,
                    WaterConsumption = 60,
                    PowerConsumption = 60,
                    HousingCapacity = 80,
                    Color = "DarkGreen"
                },
                BuildingType.SmallPowerPlant => new Building
                {
                    Name = "Small Power Plant",
                    Type = BuildingType.SmallPowerPlant,
                    Cost = 5000,
                    MaintenanceCost = 100,
                    WaterConsumption = 50,
                    PowerProduction = 200,
                    Color = "Yellow"
                },
                BuildingType.LargePowerPlant => new Building
                {
                    Name = "Large Power Plant",
                    Type = BuildingType.LargePowerPlant,
                    Cost = 10000,
                    MaintenanceCost = 250,
                    WaterConsumption = 150,
                    PowerProduction = 500,
                    Color = "Orange"
                },
                BuildingType.SmallWaterPlant => new Building
                {
                    Name = "Small Water Plant",
                    Type = BuildingType.SmallWaterPlant,
                    Cost = 3000,
                    MaintenanceCost = 80,
                    PowerConsumption = 50,
                    WaterProduction = 200,
                    Color = "LightBlue"
                },
                BuildingType.LargeWaterPlant => new Building
                {
                    Name = "Large Water Plant",
                    Type = BuildingType.LargeWaterPlant,
                    Cost = 8000,
                    MaintenanceCost = 200,
                    PowerConsumption = 120,
                    WaterProduction = 500,
                    Color = "Blue"
                },
                _ => new Building()
            };
        }
    }
}
