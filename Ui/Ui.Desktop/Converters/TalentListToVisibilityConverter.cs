using System;
using System.Collections.Generic;
using System.Windows.Data;
using static PhexensWuerfelraum.Logic.Ui.CharacterModel;

namespace PhexensWuerfelraum.Ui.Desktop
{
    internal class TalentListToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || !(value is List<Talent>))
            {
                return false;
            }

            List<Talent> talentList = (List<Talent>)value;

            if (talentList.Count == 0)
            {
                return false;
            }

            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}