using System;
using System.Globalization;
using Xunit;
using FluentAssertions;
using Xbehave;

//https://msdn.microsoft.com/en-us/library/ew0seb73(v=vs.110).aspx
//https://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx

namespace ConversionTests
{
    public class UnitTest
    {
        [Theory]
        [InlineData("1548.36")]
        [InlineData("1000.00")]
        [InlineData("28.80")]
        [InlineData("459.12")]
        [InlineData("1000.00")]
        [InlineData("977.76")]
        [InlineData("1013.04")]
        [InlineData("418.68")]
        [InlineData("5870.64")]
        [InlineData("1000.00")]
        [InlineData("4107.3576")]
        [InlineData("2709.3121")]
        [InlineData("79.7781")]
        [InlineData("1317.0419")]
        [InlineData("2709.3121")]
        [InlineData("2601.7597")]
        [InlineData("2634.2554")]
        [InlineData("1194.2286")]
        [InlineData("14131.8198")]
        [InlineData("2616.2105")]
        [InlineData("500")]
        [InlineData("")]
        [InlineData("0")]
        public void ChangeToBRLCurrency(string value)
        {
            decimal number;
            
            if (string.IsNullOrEmpty(value.Trim())) value = "0";
            
            value = value.Replace(".", ",");            
            NumberStyles style = NumberStyles.AllowDecimalPoint;
            
            CultureInfo culture = CultureInfo.CreateSpecificCulture("pt-BR");

            Assert.True(Decimal.TryParse(value, style, culture, out number));

            //comment this line if the decimals results shoud be rounded
            number = Math.Truncate(100 * number) / 100;

            string result = string.Format(culture, "{0:N2}", number);

            Assert.True(
                (result.IndexOf(".") > -1 && result.IndexOf(",") > -1) ||
                (result.IndexOf(".") == -1 && result.IndexOf(",") > -1));

            Assert.IsType<decimal>(ChangeBRLCurrencyToDecimal(result));
        }

        public decimal ChangeBRLCurrencyToDecimal(string value)
        {
            decimal number;
            //string value = "1.345,978";
            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            CultureInfo culture = CultureInfo.CreateSpecificCulture("pt-BR");
            Decimal.TryParse(value, style, culture, out number);
            return number;
        }
        
        [Theory]
        [InlineData("2016-10-25")]
        [InlineData("05/01/2009 14:57:32.8")]
        [InlineData("2009-05-01 14:57:32.8")]
        [InlineData("2009-05-01T14:57:32.8375298-04:00")]
        [InlineData("5/01/2008")]
        [InlineData("5/01/2008 14:57:32.80 -07:00")]
        [InlineData("1 May 2008 2:57:32.8 PM")]
        [InlineData("16-05-2009 1:00:32 PM")]
        [InlineData("Fri, 15 May 2009 20:10:57 GMT")]
        public void Should_TryParse_Date(string stringDate)
        {
            ParseDate(stringDate).Should().NotBeEmpty();
        }

        public string ParseDate(string input)
        {
            string result = "";
            DateTime dateResult;

            if (DateTime.TryParse(input, out dateResult))
            {
                result = dateResult.ToString("dd/MM/yyyy");
            }

            return result;
        }        
    }
}
