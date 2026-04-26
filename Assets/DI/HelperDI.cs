using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DI
{
    public class HelperDI
    {
        public Func<Container, object> BuildFactory(Type type)
        {
            var containerParam = Expression.Parameter(typeof(Container)); // Параметр делегата (Container container)            
            var instanceVar = Expression.Variable(type); // Создание переменной для экземпляра объекта            

            var ctor = GetConstructor(type);

            var ctorParams = ctor.GetParameters()
                .Select(p =>
                    Expression.Convert(
                        Expression.Call(
                            containerParam,
                            nameof(Container.Resolve),
                            Type.EmptyTypes,
                            Expression.Constant(p.ParameterType)
                        ),
                        p.ParameterType
                    )
                ).ToArray();

            var newExpression = Expression.New(ctor, ctorParams);
            var assignInstance = Expression.Assign(instanceVar, newExpression);

            var expressions = new List<Expression> { assignInstance }; // Список выражений для блока            
            var injectingMethods = FindInjectingMethods(type); // Находим методы, помеченные как inject (например, [Inject])

            foreach (var method in injectingMethods) // Для каждого параметра метода вызываем container.Resolve(parameterType)
            {
                var injectingParams = method.GetParameters()
                    .Select(parameterInfo =>
                        Expression.Convert(
                            Expression.Call(
                                containerParam,                   // контейнер передаётся в делегат
                                nameof(Container.Resolve),       // вызов Resolve
                                Type.EmptyTypes,
                                Expression.Constant(parameterInfo.ParameterType) // тип зависимости
                            ),
                            parameterInfo.ParameterType                      // приводим результат к нужному типу
                        )
                    ).ToArray();

                var call = Expression.Call(instanceVar, method, injectingParams); // Вызываем метод на экземпляре с разрешёнными зависимостями
                expressions.Add(call);
            }

            expressions.Add(instanceVar);  // Возвращаем созданный объект            
            var block = Expression.Block(new[] { instanceVar }, expressions); // Создаём блок с переменной instanceVar и всеми выражениями            
            var lambda = Expression.Lambda<Func<Container, object>>(block, containerParam); // Создаём и компилируем лямбду Func<Container, object>
            return lambda.Compile();
        }
        public Action<object, Func<Type, object>> CreateInjectData(MethodInfo methodInfo)
        {
            var targetParam = Expression.Parameter(typeof(object));
            var resolverParam = Expression.Parameter(typeof(Func<Type, object>));

            var parameters = methodInfo.GetParameters();
            var arguments = new Expression[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                var paramType = parameters[i].ParameterType;
                var resolveCall = Expression.Invoke(resolverParam, Expression.Constant(paramType));
                arguments[i] = Expression.Convert(resolveCall, paramType);
            }

            var instanceCast = Expression.Convert(targetParam, methodInfo.DeclaringType!);
            var call = Expression.Call(instanceCast, methodInfo, arguments);

            return Expression.Lambda<Action<object, Func<Type, object>>>(call, targetParam, resolverParam).Compile();
        }
        public IEnumerable<MethodInfo> FindInjectingMethods(Type type)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                                    .Where(m => m.GetCustomAttribute<InjectAttribute>() != null);
        }

        private ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors();

            // 1. Ищем [Inject]
            var injectCtor = constructors
                .FirstOrDefault(c => c.GetCustomAttribute<InjectAttribute>() != null);

            if (injectCtor != null)
                return injectCtor;

            // 2. Если один — берём его
            if (constructors.Length == 1)
                return constructors[0];

            // 3. Если есть пустой — берём его
            var defaultCtor = constructors.FirstOrDefault(c => c.GetParameters().Length == 0);
            if (defaultCtor != null)
                return defaultCtor;

            throw new Exception($"Не удалось выбрать конструктор для {type.Name}");
        }
    }
}
