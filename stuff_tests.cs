
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ClientService.intranet.;


namespace ClientService
{
    public class ApiConfig
    {
        public static XYZ2_Service Api
        {
            get
            {
                var config = new XYZ2_Service();

                //ServicePointManager.ServerCertificateValidationCallback = DisableCertificateValidation;

                ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

                CredentialCache serviceCredentials = new CredentialCache();
                NetworkCredential networkCredential = new NetworkCredential("zzz", "ppp");
                serviceCredentials.Add(new Uri(config.Url), "Basic", networkCredential);
                config.Credentials = serviceCredentials;


                return config;
            }
        }

        static bool DisableCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors error)
        {
            // Ignore errors
            return true;
        }


        static bool CertificateValidationCallBack(
         object sender,
         System.Security.Cryptography.X509Certificates.X509Certificate certificate,
         System.Security.Cryptography.X509Certificates.X509Chain chain,
         System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            //var hash = X509Certificate.CreateFromCertFile("mycert.cer").GetCertHashString();

            //if (hash.Contains(certificate.GetCertHashString()))
            //{
            //    return true;
            //}
            

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace UnitTest_ClientService.LogTests
{
    /// <summary>
    /// Logger for sending output to the console.
    /// </summary>

    [ExtensionUri("logger://Logger")] /// Uri used to uniquely identify the console logger. 
    [FriendlyName("")] /// Alternate user friendly string to uniquely identify the logger.
    
    public class Logger : ITestLogger
    {

        /// <summary>
        /// Initializes the Test Logger.
        /// </summary>
        /// <param name="events">Events that can be registered for.</param>
        /// <param name="testRunDirectory">Test Run Directory</param>
        /// 
        public void Initialize(TestLoggerEvents events, string testRunDirectory)
        {
            // Register for the events.
            events.TestRunMessage += TestMessageHandler;
            events.TestResult += TestResultHandler;
            events.TestRunComplete += TestRunCompleteHandler;
        }

        /// <summary>
        /// Called when a test message is received.
        /// </summary>
        private void TestMessageHandler(object sender, TestRunMessageEventArgs e)
        {

            switch (e.Level)
            {
                case TestMessageLevel.Informational:
                    Console.WriteLine("Information: " + e.Message);
                    break;

                case TestMessageLevel.Warning:
                    Console.WriteLine("Warning: " + e.Message);
                    break;

                case TestMessageLevel.Error:
                    Console.WriteLine("Error: " + e.Message);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Called when a test result is received.
        /// </summary>
        private void TestResultHandler(object sender, TestResultEventArgs e)
        {
            string name = !string.IsNullOrEmpty(e.Result.DisplayName) ? e.Result.DisplayName : e.Result.TestCase.FullyQualifiedName;

            if (e.Result.Outcome == TestOutcome.Skipped)
            {
                Console.WriteLine(name + " Skipped");
            }

            else if (e.Result.Outcome == TestOutcome.Failed)
            {
                Console.WriteLine(name + " Failed");

                if (!String.IsNullOrEmpty(e.Result.ErrorStackTrace))
                {
                    Console.WriteLine(e.Result.ErrorStackTrace);
                }
            }

            else if (e.Result.Outcome == TestOutcome.Passed)
            {
                Console.WriteLine(name + " Passed");
            }
        }

        /// <summary>
        /// Called when a test run is completed.
        /// </summary>
        private void TestRunCompleteHandler(object sender, TestRunCompleteEventArgs e)
        {
            Console.WriteLine("Total Executed: {0}", e.TestRunStatistics.ExecutedTests);
            Console.WriteLine("Total Passed: {0}", e.TestRunStatistics[TestOutcome.Passed]);
            Console.WriteLine("Total Failed: {0}", e.TestRunStatistics[TestOutcome.Failed]);
            Console.WriteLine("Total Skipped: {0}", e.TestRunStatistics[TestOutcome.Skipped]);
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration_Test_ClientService.Helpers
{
    public class Helper
    {
        CultureInfo cultureInfo = new CultureInfo("en-US");

        private double RandomNumberBetween(double minValue, double maxValue)
        {
            Random random = new Random();
            var next = random.NextDouble();
            return minValue + (next * (maxValue - minValue));
        }

        public string RandomDecimalString(double minValue, double maxValue)
        {
            return RandomNumberBetween(minValue, maxValue).ToString("0.00", cultureInfo);            
        }
        
        public string RandomDecimal(double minValue, double maxValue)
        {
            return Convert.ToDecimal(RandomNumberBetween(minValue, maxValue).ToString("0.00", CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);
        }        
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Linq;
using Microsoft.SqlServer.Server;
using ClientService.Common.Enums;

namespace ClientService.Common.Helpers
{
    public class Helpers
    {
        public static string MENSAGEM_ERRO = "Houve uma exceção no retorno dos dados. Envie esta tela de erro à Equipe do System.";
        public static string GeraDadosRetornoParametros(Object objRetorno)
        {
            var dadosRetorno = new StringBuilder();

            foreach (PropertyInfo property in objRetorno.GetType().GetProperties())
            {
                object value = property.GetValue(objRetorno, null);
                dadosRetorno.AppendFormat(" \n {0} = {1} ", property.Name, value);
            }

            return dadosRetorno.ToString();
        }

        public static void EnviaSqlDataRecordOcorrencia(StatusRetornoService statusRetornoService, string mensagem)
        {
            SqlDataRecord sqlDataRecord = new SqlDataRecord(MontaMetadataOcorrencia().ToArray());
            sqlDataRecord.SetValue(0, (int)statusRetornoService);
            sqlDataRecord.SetValue(1, mensagem);
            SqlContext.Pipe.Send(sqlDataRecord);
        }


        private static List<SqlMetaData> MontaMetadataOcorrencia()
        {
            var sqlMetaData = new List<SqlMetaData>();

            sqlMetaData.Add(new SqlMetaData("CODRETORNO", SqlDbType.Int));
            sqlMetaData.Add(new SqlMetaData("MSGRETORNO", SqlDbType.NVarChar, -1));

            return sqlMetaData;
        }

        public static bool AllPropertiesNull(Object obj)
        {
            return obj.GetType().GetProperties()
                .Where(pi => pi.GetValue(obj) is string)
                .Select(pi => (string)pi.GetValue(obj))
                .All(value => String.IsNullOrEmpty(value));
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest_ClientService.LogTests
{
    interface ITestLogger
    {
        /// <summary>
        /// Initializes the Test Logger.
        /// </summary>
        /// 
        /// <param name="events">Events that can be registered for.</param>
        /// <param name="testRunDirectory">Test Run Directory</param>

        void Initialize(TestLoggerEvents events, string testRunDirectory);
    }
}
using System;
using Xunit;
using ClientService.intranet.;
using ClientService.Common.Enums;
using Xunit.Abstractions;
using Integration_Test_ClientService;
using Integration_Test_ClientService.Fixtures;
using Integration_Test_ClientService.Helpers;
using System.Diagnostics;

namespace Integration_Test_ClientService.PROC004
{
    [TestCaseOrderer("Integration_Test_ClientService.Helpers.PriorityOrderer", "Integration_Test_ClientService")]
    public class IT_CancelarCausa : IDisposable, IClassFixture<Fixture>
    {
        string apolsini;

        private readonly ITestOutputHelper output;
        Fixture fixture;
        RetornoService retorno;
        PROC004_Input dados;
        XYZ2_IncluirOcorrenciaEntrada dadosEntrada;

        //Fez um aviso, não executou pagamento e deseja cancelar o  como 
        public IT_CancelarCausa(ITestOutputHelper output, Fixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
            retorno = new RetornoService();
            dados = new PROC004_Input();
            dadosEntrada = new XYZ2_IncluirOcorrenciaEntrada();
            apolsini = "Z9120000002";
        }

        [Fact, TestPriority(0)]
        public void Executa_Cancelamento_X_()
        {
            string nomeTeste = new StackTrace().GetFrame(0).GetMethod().Name.ToString();
            string numprosiTemp = "DF " + new Random().Next(1000, 9999).ToString();

            dadosEntrada = dados.X(parametro);
            dadosEntrada.codusu = "X08684";
            dadosEntrada.numprosi = numprosiTemp;

            retorno = fixture.ExecutaMovimentacao(dadosEntrada, "Executa_X_");

            string mensagem = "Executa_X_: \n Mensagem: " + retorno.msgret + " \n Ocorr.Historico: " + retorno.numop + " \n Num Processo: " + numprosiTemp + " \n";
            output.WriteLine(mensagem);

            if (string.IsNullOrEmpty(retorno.numop))
            {
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
            }
            else
            {
                dadosEntrada = dados.Cancelamento(parametro, retorno.numop, "8304");
                dadosEntrada.codusu = "X06173";
                dadosEntrada.numprosi = numTemp;

                retorno = fixture.ExecutaMovimentacao(dadosEntrada, nomeTeste);

                mensagem = nomeTeste + ": \n Mensagem: " + retorno.msgret + " \n Ocorr.Historico: " + retorno.numop + " \n Num Processo: " + numTemp;
                output.WriteLine(mensagem);
                
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());           
            }
        }

        
        //[Fact]
        public void Nao_Deve_Executar_Cancelamento_Pre_X()
        {
            dadosEntrada.parametro = parametro;
            dadosEntrada.ocorhist = "17";
            dadosEntrada.operacao = "8151";
            dadosEntrada.codusu = "X10542";

            retorno = fixture.ExecutaMovimentacao(dadosEntrada, new StackTrace().GetFrame(0).GetMethod().Name.ToString(), StatusRetornoService.RETORNO_SEM_DADOS);
            output.WriteLine(retorno.msgret);
            Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_SEM_DADOS).ToString());
        }

        public void Dispose()
        {
            retorno = null;
            dadosEntrada = null;
            dados = null;
        }
    }
}
﻿using System;
using Xunit;
using ClientService.intranet.;
using ClientService.Common.Enums;
using Xunit.Abstractions;
using Integration_Test_ClientService.Fixtures;
using Integration_Test_ClientService.Helpers;
using System.Diagnostics;


namespace Integration_Test_ClientService.PROC004
{
    [TestCaseOrderer("Integration_Test_ClientService.Helpers.PriorityOrderer", "Integration_Test_ClientService")]
    public class IT_CancelarPagamento : IDisposable, IClassFixture<Fixture>
    {
        string parametro;
        private readonly ITestOutputHelper output;        

        Fixture fixture;
        RetornoService retorno;
        PROC004_Input dados;
        XYZ2_IncluirOcorrenciaEntrada dadosEntrada;

        public IT_CancelarPagamento(ITestOutputHelper output, Fixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
            retorno = new RetornoService();
            dados = new PROC004_Input();
            dadosEntrada = new XYZ2_IncluirOcorrenciaEntrada();
            parametro = "Z3120000012";
        }

        [Fact, TestPriority(0)]
        public void Executa_X_()
        {
            dadosEntrada = dados.X(parametro);
            dadosEntrada.codusu = "X08684"; //"X06173";

            retorno = fixture.ExecutaMovimentacao(dadosEntrada, new StackTrace().GetFrame(0).GetMethod().Name.ToString());
            output.WriteLine(retorno.msgret);

            if (retorno.msgret == "Operação de X  já foi incluída")
            {
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_SEM_DADOS).ToString());
            }
            else
            {
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
            }
        }

        [Theory, TestPriority(3)]
        [InlineData("8501")]        [InlineData("8502")]        [InlineData("8503")]        [InlineData("8504")]        [InlineData("8505")]
        [InlineData("8506")]        [InlineData("8507")]        [InlineData("8508")]        [InlineData("8509")]        [InlineData("8510")]
        [InlineData("8511")]        [InlineData("8512")]        [InlineData("8513")]        [InlineData("8514")]        [InlineData("8515")]
        [InlineData("8516")]        [InlineData("8517")]        [InlineData("8518")]        [InlineData("8519")]        [InlineData("8520")]
        [InlineData("8521")]        [InlineData("8522")]        [InlineData("8523")]
        public void CANCELA_Pagto_Honorarios(string operacao)
        {
            string nomeTeste = new StackTrace().GetFrame(0).GetMethod().Name.ToString();

            retorno = fixture.Executa(dados.PagamentoHonorarios(parametro, operacao), "Executa_Pagto_X");

            output.WriteLine("Executa_Pagto_X: \nMensagem: " + retorno.msgret + "\nOcorr.Historico: " + retorno.numop + "\n");

            if (string.IsNullOrEmpty(retorno.numop))
            {
                output.WriteLine("Executa_Pagto_X: \nMensagem: " + retorno.msgret + "\nOcorr.Historico: " + retorno.numop + "\n");
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
            }
            else
            {
                retorno = fixture.Executa(dados.Cancelamento(parametro, retorno.numop, "8581"), nomeTeste);

                output.WriteLine(nomeTeste + ": \nMensagem: " + retorno.msgret + "\nOcorr.Historico: " + retorno.numop + "\n");

                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
            }
        }

        public void Dispose()
        {
            retorno = null;
            dadosEntrada = null;
        }
    }
}

﻿using System;
using Xunit;
using ClientService.intranet.;
using ClientService.Common.Enums;
using Xunit.Abstractions;
using Integration_Test_ClientService;
using Integration_Test_ClientService.Fixtures;
using Integration_Test_ClientService.Helpers;
using System.Diagnostics;


namespace Integration_Test_ClientService.PROC004
{
    [TestCaseOrderer("Integration_Test_ClientService.Helpers.PriorityOrderer", "Integration_Test_ClientService")]
    public class IT_EncerrarProcesso : IDisposable, IClassFixture<Fixture>
    {
        string parametro;
        private readonly ITestOutputHelper output;
        Fixture fixture;
        PROC004_Input dados;
        RetornoService retorno;
        XYZ2_IncluirOcorrenciaEntrada dadosEntrada;
        public IT_EncerrarProcesso(ITestOutputHelper output, Fixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
            retorno = new RetornoService();
            dados = new PROC004_Input();
            dadosEntrada = new XYZ2_IncluirOcorrenciaEntrada();
            parametro = "Z1420000006";
        }

        [Fact, TestPriority(0)]
        public void Executa_Encerramento_Processo_()
        {
            string nomeTeste = new StackTrace().GetFrame(0).GetMethod().Name.ToString();
            string numTemp = "DF " + new Random().Next(1000, 9999).ToString();

            dadosEntrada = dados.X(parametro);
            dadosEntrada.codusu = "X08684"; 
            dadosEntrada.num = numTemp;

            retorno = fixture.Executa(dadosEntrada, "Executa_X_");

            string mensagem = "Executa_X_: \n Mensagem: " + retorno.msgret + " \n Ocorr.Historico: " + retorno.numop + " \n Num Processo: " + numTemp + " \n";
            output.WriteLine(mensagem);

            if (string.IsNullOrEmpty(retorno.numop))
            {
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
            }
            else
            {
                dadosEntrada = dados.Tipo(parametro, retorno.numop, "8302");
                dadosEntrada.codusu = "X06173";
                dadosEntrada.num = numTemp;

                retorno = fixture.Executa(dadosEntrada, nomeTeste);

                mensagem = nomeTeste + ": \n Mensagem: " + retorno.msgret + " \n Ocorr.Historico: " + retorno.numop + " \n Num Processo: " + numTemp;
                output.WriteLine(mensagem);

                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
            }
        }

        //[Fact]
        //TODO: Executa_Encerramento_Tipo_ e realizar  -> gerar ERRO
        public void Executa_Encerramento()
        {
            dadosEntrada.parametro = parametro;
            dadosEntrada.h = "2";
            dadosEntrada.operacao = "8302";
            dadosEntrada.codusu = "X10542";

            retorno = fixture.Executa(dadosEntrada, new StackTrace().GetFrame(0).GetMethod().Name.ToString());
            output.WriteLine(retorno.msgret);
            Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
        }

        public void Dispose()
        {
            retorno = null;
            dadosEntrada = null;
            dados = null;
        }
    }
}﻿

using System;
using Xunit;
using ClientService.intranet.;
using ClientService.Common.Enums;
using Xunit.Abstractions;
using Integration_Test_ClientService.Fixtures;
using Integration_Test_ClientService.Helpers;
using System.Diagnostics;


namespace Integration_Test_ClientService.PROC004
{
    [TestCaseOrderer("Integration_Test_ClientService.Helpers.PriorityOrderer", "Integration_Test_ClientService")]
    public class IT_Estornar : IDisposable, IClassFixture<Fixture>
    {
        string parametro;
        private readonly ITestOutputHelper output;
        Fixture fixture;
        RetornoService retorno;
        PROC004_Input dados;
        XYZ2_IncluirOcorrenciaEntrada dadosEntrada;

        public IT_Estornar(ITestOutputHelper output, Fixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
            retorno = new RetornoService();
            dados = new PROC004_Input();
            dadosEntrada = new XYZ2_IncluirOcorrenciaEntrada();
            parametro = "Z1420000007";
        }

        [Fact, TestPriority(0)]
        public void Executa_X_()
        {
            dadosEntrada = dados.X(parametro);
            dadosEntrada.codusu = "X08684"; //"X06173";

            retorno = fixture.Executa(dadosEntrada, new StackTrace().GetFrame(0).GetMethod().Name.ToString());
            output.WriteLine(retorno.msgret);

            if (retorno.msgret == "Operação de X  já foi incluída")
            {
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_SEM_DADOS).ToString());
            }
            else
            {
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
            }
        }


        public void Dispose()
        {
            retorno = null;
            dadosEntrada = null;
        }
    }
}﻿

using System;
using Xunit;
using ClientService.intranet.;
using ClientService.Common.Enums;
using Xunit.Abstractions;
using Integration_Test_ClientService.Fixtures;
using Integration_Test_ClientService.Helpers;
using System.Diagnostics;

namespace Integration_Test_ClientService.PROC004
{
    [TestCaseOrderer("Integration_Test_ClientService.Helpers.PriorityOrderer", "Integration_Test_ClientService")]
    public class IT_Pagto : IDisposable, IClassFixture<Fixture>
    {
        string parametro;
        private readonly ITestOutputHelper output;
        Fixture fixture;
        RetornoService retorno;
        XYZ2_IncluirOcorrenciaEntrada dadosEntrada;
        PROC004_Input dados;

        public IT_Pagto(ITestOutputHelper output, Fixture fixture)
        {
            this.output = output;
            this.fixture = fixture;
            retorno = new RetornoService();
            dadosEntrada = new XYZ2_IncluirOcorrenciaEntrada();
            dados = new PROC004_Input();
            parametro = "Z1420000006";//"Z1420000006";//Z9120000002
        }

        [Fact, TestPriority(0)]
        public void Executa_X_()
        {
            retorno = fixture.Executa(dados.X(parametro), new StackTrace().GetFrame(0).GetMethod().Name.ToString());
            output.WriteLine(retorno.msgret);

            if (retorno.msgret == "Operação de X  já foi incluída")
            {
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_SEM_DADOS).ToString());
            }
            else
            {
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
            }
        }

        [Fact, TestPriority(1)]
        public void Executa__X()
        {
            retorno = fixture.Executa(dados.X(parametro), new StackTrace().GetFrame(0).GetMethod().Name.ToString());
            output.WriteLine(retorno.msgret);
            Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
        }

        [Fact, TestPriority(2)]
        public void Executa_X_()
        {

            XYZ2_IncluirOcorrenciaEntrada dados1;
            dados1 = dados.X(parametro);

            retorno = fixture.Executa(dados1, "X");

            retorno = fixture.Executa(dados.X(parametro, retorno.numop), new StackTrace().GetFrame(0).GetMethod().Name.ToString());
            output.WriteLine(retorno.msgret);
            Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
        }       


        [Theory, TestPriority(6)]
        [InlineData("8101", "8102")]
        [InlineData("8105", "8106")]
        [InlineData("8109", "8110")]
        [InlineData("8113", "8114")]
        [InlineData("8117", "8118")]
        [InlineData("8120", "8121")]
        [InlineData("8123", "8124")]
        [InlineData("8126", "8127")]
        public void Executa_Pagto_Tipo__(string Operacao, string Operacao)
        {
            XYZ2_IncluirOcorrenciaEntrada dados1;
            dados1 = dados.X(parametro, Operacao);

            retorno = fixture.Executa(dados1, "X_Tipo");
            output.WriteLine("X_Tipo: \nMensagem: " + retorno.msgret + "\nOcorr.Historico: " + retorno.numop + "\n");

            if (string.IsNullOrEmpty(retorno.numop))
            {
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
            }
            else
            {
                retorno = fixture.Executa(dados.X(parametro, retorno.numop, Operacao), new StackTrace().GetFrame(0).GetMethod().Name.ToString());
                output.WriteLine("X_Tipo: \nMensagem: " + retorno.msgret + "\nOcorr.Historico: " + retorno.numop);
                Assert.True(retorno.codret == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString());
            }
        }       


        public void Dispose()
        {
            dados = null;
            dadosEntrada = null;
        }
    }
}

﻿using ClientService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration_Test_ClientService.Helpers
{
    public class PROC004_Input : XYZ2_IncluirOcorrenciaEntrada
    {
        public PROC004_Input()
        {
            dtpagto = DateTime.Today.ToString("yyyy-MM-dd");
        }

        public XYZ2_IncluirOcorrenciaEntrada X(string Num)
        {
            XYZ2_IncluirOcorrenciaEntrada dadosEntrada = new XYZ2_IncluirOcorrenciaEntrada();

            dadosEntrada.parametro = Num;
            dadosEntrada.h = this.h;
            dadosEntrada.autorcau = this.autorcau;
            dadosEntrada.codusu = this.codusu;
            dadosEntrada.valoper = new Helper().RandomDecimalString(200.65, 510.21);

            return dadosEntrada;
        }

        public XYZ2_IncluirOcorrenciaEntrada X(string Num, string CodOperacao)
        {
            XYZ2_IncluirOcorrenciaEntrada dadosEntrada = new XYZ2_IncluirOcorrenciaEntrada();

            dadosEntrada.parametro = Num;
            dadosEntrada.h = this.h;
            dadosEntrada.codusu = this.codusu;
            dadosEntrada.operacao = CodOperacao;
            dadosEntrada.valoper = new Helper().RandomDecimalString(4.13, 15.99);
            return dadosEntrada;
        }
    }
}


﻿
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using ClientService;
using System.Text;
using System.Globalization;
using ClientService.Common.Helpers;
using ClientService.Common.Ehs;

public partial class 
{
    [SqlProcedure]
    public static void X003(SqlString hero)
    {
        try
        {
            var dadosEntrada = new XYZ2_ObterEntrada();
            dadosEntrada.id = hero.Value;

            var  = ClientService.ApiConfig.Api.obter(new obter { dadosEntrada = dadosEntrada }).retorno;

            if (!Helpers.AllPropertiesNull())
            {
                EnviaDados();
            }
            else
            {
                Helpers.EnviaSqlDataRecordOcorrencia(StatusRetornoService.RETORNO_SEM_DADOS, 
                        string.Format("Não foram encontradas informações do () nº {0}", hero.Value));
            }
        }
        catch (Exception ex)
        {
            Helpers.EnviaSqlDataRecordOcorrencia(StatusRetornoService.RETORNO_ERRO, 
                string.Format("{0}\n\n{1}", Helpers.MENSAGEM_ERRO, ex.ToString()));
        }
    }

    private static void EnviaDados(XYZ2_ObterRetorno retorno)
    {
        var sqlMetaData = MontaMetadata_X003();
        var sqlDataRecord = new SqlDataRecord(sqlMetaData.ToArray());

        try
        {
            sqlDataRecord.SetValue(0, (int)StatusRetornoService.RETORNO_COM_DADOS);
            sqlDataRecord.SetValue(1, "");

            sqlDataRecord.SetValue(2, retorno.parametro);
            sqlDataRecord.SetValue(3, retorno.fonte);


            SqlContext.Pipe.Send(sqlDataRecord);
        }
        catch (Exception ex)
        {
            Helpers.EnviaSqlDataRecordOcorrencia(StatusRetornoService.RETORNO_ERRO,
                  string.Format("{0}\n\n{1}\n{2}", Helpers.MENSAGEM_ERRO, ex.ToString(), Helpers.GeraDadosRetornoParametros(retorno)));
        }
    }    

    private static List<SqlMetaData> MontaMetadata_X003()
    {
        var sqlMetaData = new List<SqlMetaData>();

        sqlMetaData.Add(new SqlMetaData("a", SqlDbType.Int));
        sqlMetaData.Add(new SqlMetaData("b", SqlDbType.NVarChar, -1));
        sqlMetaData.Add(new SqlMetaData("c", SqlDbType.NVarChar, 13));
        sqlMetaData.Add(new SqlMetaData("d", SqlDbType.NVarChar, 2));
        sqlMetaData.Add(new SqlMetaData("e", SqlDbType.Date));
        sqlMetaData.Add(new SqlMetaData("f", SqlDbType.Decimal, 15, 2));
        return sqlMetaData;
    }      
}


﻿
using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using ClientService;
using System.Text;
using System.Globalization;
using ClientService.Common.Helpers;
using ClientService.Common.Ehs;

public partial class 
{


    [SqlProcedure]
    public static void X004(SqlString parametro, SqlString h,SqlString x)
    {

        var dadosEntrada = new XYZ2_IncluirOcorrenciaEntrada();

        try
        {
            dadosEntrada.parametro = parametro.Value;
            dadosEntrada.h = h.Value;


            var retornoLancamento = ClientService.ApiConfig
                .Api.incluirOcorrencia(new incluirOcorrencia { dadosEntrada = dadosEntrada })
                .retorno;

            if (retornoLancamento.codret.Trim() == ((int)StatusRetornoService.RETORNO_COM_DADOS).ToString())
            {
                EnviaDadosRetorno(retornoLancamento);
            }
            else
            {
                Helpers.EnviaSqlDataRecordOcorrencia(StatusRetornoService.RETORNO_SEM_DADOS,
                    string.Format("Mensagem retorno : {0}\n\nDados enviados pelo System para {1}",                     
                    retornoLancamento.msgret,
                    Helpers.GeraDadosRetornoParametros(dadosEntrada)));
            }
        }
        catch (Exception ex)
        {
            Helpers.EnviaSqlDataRecordOcorrencia(StatusRetornoService.RETORNO_ERRO,
                string.Format("{0}\n\n{1}\n\nDados de envio System para deware: {2}", Helpers.MENSAGEM_ERRO, ex.ToString(), Helpers.GeraDadosRetornoParametros(dadosEntrada)));
        }
    }

    private static void EnviaDadosRetorno(XYZ2_IncluirOcorrenciaRetorno retorno)
    {
        var sqlMetaData = MontaMetadata_X004();
        var sqlDataRecord = new SqlDataRecord(sqlMetaData.ToArray());

        try
        {
            sqlDataRecord.SetValue(0, (int)StatusRetornoService.RETORNO_COM_DADOS);
            sqlDataRecord.SetValue(1, "");
            sqlDataRecord.SetValue(2, retorno.hop);
            sqlDataRecord.SetValue(3, retorno.cod);            

            SqlContext.Pipe.Send(sqlDataRecord);
        }
        catch (Exception ex)
        {
            Helpers.EnviaSqlDataRecordOcorrencia(StatusRetornoService.RETORNO_ERRO,
                  string.Format("{0}\n\n{1}\n{2}", Helpers.MENSAGEM_ERRO, ex.ToString(), Helpers.GeraDadosRetornoParametros(retorno)));
        }
    }
}


 ﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Integration_Test_ClientService.Helpers
{
    public class Logger
    {
        private string folder { get; set; }


        public Logger()
        {
            folder = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())),"LogTests");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public void WriteToFile(string text, string filename)
        {
            string logFileName = Path.Combine(folder, filename + "_" + DateTime.Now.ToString("ddMMyyyy_HH_mm_ss") + "_" + new Random().Next(799, 999).ToString() + ".txt");
            File.WriteAllText(logFileName, text.Substring(0, text.Length), Encoding.Default);            
        }

        public static string GetInlineParamData(Object objRet)
        {
            var dadosRet = new StringBuilder();

            foreach (PropertyInfo property in objRet.GetType().GetProperties())
            {
                object value = property.GetValue(objRet, null);
                dadosRet.AppendFormat("{0} = {1} | ", property.Name, value);
            }

            return dadosRet.ToString();
        }

        public async Task WriteTextAsync(string text, string filePath)
        {
            byte[] encodedText = Encoding.Unicode.GetBytes(text);

            string logFileName = Path.Combine(folder, filePath + "_" + DateTime.Now.ToString("ddMMyyyy_HH_mm_ss") + "_" + new Random().Next(799, 999).ToString() + ".sql");

            using (FileStream sourceStream = new FileStream(logFileName,
                FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
                await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);                
            };
        }
    }
}
﻿using System;
using System.IO;
using System.Text;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using System.Globalization;

namespace Integration_Test_ClientService.Helpers
{
    public class Logger_Tests : IDisposable
    {
        private readonly ITestOutputHelper output;
        string folder;
        Logger log;
     
        public Logger_Tests(ITestOutputHelper output)
        {
            this.output = output;
            log = new Logger();
            folder = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())), "LogTests");
        }

