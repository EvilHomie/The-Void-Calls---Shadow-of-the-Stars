using System;

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

        public void FromInstance(object instance)
        {
            ResolveType = ResolveType.Instance;
            Instance = instance;
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
