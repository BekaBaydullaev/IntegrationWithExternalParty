    using CsvHelper;
    using CsvHelper.Configuration;
    using CsvHelper.TypeConversion;
    using System;
    using System.Globalization;

    public class CustomDateTimeConverter : DateTimeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "yyyy/MM/dd", "yyyy/M/d", "MM/dd/yyyy", "M/d/yyyy", "dd.MM.yyyy", "d.M.yyyy", "d.m.yyyy", "yyyy.mm.dd" };

            if (DateTime.TryParseExact(text, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                return date;
            }
            return base.ConvertFromString(text, row, memberMapData);
        }
    }
