using System;
using UnityEngine;

namespace DI
{
    public class BindingInfo
    {
        public ResolveType ResolveType = ResolveType.Singleton;
        public Type Contract;
        public Type Implementation;
        public object Instance;
        public Func<Container, object> Factory;

        public void AsSingleton()
        {
            ResolveType = ResolveType.Singleton;
        }

        public void AsTransient()
        {
            ResolveType = ResolveType.Transient;
        }

        public BindingInfo FromInstance(object instance)
        {
            if (instance == null)
            {
                throw new Exception($"Для контракта {Contract.Name} не был передан экземпляр.");
            }

            ResolveType = ResolveType.Instance;
            Instance = instance;

            return this;
        }

        public BindingInfo To<T>()
        {
            var type = typeof(T);

            if (!Contract.IsAssignableFrom(type))
            {
                throw new Exception($"тип {type.Name} не наследуется от {Contract.Name}");
            }

            Implementation = type;
            return this;
        }
    }
}
