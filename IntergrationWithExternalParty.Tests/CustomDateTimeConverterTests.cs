using Microsoft.VisualStudio.TestTools.UnitTesting;
using CsvHelper.Configuration;
using System;

[TestClass]
public class CustomDateTimeConverterTests
{
    private readonly CustomDateTimeConverter _converter = new CustomDateTimeConverter();

    [TestMethod]
    [DataRow("01/12/2022", 2022, 12, 01)]
    [DataRow("1/1/2022", 2022, 1, 1)]
    [DataRow("2022/01/12", 2022, 01, 12)]
    [DataRow("2022/1/1", 2022, 1, 1)]
    [DataRow("01/12/2022", 2022, 12, 01)]
    [DataRow("1/1/2022", 2022, 1, 1)]
    [DataRow("12.12.2021", 2021, 12, 12)]
    [DataRow("9.7.1986", 1986, 7, 9)]
    [DataRow("07.1.2012", 2012, 1, 07 )]
    [DataRow("9.08.2002", 2002, 09, 8 )]
    public void CustomDateTimeConverter_ShouldConvertValidDateFormats(string inputDate, int expectedYear, int expectedMonth, int expectedDay)
    {
        // Arrange
        var memberMapData = new MemberMapData(null); // Can be null for this test

        // Act
        var convertedDate = (DateTime)_converter.ConvertFromString(inputDate, null, memberMapData); // We're not using IReaderRow, so it can be null

        // Assert
        Assert.AreEqual(expectedYear, convertedDate.Year);
        Assert.AreEqual(expectedMonth, convertedDate.Month);
        Assert.AreEqual(expectedDay, convertedDate.Day);
    }

    [TestMethod]
    [DataRow("some-invalid-date")]
    [DataRow("13/25/2022")]  // Invalid month and day
    [DataRow("12.78.0218")]
    [DataRow("08.18.1918")]
    [DataRow("7.13.2018")]
    [DataRow("2000.1.18")]
    [DataRow("2019.19.10")]
    public void CustomDateTimeConverter_ShouldHandleInvalidDateFormats(string inputDate)
    {
        // Arrange
        var memberMapData = new MemberMapData(null); // Can be null for this test

        // Act & Assert
        Assert.ThrowsException<Exception>(() => _converter.ConvertFromString(inputDate, null, memberMapData));
    }
}
