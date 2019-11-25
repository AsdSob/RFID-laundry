using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace PALMS.ViewModels.Common
{
    public static class Helper
    {
        private static List<Type> _types;

        /// <summary>
        /// Get types when contains attribute.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="attributeType">The attribute.</param>
        /// <returns>The collection of types.</returns>
        public static IEnumerable<Type> GetTypesWithAttribute(Assembly assembly, Type attributeType)
        {
            return assembly.GetTypes().Where(type => type.GetCustomAttributes(attributeType, true).Any());
        }

        /// <summary>
        /// Get control by <see cref="HasViewModelAttribute"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="FrameworkElement"/>.</typeparam>
        /// <param name="viewModelType">The view model type.</param>
        /// <returns>The <see cref="FrameworkElement"/>.</returns>
        public static T GetControl<T>(this Type viewModelType) where T : FrameworkElement
        {
            if (_types == null)
            {
                _types = new List<Type>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("PALMS")))
                {
                    var types = GetTypesWithAttribute(assembly, typeof(HasViewModelAttribute)).ToList();
                    if (types.Any())
                        _types.AddRange(types);
                }
                
            }

            var elementType = _types.FirstOrDefault(t =>
            {
                var attribute = t.GetCustomAttribute<HasViewModelAttribute>();

                return attribute != null && attribute.ViewModelType == viewModelType;
            });

            if (elementType != null)
                return Activator.CreateInstance(elementType) as T;

            return null;
        }

        /// <summary>
        /// Run action in main thread.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="dispatcherPriority">The disptcher priority.</param>
        public static void RunInMainThread(this Action action, DispatcherPriority dispatcherPriority = DispatcherPriority.Normal)
        {
            Application.Current?.Dispatcher.Invoke(dispatcherPriority, action);
        }
    }
}
