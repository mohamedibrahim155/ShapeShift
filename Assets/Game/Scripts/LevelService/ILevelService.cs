using System;
using UnityEngine;

namespace Scripts.Level
{
    public interface ILevelService
    {
        public event Action<LevelView> OnLevelCreated;
        void CreateLevel(int levelNo);
        LevelView GetLevel(int levelNo);


    }
}