        [Fact]
        public void Shoud_Create_Folder()
        {
            output.WriteLine("Log test");
            Assert.True(Directory.Exists(folder));
        }

        [Fact]
        public void Shoud_WriteToFile()
        {
            output.WriteLine("Log test");
            log.WriteToFile("this is a test text", "TEST_WRITE_FILE");
            Assert.True(Directory.GetFiles(folder, "*TEST_WRITE_FILE*.sql").Length > 0);
        }

        [Fact]
        public void AppendDashes()
        {
            output.WriteLine("Log test");
            string fileName = "Nome_do_Arquivo";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("--" + DateTime.Now.ToString() + "_" + fileName);
            sb.Append("--");
            sb.AppendLine("TESTE append");
            log.WriteToFile(sb.ToString(), fileName);
        }

        [Fact]
        public void FluentAssertionsTest()
        {
            string actual = "ABCDEFGHI";
            actual.Should().StartWith("A");
        }


        private static readonly Random random = new Random();

        private static double RandomhberBetween(double minValue, double maxValue)
        {
            var next = random.NextDouble();
            return minValue + (next * (maxValue - minValue));
        }

        [Fact]
        public void RandomDecimalString()
        {
            CultureInfo cultureInfo = new CultureInfo("en-US");

            string value = RandomhberBetween(4.13, 15.99).ToString("#.##", cultureInfo);
            output.WriteLine("Decimal value: " + value);
            Assert.IsType<string>(value);
        }

