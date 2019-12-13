using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Client.Desktop.ViewModels.Common.ViewModels;

namespace Client.Desktop.ViewModels.Common.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<UnitViewModel> GetValues<T>()
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new ArgumentException("T must be an enum");

            var result = new List<UnitViewModel>();
            var attributeType = typeof(DescriptionAttribute);
            foreach (var enumValue in Enum.GetValues(type))
            {
                var memInfo = type.GetMember(enumValue.ToString());
                var attributes = memInfo[0].GetCustomAttributes(attributeType, false);
                if (!(attributes.FirstOrDefault(x => x is DescriptionAttribute) is DescriptionAttribute descriptionAttribute))
                    throw new ArgumentException($"Description attribute for element of {type} not set");
                var description = descriptionAttribute.Description;

                result.Add(new UnitViewModel((int)enumValue, description));
            }
            return result;
        }
    }

    public class UnitViewModel : ViewModelBase
    {
        public int Id { get; }
        public string Name { get; }

        public UnitViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

}
