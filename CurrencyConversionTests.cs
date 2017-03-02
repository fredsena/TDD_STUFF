using System;
using System.Globalization;
using Xunit;

//https://msdn.microsoft.com/en-us/library/ew0seb73(v=vs.110).aspx
//https://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx

namespace CurrencyConversionTests
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
    }
}
