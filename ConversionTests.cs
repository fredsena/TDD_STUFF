using System;
using System.Globalization;
using Xunit;
using FluentAssertions;
using Xbehave;

//Decimal.TryParse Method
//https://msdn.microsoft.com/en-us/library/ew0seb73(v=vs.110).aspx

//Standard Numeric Format Strings
//https://msdn.microsoft.com/en-us/library/dwhawy9k(v=vs.110).aspx

//Formatting Types in the .NET Framework
//https://msdn.microsoft.com/en-us/library/26etazsy.aspx

namespace ConversionTests
{
    public class UnitTest
    {        
        [Fact]
        public void GetConcatenatedUrl()
        {
            var relativeUri = "/oauth2/token";
            var baseUrlSlash = "https://test.com/api/v2/";
            var baseUrlNoOSlash = "https://test.com/api";
            
            var result = ConcatenateUrl(baseUrlSlash, relativeUri);
            result.Should().Be("https://test.com/api/v2/oauth2/token");

            var result2 = ConcatenateUrl(baseUrlNoOSlash, "oauth2/token");
            result2.Should().Be("https://test.com/api/oauth2/token");

            var result3 = ConcatenateUrl(baseUrlSlash, "oauth2/token");
            result3.Should().Be("https://test.com/api/v2/oauth2/token");

            var result4 = ConcatenateUrl(baseUrlNoOSlash, "/oauth2/token");
            result4.Should().Be("https://test.com/api/oauth2/token");
        }

        public static string ConcatenateUrl(string baseUrl, string relativeUri)
        {
            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(relativeUri))
            {
                return null;
            }

            var baseUrlResult = baseUrl;
            relativeUri = relativeUri.Replace("//", "/");
            var relativeUriResult = relativeUri;

            int firstSlash = relativeUri.IndexOf('/');

            if (firstSlash == 0)
            {
                relativeUriResult = relativeUri.Substring(firstSlash + 1);
            }           

            int lastSlash = baseUrl.LastIndexOf('/');

            if (lastSlash == baseUrl.Length - 1)
            {
                baseUrlResult = baseUrl.Substring(0, lastSlash);
            }           

            return string.Concat(baseUrlResult, "/", relativeUriResult);
        }        
        
        [Fact]
        public void Should_Remove_Quotes_From_Property()
        {
            var input = "{currency_code:'BRL',country_code:'BR',accounts:[{account_id:'1234',active:true,options:['XAMEX','DEC','JECB','MO','MCE','VESA'],inst:[],is_active:true,ex:[]},{account_id:'4321',active:true,options:['EX','LTA','DEMC','MEC','EUKE','VESA'],inst:[],is_active:false,ex:[]}]}";
            var regex = new Regex(@"account_id:'(.)*?'");
            var matches = regex.Matches(input);

            var resultado = input;

            foreach (Match match in matches)
            {
                foreach (Capture capture in match.Captures)
                {
                    var matchFound = capture.Value;
                    var idealPattern = matchFound.Replace("'", string.Empty);
                    resultado = resultado.Replace(matchFound, idealPattern);
                }
            }

            input.Should().Contain("account_id:'");
            resultado.Should().NotContain("account_id:'");
        }       
        
        
        [Theory]
        [InlineData("60.185,90", "60.185,90")]
        [InlineData("1548.36", "1.548,36")]
        [InlineData("28.80", "28,80")]
        [InlineData("459.12", "459,12")]
        [InlineData("1000.00", "1.000,00")]
        [InlineData("79.7781", "79,77")]
        [InlineData("14131.8198", "14.131,81")]
        [InlineData("2616.2105", "2.616,21")]
        [InlineData("500", "500,00")]
        [InlineData("", "0,00")]
        [InlineData("0", "0,00")]
        [InlineData("...", "0,00")]
        [InlineData("number", "0,00")]
        [InlineData("1q2w3e4r", "0,00")]
        [InlineData("985651413.8198", "985.651.413,81")]
        public void Shoud_Convert_Currency_String_To_BRL(string value, string expectedValue)
        {
            var result = ConvertDecimalStringToBRL(value);

            Assert.True((result.IndexOf(".") > -1 && result.IndexOf(",") > -1) || (result.IndexOf(".") == -1 && result.IndexOf(",") > -1));

            Assert.Equal(result, expectedValue);

            Assert.IsType<decimal>(ConvertBRLCurrencyStringToDecimal(result));
        }
        
        public string ConvertDecimalStringToBRL(string value)
        {            
            if (IsBRLCurrencyString(value)) return value;

            return ConvertToBRLCurrencyString(value);
        }
        
        public bool IsBRLCurrencyString(string stringCurrency)
        {
            decimal result;
            string resultToCompare;

            result = ConvertBRLCurrencyStringToDecimal(stringCurrency);

            resultToCompare = ConvertToBRLCurrencyString(result.ToString());
            
            return (stringCurrency == resultToCompare);          

        }

        public string ConvertToBRLCurrencyString(string value)
        {
            decimal number;

            if (string.IsNullOrEmpty(value.Trim())) value = "0";

            value = value.Replace(".", ",");
            NumberStyles style = NumberStyles.AllowDecimalPoint;

            CultureInfo culture = CultureInfo.CreateSpecificCulture("pt-BR");

            Decimal.TryParse(value, style, culture, out number);

            //comment this line if the decimals results shoud be rounded
            number = Math.Truncate(100 * number) / 100;

            return string.Format(culture, "{0:N2}", number);
        }

        public decimal ConvertBRLCurrencyStringToDecimal(string value)
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
        
        [Scenario]
        [Example("60.185,90")]
        public void Shoud_IsBRLCurrencyString_TRUE(string stringCurrency, decimal result, string resultToCompare)
        {
            "Given this value: (a stringCurrency) {0}"
                .f(() => { });

            "When I convert to decimal"
                .f(() => result = ConvertBRLCurrencyStringToDecimal(stringCurrency));

            "Then I try to convert back to BRLCurrency string"
                .f(() => resultToCompare = ConvertToBRLCurrencyString(result.ToString()));

            "Then the original value and the converted value should be equal"
                .f(() => Assert.Equal(stringCurrency, resultToCompare));            
        }


        [Scenario]
        [Example("60185.90")]
        public void Shoud_IsBRLCurrencyString_FALSE(string stringCurrency, decimal result, string resultToCompare)
        {
            "Given this value: (a stringCurrency) {0}"
                .f(() => { });

            "When I convert to decimal"
                .f(() => result = ConvertBRLCurrencyStringToDecimal(stringCurrency));

            "Then I try to convert back to BRLCurrency string"
                .f(() => resultToCompare = ConvertToBRLCurrencyString(result.ToString()));

            "Then the original value and the converted value should be equal"
                .f(() => Assert.NotEqual(stringCurrency, resultToCompare));
        }
    }
}
