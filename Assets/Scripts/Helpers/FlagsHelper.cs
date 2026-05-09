using System;

namespace Helpers
{
    public static class FlagsHelper
    {
        /// <summary>Проверяет, установлен ли флаг.</summary>
        public static bool ContainsAny<T>(T value, T flag) where T : Enum
        {
            return (Convert.ToInt32(value) & Convert.ToInt32(flag)) != 0;
        }

        /// <summary>Добавляет флаг к текущему значению (на месте).</summary>
        public static void AddFlag<T>(ref T value, T flag) where T : Enum
        {
            value = (T)(object)((Convert.ToInt32(value) | Convert.ToInt32(flag)));
        }

        /// <summary>Удаляет флаг из текущего значения (на месте).</summary>
        public static void RemoveFlag<T>(ref T value, T flag) where T : Enum
        {
            value = (T)(object)((Convert.ToInt32(value) & ~Convert.ToInt32(flag)));
        }

        /// <summary>Объединяет несколько флагов (создает новое значение).</summary>
        public static T CombineFlags<T>(params T[] flags) where T : Enum
        {
            int result = 0;
            foreach (var flag in flags)
                result |= Convert.ToInt32(flag);
            return (T)(object)result;
        }

        /// <summary>Сбрасывает текущее значение и устанавливает новые флаги (на месте).</summary>
        public static void SetFlags<T>(ref T value, params T[] flags) where T : Enum
        {
            value = CombineFlags(flags);
        }
    }

    public static class FlagsExtensions
    {
        /// <summary>
        /// Есть ли хотя бы одно совпадение флагов.
        /// </summary>
        public static bool ContainsAny<T>(this T value, T flags)
            where T : Enum
        {
            return (Convert.ToInt32(value) & Convert.ToInt32(flags)) != 0;
        }

        /// <summary>
        /// Содержит ли значение все указанные флаги.
        /// Аналог Enum.HasFlag().
        /// </summary>
        public static bool ContainsAll<T>(this T value, T flags)
            where T : Enum
        {
            int v = Convert.ToInt32(value);
            int f = Convert.ToInt32(flags);

            return (v & f) == f;
        }

        /// <summary>
        /// Добавляет флаги.
        /// </summary>
        public static T AddFlags<T>(this T value, T flags)
            where T : Enum
        {
            return (T)(object)(Convert.ToInt32(value) | Convert.ToInt32(flags));
        }

        /// <summary>
        /// Удаляет флаги.
        /// </summary>
        public static T RemoveFlags<T>(this T value, T flags)
            where T : Enum
        {
            return (T)(object)(Convert.ToInt32(value) & ~Convert.ToInt32(flags));
        }

        /// <summary>
        /// Содержит ли определенный флаг
        /// </summary>
        public static bool Contains<T>(this T value, T flag)
            where T : Enum
        {
            return (Convert.ToInt32(value) & Convert.ToInt32(flag))
                   == Convert.ToInt32(flag);
        }
    }
}