        public void Dispose()
        {
            folder = null;
            log = null;
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Integration_Test_ClientService.Helpers
{
    public class PriorityOrderer : ITestCaseOrderer
    {
        public IEherable<TTestCase> OrderTestCases<TTestCase>(IEherable<TTestCase> testCases) where TTestCase : ITestCase
        {
            var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

            foreach (TTestCase testCase in testCases)
            {
                int priority = 0;

                foreach (IAttributeInfo attr in testCase.TestMethod.Method.GetCustomAttributes((typeof(TestPriorityAttribute).AssemblyQualifiedName)))
                    priority = attr.GetNamedArgument<int>("Priority");

                GetOrCreate(sortedMethods, priority).Add(testCase);
            }

            foreach (var list in sortedMethods.Keys.Select(priority => sortedMethods[priority]))
            {
                list.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
                foreach (TTestCase testCase in list)
                    yield return testCase;
            }
        }

        static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dictionary, TKey key) where TValue : new()
        {
            TValue result;

            if (dictionary.TryGetValue(key, out result)) return result;

            result = new TValue();
            dictionary[key] = result;

            return result;
        }
    }
}
﻿
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Integration_Test_ClientService.Helpers;
using ClientService;
using ClientService.Common.Helpers;
using ClientService.Common.Ehs;
using Xunit.Abstractions;
using System.Diagnostics;
using System.ComponentModel;

namespace Integration_Test_ClientService.Fixtures
{
    public class Fixture : IDisposable
    {
        StringBuilder sb;
        Logger log;

