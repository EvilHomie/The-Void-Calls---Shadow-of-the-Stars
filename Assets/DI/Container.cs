using System;
using System.Collections.Generic;
using UnityEngine;

namespace DI
{
    /// <summary>
    /// метод должен быть public
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class InjectAttribute : Attribute { }
    public enum ResolveType
    {
        Singleton,
        Instance,
        Transient
    }

    public class Container
    {
        private readonly HelperDI _helperDI;
        private readonly Container _parentContainer;
        private readonly Dictionary<Type, BindingInfo> _bindings;
        private readonly Dictionary<Type, List<Action<object, Func<Type, object>>>> _cachedMonoBehaviourInjection;

        public Container(Container parentContainer = null)
        {
            _helperDI = new();
            _bindings = new();
            _cachedMonoBehaviourInjection = new();
            _parentContainer = parentContainer;
            BindItself();
        }

        /// <summary>
        /// По умолчанию регистрируется как Singleton
        /// </summary>        
        public BindingInfo Bind<T>() where T : class
        {
            var type = typeof(T);
            bool isContract = type.IsInterface || type.IsAbstract;

            var bindingInfo = new BindingInfo
            {
                ResolveType = ResolveType.Singleton,
                Contract = type,
                Implementation = type,
                Factory = isContract ? null : _helperDI.BuildFactory(type) // если тип является контрактом, то создание фабрики переносится в Resolve. Т.к. не известна имплементация
            };

            _bindings.Add(type, bindingInfo);
            return bindingInfo;
        }

        public T Resolve<T>() where T : class
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            var bindingInfo = GetBindingInfo(type);

            if (bindingInfo.ResolveType != ResolveType.Instance)  // необходимо в случае если во время бинда не была известна имплементация и пропуск создания фабрики для instance
            {
                bindingInfo.Factory ??= _helperDI.BuildFactory(bindingInfo.Implementation);
            }

            return bindingInfo.ResolveType switch
            {
                ResolveType.Singleton => bindingInfo.Instance ??= bindingInfo.Factory(this),
                ResolveType.Transient => bindingInfo.Factory(this),
                ResolveType.Instance => bindingInfo.Instance,
                _ => throw new Exception($"нет реализации запроса с типом {bindingInfo.ResolveType}")
            };
        }

        public BindingInfo GetBindingInfo(Type type)
        {
            if (!_bindings.TryGetValue(type, out var binding)) // ищем бинд
            {
                binding = _parentContainer?.GetBindingInfo(type); // ищем бинд у родительского контейнера                
            }

            return binding ?? throw new Exception($"Зависимость {type.Name} не найдена!");
        }

        public void Instantiate(MonoBehaviour instance)
        {
            instance.gameObject.SetActive(false);
            GameObject gameobject = UnityEngine.Object.Instantiate(instance).gameObject;
            InjectMonoBehaviour(instance);
            gameobject.SetActive(true);
        }

        public void InjectMonoBehaviour(MonoBehaviour monoBehaviour)
        {
            var type = monoBehaviour.GetType();

            // кэшируем Expression-делегаты для методов с [Inject]
            if (!_cachedMonoBehaviourInjection.TryGetValue(type, out var actions))
            {
                actions = new List<Action<object, Func<Type, object>>>();
                var injectingMethods = _helperDI.FindInjectingMethods(type);

                foreach (var methodInfo in injectingMethods)
                {
                    actions.Add(_helperDI.CreateInjectData(methodInfo));
                }

                _cachedMonoBehaviourInjection[type] = actions;
            }

            if (actions.Count == 0) return;

            foreach (var action in actions)
            {
                action(monoBehaviour, Resolve);
            }
        }

        private void BindItself()
        {
            var bindingInfo = new BindingInfo
            {
                ResolveType = ResolveType.Instance,
                Instance = this
            };

            _bindings.Add(this.GetType(), bindingInfo);
        }
    }
}
