public static class Enums
{
    // Dev
    public enum ReleaseMode { Debug, Release };
    public enum ServerMode { TestNet, MainNet };


    public enum Screen { Home, Garage, MapSelection, Lobby, Gameplay, PreGameplay, Invite, Shop, Profile }
    public enum GraphicsMode { Performant, Balanced, HighFidelity }
    public enum Platform { WebGl, Android, iOS }
    public enum AnimType { Scale, Position }
    public enum FuelPurchaseType { Tank, Restore }
    public enum ChestType { SNACK, TREAT, FEAST }



    // Inputs
    public enum InputType { Arrows, Joystick }
    public enum SwipeDirection { None, Up, Down, Left, Right };
    public enum Direction { None, Forward, Backward, Left, Right };
    public enum RiderType { Player, Rival, Traffic };


    // Rider
    public enum VehicleType { TwoWheeler, FourWheeler }
    public enum UpgradeType { Speed, Shield, Mileage, Armour }
    public enum GarageVehicleType { Regular, Special }


    // Maps
    public enum OrderCollectingBy { None, Player, Rival };
    public enum OrderStatus { NewOrder, Delivering, Delivered, MissedIt };
    public enum BoosterType { Speed, Health, Fuel, Acceleration, None }
    public enum LevelBoosterType { Speed, Shield, NoRival, None }
    public enum ObstacleType { Still, Dynamic }
    public enum ObstacleVarient { Tree, JCB, SchoolBus, CourierVan, WoodTruck, OilTanker, FireBrigade }


    // Level
    public enum Objective { DeliverOrders, CollectGreenEmoji, CollectYellowEmoji, CollectRedEmoji }
    public enum ObjectiveCondition { None, InTime, MinHealth }
    public enum CostType { POINTS, ORARE }


    // Leaderboard
    public enum SortBy { Munches, DishPTS, Brownie }


    // Dish
    public enum DishRegion
    {
        Any, Caribbean, Australia_and_New_Zealand,
        Southern_Asia, Central_Asia, Northern_Asia, Western_Asia, East_Asia, Eastern_Asia, South___Eastern_Asia,
        Southern_Europe, Northern_Europe, Western_Europe, Eastern_Europe,
        Central_America, South_America, Northern_America,
        Middle_Africa, Eastern_Africa, Northern_Africa, Southern_Africa, Western_Africa
    }
    public enum DishContinent { Any, North_America, Europe, Asia, Africa, South_America, Australia, Oceania }
    public enum DishCountry { Any, India, Jamaica, Philippines, Poland, Australia, Britain, USA }
    public enum DishDiet { Any, NON_VEGETARIAN, VEGAN, VEGETARIAN }
    public enum DishMethod { Any, Raw, Boil, Cook, Grill, Fry, Bake, Roast, Toast, Steam, Fried }
    public enum DishBrand { Any, OneRare, Chef_Special, Cornitos, The_Bhukkad_Cafe, Glocal_Junction, Art_of_Dum, Indian_Bistro, China_Bistro, Papa_Johns, Masterchow, Get_Fudo, MAGGI, Miam, Bagrry__s, Wingreens_Farms, To_Be_Honest, Burgrill, Salad_Days, }
    public enum DishMeter { Any, Skill_Meter, Time_Meter, Spice_Meter }
    public enum MeterCondition { None, Equals_To, Less_Than, Greater_Than }





    public enum QueryResult { Success, Error, Cancelled }

    //Analytics

    public enum EnvironmentType { editor, testing, production }

    public enum GameModeType { SinglePlayer, Multiplayer }

    public enum OrderFailedReasonType { Time, Rival }

    public enum Orientation { Portrait, Landscape }
    public enum SourceType { LevelComplete, OrderDelivered }

 
}