        private static string ocorrHistorico;

        public string GetOcorrHistorico()
        {
            return ocorrHistorico;
        }

        XYZ2_IncluirOcorrenciaEntrada dadosEntrada;
        XYZ2_IncluirOcorrenciaRetorno retornoLancamento;

        public Fixture()
        {
            sb = new StringBuilder();
            log = new Logger();
            dadosEntrada = new XYZ2_IncluirOcorrenciaEntrada();
            retornoLancamento = new XYZ2_IncluirOcorrenciaRetorno();
        }

        public RetornoService Executa(XYZ2_IncluirOcorrenciaEntrada dadosEntrada,
            string methodName,
            StatusRetornoService tipoRetornovisto = StatusRetornoService.RETORNO_COM_DADOS)
        {
            this.dadosEntrada = dadosEntrada;

            string logFileName = string.Format("{0}_{1}_{2}", methodName, dadosEntrada.parametro, dadosEntrada.operacao);

            //Retorna: “1” (sucesso) ou “0” (falha)
            retornoLancamento = ClientService.ApiConfig
                .Api.incluirOcorrencia(new incluirOcorrencia { dadosEntrada = dadosEntrada })
                .retorno;

            RegistraLog(retornoLancamento, this.dadosEntrada, logFileName, tipoRetornovisto);

            return Retornode(retornoLancamento);
        }


