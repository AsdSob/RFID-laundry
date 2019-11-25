using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using PALMS.ViewModels.Common.Enumerations;

namespace PALMS.ViewModels.Common.Extensions
{
    public static class EnumExtentions
    {
    /// <summary>
    /// 
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
}
