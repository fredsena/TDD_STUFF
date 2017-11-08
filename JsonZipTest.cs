using System;
using FluentAssertions;
using System.Collections.Generic;
using Xunit;
using ExportJson.ExportClasses;
using Newtonsoft.Json;

//https://dotnetzip.codeplex.com/
using Ionic.Zlib;

using Ionic.Zip;
using System.IO;

/*
References for further projects :-)

using NSubstitute;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;
using Ploeh.AutoFixture.Xunit2;
*/

namespace ExportJson.Tests
{
    //var RegisteredUsers = new List<Person>();
    //RegisteredUsers.Add(new Person() { PersonID = 1, Name = "Bryon Hetrick", Registered = true });
    //RegisteredUsers.Add(new Person() { PersonID = 2, Name = "Nicole Wilcox", Registered = true });
    //RegisteredUsers.Add(new Person() { PersonID = 3, Name = "Adrian Martinson", Registered = false });
    //RegisteredUsers.Add(new Person() { PersonID = 4, Name = "Nora Osborn", Registered = false });    

    public class ExampleAutoDataAttribute : AutoDataAttribute
    {
        public ExampleAutoDataAttribute() : base(new Fixture().Customize(new AutoNSubstituteCustomization()))
        {
            Fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => Fixture.Behaviors.Remove(b));
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }

    public class UnitTest
    {
        string path = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName, "ZIP");

        string fileName = "JsonZipFile.zip";

        [Fact]
        public void Get_Json_From_Contact()
        {
            IEnumerable<Contact> Contacts;

            Contacts = new List<Contact>
            {
                new Contact(7113, "Ted Norris", new DateTime(1977, 5, 13), "488-555-1212"), 
                new Contact(7114, "Mary Lamb", new DateTime(1974, 10, 21), "337-555-1212"), 
                new Contact(7115, "James Shoemaker", new DateTime(1968, 2, 8), "643-555-1212"), 
            };
            var result = JsonConvert.SerializeObject(Contacts);

            result.Should().NotBeNull();
        }

        [Fact]
        public void Get_Json_from_GrossIncident()
        {
            GetJsonIncidents().Should().NotBeNull();
        }


        [Fact]
        public void Convert_String_To_ZipStream()
        {
            System.IO.MemoryStream msSinkCompressed;
            System.IO.MemoryStream msSinkDecompressed;
            ZlibStream zOut;
            String originalText = "Hello, World!  This String will be compressed... ";

            // first, compress:
            msSinkCompressed = new System.IO.MemoryStream();
            zOut = new ZlibStream(msSinkCompressed, CompressionMode.Compress, CompressionLevel.BestCompression, true);
            CopyStream(StringToMemoryStream(originalText), zOut);
            zOut.Close();

            // at this point, msSinkCompressed contains the compressed bytes

            // now, decompress:
            msSinkCompressed.Seek(0, System.IO.SeekOrigin.Begin);
            msSinkDecompressed = new System.IO.MemoryStream();
            zOut = new ZlibStream(msSinkDecompressed, CompressionMode.Decompress, true);
            CopyStream(msSinkCompressed, zOut);

            // at this point, msSinkDecompressed contains the decompressed bytes
            string decompressed = MemoryStreamToString(msSinkDecompressed);

            Assert.Equal(originalText, decompressed);
        }


        [Fact]
        public void Json_To_Zip_Byte()
        {
            byte[] bZipJson;

            using (var mStream = new System.IO.MemoryStream())
            {
                using (ZipFile zip = new ZipFile(fileName))
                {
                    zip.AddEntry("GrossIncidents.json", GetJsonIncidents());
                    zip.Save(mStream);                    
                }

                bZipJson = mStream.ToArray();                                
            }

            File.WriteAllBytes(Path.Combine(path,fileName), bZipJson);

            bZipJson.Should().NotBeNull();
        }

        public string GetJsonIncidents()
        {
            List<GrossIncident> incidents;
            incidents = new List<GrossIncident>
            {
                new GrossIncident 
                { 
                    EventType = "1", JuridicID = "52556", RootNumber="", RootDate="", Category = "3", LossReason="2",
	                InternalID = "52556", Description = "Ação trabalhista", Value = 2.500M, Responsible = "teste01",
	                IncidentDate = "01/03/2015", ConfirmDate = "20/03/2016", JudicialSource = 1, PslCount = 2,
	                LossStatus = 1, Action = "", SourceUnit = "", FetchedUnit = "", Process = "", Approver = "teste02",
	                BusinessFunction = 1 
                },

                new GrossIncident 
                { 
                    EventType = "2", JuridicID = "42556", RootNumber="", RootDate="", Category = "5", LossReason="7",
	                InternalID = "82556", Description = "Ação Jurídica", Value = 5.500M, Responsible = "teste02",
	                IncidentDate = "01/03/2016", ConfirmDate = "20/03/2017", JudicialSource = 9, PslCount = 4,
	                LossStatus = 3, Action = "", SourceUnit = "", FetchedUnit = "", Process = "", Approver = "teste05",
	                BusinessFunction = 2 
                }
            };


            var response = new GrossIncidentResponse();

            response.UploadUnit = "BRANCH";
            response.Fip = "12345";
            response.GrossIncidents = incidents;

            return JsonConvert.SerializeObject(response);
        }


        static String MemoryStreamToString(System.IO.MemoryStream ms)
        {
            byte[] ByteArray = ms.ToArray();
            return System.Text.Encoding.ASCII.GetString(ByteArray);
        }


        static void CopyStream(System.IO.Stream src, System.IO.Stream dest)
        {
            byte[] buffer = new byte[1024];
            int len = src.Read(buffer, 0, buffer.Length);
            while (len > 0)
            {
                dest.Write(buffer, 0, len);
                len = src.Read(buffer, 0, buffer.Length);
            }
            dest.Flush();
        }

        static System.IO.MemoryStream StringToMemoryStream(string s)
        {
            byte[] a = System.Text.Encoding.ASCII.GetBytes(s);
            return new System.IO.MemoryStream(a);
        }
    }
}