        private void RegistraLog(XYZ2_IncluirOcorrenciaRetorno retornoLancamento,
                                    XYZ2_IncluirOcorrenciaEntrada dadosEntrada,
                                    string logFileName, StatusRetornoService tipoRetornovisto = StatusRetornoService.RETORNO_COM_DADOS)
        {
            if (retornoLancamento.codret.Trim() == ((int)tipoRetornovisto).ToString())
            {
                sb.AppendLine();
                sb.AppendLine("--" + DateTime.Now.ToString() + "_" + logFileName);
                sb.AppendLine("--Entrada: " + Logger.GetInlineParamData(dadosEntrada));
                sb.AppendLine("--Retorno: " + Logger.GetInlineParamData(retornoLancamento));


                sb.AppendFormat("INSERT INTO [dbo].[x] VALUES ('{0}',{1}, {2}, 0, {3}, {4}, '')",
                    this.dadosEntrada.h, this.dadosEntrada.param, retornoLancamento.h, this.dadosEntrada.oper, retornoLancamento.cod);

                sb.AppendLine();
                //await log.WriteTextAsync(sb.ToString(), logFileName);
                log.WriteToFile(sb.ToString(), logFileName);
                ocorrHistorico = retornoLancamento.hop;
            }
            else
            {
                sb.AppendLine();
                sb.AppendLine("--" + DateTime.Now.ToString() + "_ERRO_" + logFileName);
                sb.AppendLine("--Entrada: " + Logger.GetInlineParamData(dadosEntrada));
                sb.AppendLine("--Retorno mensagem: " + retornoLancamento.msgret);
                sb.AppendLine();
                log.WriteToFile(sb.ToString(), "########_ERRO_" + logFileName);
                //await log.WriteTextAsync(sb.ToString(), "########_ERRO_" + logFileName);
            }

            sb.Clear();
        }

