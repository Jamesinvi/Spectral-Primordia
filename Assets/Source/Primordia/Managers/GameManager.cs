using System;
using System.Collections.Generic;
using Primordia.Core;
using Primordia.Systems;
using UnityEngine;

namespace Primordia.Managers
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        private List<EntitySystem> _fixedUpdateSystems;
        private List<EntitySystem> _lateUpdateSystems;
        private List<EntitySystem> _slowUpdateSystems;
        private float _timeAtLastEverySecondUpdate;
        private float _timeAtLastSlowUpdate;
        private List<EntitySystem> _updateEverySecondSystems;
        private List<EntitySystem> _updateSystems;

        private void Awake()
        {
            _updateSystems = new List<EntitySystem>(8);
            _fixedUpdateSystems = new List<EntitySystem>(8);
            _lateUpdateSystems = new List<EntitySystem>(8);
            _slowUpdateSystems = new List<EntitySystem>(8);
            _updateEverySecondSystems = new List<EntitySystem>(8);
            var generateResourcesSystem = new GenerateResourcesSystem("Resource Generation System");
        }

        private void Update()
        {
            UpdateSystems(_updateSystems, Time.deltaTime);
        }

        private void FixedUpdate()
        {
            UpdateSystems(_fixedUpdateSystems, Time.fixedDeltaTime);
            if (Time.fixedTime - _timeAtLastSlowUpdate >= 0.1f)
            {
                UpdateSystems(_slowUpdateSystems, 0.1f);
                _timeAtLastSlowUpdate = Time.fixedTime;
            }

            if (Time.fixedTime - _timeAtLastEverySecondUpdate >= 1f)
            {
                UpdateSystems(_updateEverySecondSystems, 1f);
                _timeAtLastEverySecondUpdate = Time.fixedTime;
            }
        }

        private void LateUpdate()
        {
            UpdateSystems(_lateUpdateSystems, Time.deltaTime);
        }


        private static void UpdateSystems(List<EntitySystem> systems, float deltaTime)
        {
            foreach (EntitySystem system in systems) system?.Update(deltaTime);
        }

        public void AddSystemToUpdateList(EntitySystem entitySystem)
        {
            switch (entitySystem.updateType)
            {
                case UpdateType.Update:
                    _updateSystems.Add(entitySystem);
                    break;
                case UpdateType.LateUpdate:
                    _lateUpdateSystems.Add(entitySystem);
                    break;
                case UpdateType.FixedUpdate:
                    _fixedUpdateSystems.Add(entitySystem);
                    break;
                case UpdateType.SlowUpdate:
                    _slowUpdateSystems.Add(entitySystem);
                    break;
                case UpdateType.EverySecond:
                    _updateEverySecondSystems.Add(entitySystem);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}