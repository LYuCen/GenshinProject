using System;
using System.Collections.Generic;
using mEnum;
using mModel;
using mWeapon.Component;
using mWeapon.Presenter;
using mWeapon.WeaponFeature;
using QFramework;

namespace mWeapon
{
    public class WeaponPro : IController
    {
        public IReadOnlyWeaponRuntime Runtime => _runtime;
        internal WeaponRuntime InternalRuntime => _runtime;

        private WeaponRuntime _runtime;
        public WeaponTaskRunner TaskRunner { get; private set; }
        public WeaponType WeaponType => _data.weaponType;
        public string WeaponName => _data.weaponName;
        public WeaponPresenter Presenter = null;

        private readonly List<IWeaponFeature> _features = new();
        private readonly Dictionary<Type, object> _components = new();
        private readonly WeaponData _data;
        public readonly BagModel BagModel;

        public WeaponPro(WeaponData data)
        {
            _data = data;
            _runtime = new WeaponRuntime();
            TaskRunner = new WeaponTaskRunner();
            BagModel = this.GetModel<BagModel>();
            BuildComponents();
            BuildFeatures();
            _runtime.Component = GetComponent<MagazineComponent>();
        }

        private void BuildFeatures()
        {
            foreach (var featureData in _data.features)
            {
                IWeaponFeature feature = WeaponFeatureFactory.Create(featureData);

                if (feature == null) continue;

                feature.Initialize(this);

                _features.Add(feature);
            }
        }
        
        private void BuildComponents()
        {
            foreach (var data in _data.components)
            {
                object component = WeaponComponentFactory.Create(data);

                if (component == null) continue;

                _components[component.GetType()] = component;
            }
        }

        public T GetFeature<T>() where T : class, IWeaponFeature
        {
            for (int i = 0; i < _features.Count; i++)
            {
                if (_features[i] is T feature) return feature;
            }
            return null;
        }
        
        /// <summary>
        /// 必须包含的 Feature
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T RequireFeature<T>() where T : class, IWeaponFeature
        {
            T feature = GetFeature<T>();

            if (feature == null)
            {
                throw new Exception($"Weapon [{_data.name}] 缺少 Feature : {typeof(T).Name}");
            }

            return feature;
        }
        
        public T GetComponent<T>() where T : class
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                return component as T;
            }
            return null;
        }
        
        /// <summary>
        /// 必须包含的 Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T RequireComponent<T>() where T : class
        {
            T component = GetComponent<T>();

            if (component == null)
            {
                throw new Exception($"Weapon [{_data.name}] 缺少 Component : {typeof(T).Name}");
            }

            return component;
        }
        
        

        public void HandleAction(WeaponAction action)
        {
            foreach (var feature in _features)
            {
                feature.HandleAction(action);
            }
        }

        public void Tick()
        {
            TaskRunner.Tick();

            foreach (var feature in _features)
            {
                feature.Tick();
            }
        }
        
        public void OnEnter()
        {
            foreach (var feature in _features)
            {
                feature.OnEnter();
            }
        }

        public void OnExit()
        {
            foreach (var feature in _features)
            {
                feature.OnExit();
            }
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}