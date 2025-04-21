using System;
using System.Collections.Generic;
using Spectral.Autonation.Components;
using Spectral.Autonation.Systems;
using Spectral.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spectral.Autonation.Managers
{
    public class GameManager : SingletonBehaviour<GameManager>
    {
        public Mesh mesh;
        public Material mat;
        private List<EntitySystem> _fixedUpdateSystems;
        private List<EntitySystem> _lateUpdateSystems;
        private List<EntitySystem> _slowUpdateSystems;
        private List<EntitySystem> _updateSystems;
        private float _timeAtLastSlowUpdate;

        private void Awake()
        {
            _updateSystems = new List<EntitySystem>(8);
            _fixedUpdateSystems = new List<EntitySystem>(8);
            _lateUpdateSystems = new List<EntitySystem>(8);
            _slowUpdateSystems = new List<EntitySystem>(8);
            var moveEntitySystem = new MoveEntitySystem("MoveSystem", UpdateType.Update);
        }

        private void Start()
        {
            for (var i = 0; i < 5000; i++)
            {
                Vector3 pos = Random.insideUnitSphere * 10;
                var transformC = new TransformC(1, i, pos, Random.rotation, Random.insideUnitSphere / 10);
                var moverC = new MoverC(2, i, Random.value, Random.insideUnitSphere);
                var rendererC = new RendererC(3, i, mesh, mat);
                EntityDatabase.Instance.Add(new Entity(i), transformC, moverC, rendererC);
                rendererC.LinkToEntityGameObject();
            }
        }

        private void Update()
        {
            IterateSystems(_updateSystems, Time.deltaTime);
        }

        private void FixedUpdate()
        {
            IterateSystems(_fixedUpdateSystems, Time.fixedDeltaTime);
            if (Time.fixedTime - _timeAtLastSlowUpdate >= 0.1f)
            {
                IterateSystems(_slowUpdateSystems, 0.1f);
                _timeAtLastSlowUpdate = Time.fixedTime;
            }
        }

        private void LateUpdate()
        {
            IterateSystems(_lateUpdateSystems, Time.deltaTime);
        }


        private static void IterateSystems(List<EntitySystem> systems, float deltaTime)
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}