        private RetornoService Retornode(XYZ2_IncluirOcorrenciaRetorno Retorno)
        {
            var retorno = new RetornoService();
            retorno.cod = Retorno.cod;
            retorno.codret = Retorno.codret;
            retorno.msgret = Retorno.msgret;
            retorno.hop = Retorno.hop;

            return retorno;
        }

        public void Dispose()
        {
            sb = null;
            dadosEntrada = null;
            log = null;
            retornoLancamento = null;
        }

    }
}
﻿
namespace ClientService.Common.Ehs
{
    public eh StatusRetornoService : int
    {
        RETORNO_SEM_DADOS = 0,
        RETORNO_COM_DADOS = 1,
        RETORNO_ERRO = 2
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace UnitTest_ClientService.LogTests
{
    public abstract class TestLoggerEvents
    {
        /// <summary>
        /// Raised when a test message is received.
        /// </summary>
        
        public abstract event EventHandler<TestRunMessageEventArgs> TestRunMessage;

        /// <summary>
        /// Raised when a test result is received.
        /// </summary>
        /// 
        public abstract event EventHandler<TestResultEventArgs> TestResult;

        /// <summary>
        /// Raised when a test run is complete.
        /// </summary>

        public abstract event EventHandler<TestRunCompleteEventArgs> TestRunComplete;
        
    }
}
﻿using System;

namespace Integration_Test_ClientService.Helpers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestPriorityAttribute : Attribute
    {
        public TestPriorityAttribute(int priority)
        {
            Priority = priority;
        }

        public int Priority { get; private set; }
    }
}
﻿using System;
using Xunit;
using Xunit.Abstractions;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using ClientService;
using System.Web.Services.Protocols;
using System.Diagnostics;

namespace Test
{
    public class UT_X001
    {
        private readonly ITestOutputHelper output;

