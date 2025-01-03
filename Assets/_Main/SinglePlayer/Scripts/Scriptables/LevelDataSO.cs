using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static FoodFury.ModelClass;

namespace FoodFury
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "FoodFury/LevelData")]
    public class LevelDataSO : ScriptableObject
    {
        public int version;
        public List<Level> levels;

        [Serializable]
        public class Level
        {
            public string title;
            public int level = 1;
            public int orderTime = 60;
            public int orderMinRange = 50;
            public int orderMaxRange = 400;
            public int levelBoosterCost = 10;

            [Space(10)]
            public List<LevelObjective> objectives = new List<LevelObjective>();
            public List<LevelCondition> conditions = new List<LevelCondition>();

            [Space(10)]
            public List<BoosterDetail> boosterDetails;
            //public ObstacleDetail obstacleDetails;

            public bool TryGetObjective(Enums.Objective _objective, out int _value)
            {
                var _result = objectives.Where(kv => kv.objective == _objective);
                _value = _result.Count() > 0 ? _result.First().objectiveValue : 0;
                return _result.Count() > 0;
            }

            public bool TryGetCondition(Enums.ObjectiveCondition _condition, out int _value)
            {
                var _result = conditions.Where(kv => kv.condition == _condition);
                _value = _result.Count() > 0 ? _result.First().conditionValue : 0;
                return _result.Count() > 0;
            }

            [Serializable]
            public class LevelObjective
            {
                [JsonConverter(typeof(StringEnumConverter))] public Enums.Objective objective;
                public int objectiveValue = 1;
            }

            [Serializable]
            public class LevelCondition
            {
                [JsonConverter(typeof(StringEnumConverter))] public Enums.ObjectiveCondition condition;
                public int conditionValue = 1;
            }
        }
    }
}
