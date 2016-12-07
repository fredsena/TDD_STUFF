using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace UnitTests
{
    public class MiscTests
    {
        private readonly ITestOutputHelper output;

        public MiscTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("6 - ORDINARIA")]        
        [InlineData("50 - CAUTELAR INOMINADA")]           
        [InlineData("30 - DECLARATORIA")]
        [InlineData("5 - CONSIGNACAO EM PAGAMENTO")]
        [InlineData("61 - CAUTELAR INCIDENTAL")]
        [InlineData("74 - RESPONSABILIDADE SECURITARIA")]
        [InlineData("87 - AÇÃO ORDINARIA / COBRANÇA  JUR")]
        [InlineData("45 - REVISIONAL DE CONTRATO")]
        [InlineData("44 - EXIBICAO DE DOCUMENTOS")]
        [InlineData("23 - REPARACAO DE DANOS")]
        [InlineData("67 - CAUTELAR PROUCAO ANTEC. PROVAS")]
        [InlineData("9 - PRODUCAO ANTECIPADA PROVAS")]
        [InlineData("21 - DECLARATORIA QUITACAO DEBITO")]
        public void AlignCrazyLayout(string text)
        {
            string result = CrazyAlign(text);
            result.Should().HaveLength(25);
            output.WriteLine("|" + result + "| Lenght = " + result.Length);
        }

        public string CrazyAlign(string value)
        {
            var qtd = value.Count(Char.IsDigit);
            string valor = value.PadLeft(value.Length + (4-qtd), ' ');

            if (valor.Length > 25)
            {
                valor = valor.Substring(0, 25);
            }

            valor = valor + new String(' ', 25 - valor.Length);
            return valor;
        }

        [Fact]
        public void RegexTest()
        {
            string pattern = @"^(0[1-9]|1[0-2])[/]([1-2][0-9][0-9][0-9])$";
            RegexOptions regexOptions = RegexOptions.None;
            Regex regex = new Regex(pattern, regexOptions);
            string inputData = @"12/2013";
            Assert.True(regex.IsMatch(inputData));
        }
    }
}