        public UT_X001(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData("Z1420000006")]
        [InlineData("Z9120000002")]
        [InlineData("Z9120000001")]
        [InlineData("Z1420000006")]
        [InlineData("Z1420000007")]
        [InlineData("Z3120000011")]
        [InlineData("Z3120000012")]
        [Trait("Initial", "Is ?")]
        public void _pertence_(string )
        {
            bool isde = false;

            var dadosEntrada = new XYZ2_IdentificarEntrada();

            dadosEntrada.id = new SqlString().Value;
            try
            {
                isde = ClientService.ApiConfig.Api.identificar(new identificar { dadosEntrada = dadosEntrada }).retorno.ind;                  
            }
            catch (Exception ex)
            {
                output.WriteLine(ex.ToString());                
            }

            output.WriteLine(" de: " +  + " Válido: " + isde.ToString());  
            Assert.True(isde);
        }

        [Fact]
        public void _NAO_pertence_()
        {
            bool isde = true;

            var dadosEntrada = new XYZ2_IdentificarEntrada();
            dadosEntrada.id = new SqlString("654654654654").Value;

            try
            {
                isde = ClientService.ApiConfig.Api.identificar(new identificar { dadosEntrada = dadosEntrada }).retorno.ind;
            }
            catch (Exception ex)
            {
                output.WriteLine(ex.ToString());
            }            

            Assert.False(isde);
        }


        [Fact]
        private void Conexao_de_com_Sucesso()
        {
            bool isde = false;
            var dadosEntrada = new XYZ2_IdentificarEntrada();
            dadosEntrada.id = new SqlString("Z9120000022").Value;
            try
            {
                isde = ClientService.ApiConfig.Api.identificar(new identificar { dadosEntrada = dadosEntrada }).retorno.ind;
            }
            catch (Exception ex)
            {
                output.WriteLine(ex.ToString());
                Debug.WriteLine(ex.ToString());
            }

            var argException = Record.Exception(() => ClientService.ApiConfig.Api.identificar(new identificar { dadosEntrada = dadosEntrada }).retorno.ind);

            Assert.Null(argException);
            Assert.IsNotType(typeof(SoapException), argException);
            
        }
    }
}
