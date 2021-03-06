﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenShare.Net.Library.Common;
using OpenShare.Net.UnitTest.UnitTests.Models;

namespace OpenShare.Net.UnitTest.UnitTests
{
    [TestClass]
    public class OpenShareNetLibraryCommonUnitTests : BaseUnitTest
    {
        [TestMethod]
        public void SecureStringExtensions_Tests()
        {
            var value = new[] { 'c', 'h', 'a', 'r', 's' };
            var bytes = Encoding.UTF8.GetBytes(value);
            var valueString = Encoding.UTF8.GetString(bytes);

            var secureStringValue = value.ToSecureString();
            Assert.IsNotNull(secureStringValue);

            var insecureStringValue = secureStringValue.ToUnsecureString();
            Assert.IsNotNull(insecureStringValue);
            Assert.IsTrue(insecureStringValue.Length == value.Length);
            Assert.AreEqual(valueString, insecureStringValue);

            var secureStringValueString = valueString.ToSecureString();
            Assert.IsNotNull(secureStringValueString);

            var insecureStringValueString = secureStringValueString.ToUnsecureString();
            Assert.IsNotNull(insecureStringValueString);
            Assert.IsTrue(insecureStringValueString.Length == value.Length);
            Assert.AreEqual(valueString, insecureStringValueString);

            Assert.AreEqual(insecureStringValue, insecureStringValueString);

            var secureStringAppSetting = ConfigurationHelper.GetSecureStringFromAppSettings("UnitTestSecureString");
            Assert.IsNotNull(secureStringAppSetting);

            var insecureStringAppSetting = secureStringValue.ToUnsecureString();
            Assert.IsNotNull(insecureStringAppSetting);
            Assert.IsTrue(insecureStringAppSetting.Length == value.Length);
            Assert.AreEqual(valueString, insecureStringAppSetting);
        }

        [TestMethod]
        public void Impersonation_Tests()
        {
            using (var impersonation = new Impersonation(Domain, Username, Password))
            {
                Assert.IsNotNull(impersonation);
            }
        }

        [TestMethod]
        public void FileExtensions_Tests()
        {
            const string testFileData = "A whole new testing world.";
            using (new Impersonation(Domain, Username, Password))
            {
                // To complex to go build path up to directory, keep this as is.
                Assert.IsTrue(Directory.Exists(WebsiteSharePath));
            }

            var originalDirectory = WebsiteSharePath + "TestFolder1";
            using (new Impersonation(Domain, Username, Password))
            {
                FileExtensions.CreateDirectories(originalDirectory, FolderList, Domain, Username);
                FileExtensions.CreateFile(originalDirectory + @"\File Sharing\data.txt", Domain, Username, testFileData);
                Assert.IsTrue(File.Exists(originalDirectory + @"\File Sharing\data.txt"));

                FileExtensions.AppendFile(originalDirectory + @"\File Sharing\data.txt", Domain, Username, testFileData);

                FileExtensions.CreateFile(originalDirectory + @"\File Sharing\ExamplePdf.pdf", Domain, Username, testFileData);
                FileExtensions.CreateFile(originalDirectory + @"\File Sharing\ExampleXls.xls", Domain, Username, testFileData);
                FileExtensions.CreateFile(originalDirectory + @"\File Sharing\ExampleXlsx.xlsx", Domain, Username, testFileData);
                FileExtensions.CreateFile(originalDirectory + @"\File Sharing\ExampleXlsb.xlsb", Domain, Username, testFileData);
                FileExtensions.CreateFile(originalDirectory + @"\File Sharing\ExampleXlsm.xlsm", Domain, Username, testFileData);
                FileExtensions.CreateFile(originalDirectory + @"\File Sharing\ExampleCsv.csv", Domain, Username, testFileData);
                FileExtensions.CreateFile(originalDirectory + @"\File Sharing\ExampleCsv.csvX", Domain, Username, testFileData);
                var linqFiles =
                    Directory.GetFiles(originalDirectory + @"\File Sharing\")
                        .Where(s => s.EndsWith(".pdf") || s.EndsWith(".xls") || s.EndsWith(".xlsx")
                                    || s.EndsWith(".xlsb") || s.EndsWith(".xlsm") || s.EndsWith(".csv")).ToList();
                Assert.AreEqual(linqFiles.Count, 6);
            }

            var newDirectory = WebsiteSharePath + "TestFolder2";
            using (new Impersonation(Domain, Username, Password))
            {
                FileExtensions.RemapDirectory(originalDirectory, newDirectory, Domain, Username);
                FileExtensions.CreateDirectories(newDirectory, FolderList, Domain, Username);

                Assert.IsFalse(Directory.Exists(originalDirectory));
                Assert.IsTrue(File.Exists(newDirectory + @"\File Sharing\data.txt"));
            }

            using (new Impersonation(Domain, Username, Password))
            {
                FileExtensions.DeleteDirectories(newDirectory, Domain, Username);
                Assert.IsFalse(Directory.Exists(newDirectory));
            }
        }

        [TestMethod]
        public void FileExtensions_AesTests()
        {
            const string testFileData = "A whole new testing world.";
            var folderList = new List<string> { "Aes" };
            var baseFolderPath = string.Format(@"{0}{1}", WebsiteSharePath, @"Aes");
            var baseFilePath = string.Format(@"{0}\{1}", baseFolderPath, "AesFile_");
            using (new Impersonation(Domain, Username, Password))
            {
                Assert.IsTrue(Directory.Exists(WebsiteSharePath));
                FileExtensions.CreateDirectories(WebsiteSharePath, folderList, Domain, Username);

                // CreateFile | ReadFile
                var createFilePath = string.Format("{0}{1}", baseFilePath, "CreateFile.txt");
                FileExtensions.CreateFile(createFilePath, Domain, Username, testFileData);
                var createFileData = FileExtensions.ReadFile(createFilePath, Domain, Username);
                Assert.AreEqual(testFileData, createFileData);

                // CreateFileAsync | ReadFileAsync
                var createFileAsyncPath = string.Format("{0}{1}", baseFilePath, "CreateFileAsync.txt");
                FileExtensions.CreateFileAsync(createFileAsyncPath, Domain, Username, testFileData).Wait();
                var createFileDataAsync = FileExtensions.ReadFileAsync(createFileAsyncPath, Domain, Username).Result;
                Assert.AreEqual(testFileData, createFileDataAsync);

                // CreateAesEncryptedFile | ReadAesEncryptedFile
                var createAesEncryptedFilePath = string.Format("{0}{1}", baseFilePath, "CreateAesEncryptedFile.txt");
                FileExtensions.CreateAesEncrytpedFile(createAesEncryptedFilePath, Domain, Username, testFileData,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
                var createAesEncryptedFileData = FileExtensions.ReadAesEncryptedFile(createAesEncryptedFilePath, Domain, Username,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
                Assert.AreEqual(testFileData, createAesEncryptedFileData);

                // CreateAesEncryptedFileAsync | ReadAesEncryptedFileAsync
                var createAesEncryptedFileAsyncPath = string.Format("{0}{1}", baseFilePath, "CreateAesEncryptedFileAsync.txt");
                FileExtensions.CreateAesEncrytpedFileAsync(createAesEncryptedFileAsyncPath, Domain, Username, testFileData,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize).Wait();
                var createAesEncryptedFileDataAsync = FileExtensions.ReadAesEncryptedFileAsync(createAesEncryptedFileAsyncPath, Domain, Username,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize).Result;
                Assert.AreEqual(testFileData, createAesEncryptedFileDataAsync);

                var appendTestFileData = testFileData + testFileData;

                // AppendFile | ReadFile
                var appendFilePath = string.Format("{0}{1}", baseFilePath, "AppendFile.txt");
                FileExtensions.CreateFile(appendFilePath, Domain, Username, testFileData);
                FileExtensions.AppendFile(appendFilePath, Domain, Username, testFileData);
                var appendFileData = FileExtensions.ReadFile(appendFilePath, Domain, Username);
                Assert.AreEqual(appendTestFileData, appendFileData);

                // AppendFileAsync | ReadFileAsync
                var appendFileAsyncPath = string.Format("{0}{1}", baseFilePath, "AppendFileAsync.txt");
                FileExtensions.CreateFileAsync(appendFileAsyncPath, Domain, Username, testFileData).Wait();
                FileExtensions.AppendFileAsync(appendFileAsyncPath, Domain, Username, testFileData).Wait();
                var appendFileDataAsync = FileExtensions.ReadFileAsync(appendFileAsyncPath, Domain, Username).Result;
                Assert.AreEqual(appendTestFileData, appendFileDataAsync);

                // AppendAesEncryptedFile | ReadAesEncryptedFile
                var appendAesEncryptedFilePath = string.Format("{0}{1}", baseFilePath, "AppendAesEncryptedFile.txt");
                FileExtensions.CreateAesEncrytpedFile(appendAesEncryptedFilePath, Domain, Username, testFileData,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
                FileExtensions.AppendAesEncrytpedFile(appendAesEncryptedFilePath, Domain, Username, testFileData,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
                var appendAesEncryptedFileData = FileExtensions.ReadAesEncryptedFile(appendAesEncryptedFilePath, Domain, Username,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
                Assert.AreEqual(appendTestFileData, appendAesEncryptedFileData);

                // AppendAesEncryptedFileAsync | ReadAesEncryptedFileAsync
                var appendAesEncryptedFileAsyncPath = string.Format("{0}{1}", baseFilePath, "AppendAesEncryptedFileAsync.txt");
                FileExtensions.CreateAesEncrytpedFileAsync(appendAesEncryptedFileAsyncPath, Domain, Username, testFileData,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize).Wait();
                FileExtensions.AppendAesEncrytpedFileAsync(appendAesEncryptedFileAsyncPath, Domain, Username, testFileData,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize).Wait();
                var appendAesEncryptedFileDataAsync = FileExtensions.ReadAesEncryptedFileAsync(appendAesEncryptedFileAsyncPath, Domain, Username,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize).Result;
                Assert.AreEqual(appendTestFileData, appendAesEncryptedFileDataAsync);

                // AppendAesEncryptedFileBytesAsync | ReadAesEncryptedFileBytesAsync
                var testFileDataBytes = Encoding.UTF32.GetBytes(testFileData);
                var appendAesEncryptedFileBytesAsyncPath = string.Format("{0}{1}", baseFilePath, "AppendAesEncryptedFileBytesAsync.txt");
                FileExtensions.CreateAesEncrytpedFileBytesAsync(appendAesEncryptedFileBytesAsyncPath, Domain, Username, testFileDataBytes,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize).Wait();
                FileExtensions.AppendAesEncrytpedFileBytesAsync(appendAesEncryptedFileBytesAsyncPath, Domain, Username, testFileDataBytes,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize).Wait();
                var appendAesEncryptedFileDataBytesAsync = FileExtensions.ReadAesEncryptedFileBytesAsync(appendAesEncryptedFileBytesAsyncPath, Domain, Username,
                    AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize).Result;
                var appendAesEncryptedFileDataDecodedAsync = Encoding.UTF32.GetString(appendAesEncryptedFileDataBytesAsync);
                Assert.AreEqual(appendTestFileData, appendAesEncryptedFileDataDecodedAsync);

                FileExtensions.DeleteDirectories(baseFolderPath, Domain, Username);
                Assert.IsFalse(Directory.Exists(baseFolderPath));
            }
        }

        [TestMethod]
        public void GuidGenerator_Tests()
        {
            var guid1 = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            var guid2 = Guid.NewGuid().ToString().Replace("-", "").ToUpper();
            Console.WriteLine(guid1);
            Console.WriteLine(guid2);
            Assert.AreNotEqual(guid1, guid2);
        }

        [TestMethod]
        public void GeneralGuidGenerator_Tests()
        {
            var guid1 = Guid.NewGuid().ToString();
            var guid2 = Guid.NewGuid().ToString();
            Console.WriteLine(guid1);
            Console.WriteLine(guid2);
            Assert.AreNotEqual(guid1, guid2);
        }

        [TestMethod]
        public void AesEncryption_Tests()
        {
            const string data = "Hello Test World !@#$%!@%^!&(#)#)@!JJ!N";
            var encryptedData = data.ToAesEncryptedString(
                AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
            var decryptedData = encryptedData.ToAesDecryptedString(
                AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
            Assert.AreEqual(data, decryptedData);

            const string nullString = null;
            var encryptedNullString = nullString.ToAesEncryptedString(
                AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
            var decryptedNullString = encryptedNullString.ToAesDecryptedString(
                AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
            Assert.AreEqual(nullString, decryptedNullString);

            var emptyString = string.Empty;
            var encryptedEmptyString = emptyString.ToAesEncryptedString(
                AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
            var decryptedEmptyString = encryptedEmptyString.ToAesDecryptedString(
                AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
            Assert.AreEqual(emptyString, decryptedEmptyString);

            var dataBytes = Encoding.UTF32.GetBytes(data);
            var encryptedBytes = dataBytes.ToAesEncryptedBytes(
                AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
            var decryptedBytes = encryptedBytes.ToAesDecryptedBytes(
                AesPassword, AesSalt, AesPasswordIterations, AesInitialVector, AesKeySize);
            var decryptedDataFromBytes = Encoding.UTF32.GetString(decryptedBytes);
            Assert.AreEqual(data, decryptedDataFromBytes);

        }

        [TestMethod]
        public void ModelExtensions_TrimStringsTests()
        {
            var model = new MochStringModel
            {
                Value1 = "asdfasdf",
                Value2 = string.Empty,
                Value3 = null
            };
            model.TrimStrings();
            Assert.AreEqual(model.Value1, "asdfasdf");
            Assert.AreEqual(model.Value2, string.Empty);
            Assert.AreEqual(model.Value2, string.Empty);
        }

        [TestMethod]
        public void ModelExtensions_CleanStringsTests()
        {
            var model = new MochStringModel
            {
                Value1 = "asdfasdf",
                Value2 = string.Empty,
                Value3 = null
            };
            model.CleanStrings();
            Assert.AreEqual(model.Value1, "asdfasdf");
            Assert.AreEqual(model.Value2, null);
            Assert.AreEqual(model.Value2, null);
        }

        [TestMethod]
        public void DateExtentions_Tests()
        {
            const string compareValue = "2014/02/28 12:09:01.239";
            var dateTime = new DateTime(2014, 2, 28, 12, 9, 1, 239);
            Assert.AreEqual(compareValue, dateTime.ToLongDateTimeString());
            var startOfDay = dateTime.StartOfDay();
            Assert.AreEqual(dateTime.Date, startOfDay);
            var endOfDay = dateTime.EndOfDay();
            Assert.AreEqual(startOfDay.AddDays(1).AddMilliseconds(-3), endOfDay);
            var dateTest1 = ConfigurationHelper.GetDateFromAppSettings("DateTest1");
            var dateTest2 = ConfigurationHelper.GetDateFromAppSettings("DateTest2");
            Assert.AreEqual(dateTest1, new DateTime(2014, 12, 31));
            Assert.AreEqual(dateTest2, new DateTime(1, 1, 1));
            Assert.AreEqual(DateTime.MaxValue.Date.StartOfDay(), new DateTime(9999, 12, 31));
            Assert.AreEqual(DateTime.MaxValue.Date.EndOfDay(0), DateTime.MaxValue);

            const string estTimeZoneId = "Eastern Standard Time";
            var utcNow = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var estNow = new DateTime(2016, 12, 31, 19, 0, 0, DateTimeKind.Unspecified);

            Assert.AreEqual(estNow.StartOfDay(), utcNow.StartOfDayFromUtc(estTimeZoneId));
            Assert.AreEqual(estNow.StartOfDay().ToUniversalTime(), utcNow.StartOfDayFromUtc(estTimeZoneId).ToUniversalTime());

            Assert.AreEqual(estNow.EndOfDay(0), utcNow.EndOfDayFromUtc(estTimeZoneId, 0));
            Assert.AreEqual(estNow.EndOfDay(1), utcNow.EndOfDayFromUtc(estTimeZoneId, 1));
            Assert.AreEqual(estNow.EndOfDay(9), utcNow.EndOfDayFromUtc(estTimeZoneId, 9));

            Assert.AreEqual(estNow.EndOfDay(0).ToUniversalTime(), utcNow.EndOfDayFromUtc(estTimeZoneId, 0).ToUniversalTime());
            Assert.AreEqual(estNow.EndOfDay(1).ToUniversalTime(), utcNow.EndOfDayFromUtc(estTimeZoneId, 1).ToUniversalTime());
            Assert.AreEqual(estNow.EndOfDay(9).ToUniversalTime(), utcNow.EndOfDayFromUtc(estTimeZoneId, 9).ToUniversalTime());
        }

        [TestMethod]
        public void DateTimeHelper_Tests()
        {
            const int month = 8;
            const int day = 1;
            const int year = 2017;

            var date = DateTimeHelper.GetDateTimeFromShortDateString($"{month}/{day}/{year}");
            Assert.AreEqual(date.Month, month);
            Assert.AreEqual(date.Day, day);
            Assert.AreEqual(date.Year, year);

            date = DateTimeHelper.GetDateTimeFromShortDateString($"{month:d2}/{day:d2}/{year:d4}");
            Assert.AreEqual(date.Month, month);
            Assert.AreEqual(date.Day, day);
            Assert.AreEqual(date.Year, year);

            Assert.AreEqual(DateTimeHelper.GetFourDigitYear((int?)null), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(-1), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(DateTime.MaxValue.Year + 1), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(0), 2000);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(10), 2010);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(29), 2029);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(30), 1930);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(99), 1999);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(999), 999);

            Assert.AreEqual(DateTimeHelper.GetFourDigitYear((string)null), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(string.Empty), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(" "), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear("-1"), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear(Convert.ToString(DateTime.MaxValue.Year + 1)), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear("2f23t"), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear("0"), 2000);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear("10"), 2010);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear("29"), 2029);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear("30"), 1930);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear("99"), 1999);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYear("999"), 999);

            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString((int?)null), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(-1), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(DateTime.MaxValue.Year + 1), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(0), "2000");
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(10), "2010");
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(29), "2029");
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(30), "1930");
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(99), "1999");
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(999), "0999");

            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString((string)null), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(string.Empty), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(" "), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString("-1"), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString(Convert.ToString(DateTime.MaxValue.Year + 1)), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString("2f23t"), null);
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString("0"), "2000");
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString("10"), "2010");
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString("29"), "2029");
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString("30"), "1930");
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString("99"), "1999");
            Assert.AreEqual(DateTimeHelper.GetFourDigitYearAsString("999"), "0999");

            const string estTimeZoneId = "Eastern Standard Time";
            const string pstTimeZoneId = "Pacific Standard Time";
            var utcTimeZoneId = TimeZoneInfo.Utc.Id;
            var period = TimeSpan.FromDays(1);
            var utcNow = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Full methods
            var runStartMidnightEst = new TimeSpan(0, 0, 0, 0, 0);
            var dueDateMidnightEst = DateTimeHelper.GetTimeTillNextReminder(utcNow, runStartMidnightEst, period, estTimeZoneId);
            Assert.IsTrue(dueDateMidnightEst.TotalHours - 5d < 0.0001d);

            var runStartOneAmEst = new TimeSpan(0, 1, 0, 0, 0);
            var dueDateOneAmEst = DateTimeHelper.GetTimeTillNextReminder(utcNow, runStartOneAmEst, period, estTimeZoneId);
            Assert.IsTrue(dueDateOneAmEst.TotalHours - 6d < 0.0001d);

            var runStartMidnightPst = new TimeSpan(0, 0, 0, 0, 0);
            var dueDateMidnightPst = DateTimeHelper.GetTimeTillNextReminder(utcNow, runStartMidnightPst, period, pstTimeZoneId);
            Assert.IsTrue(dueDateMidnightPst.Subtract(dueDateMidnightEst).TotalHours - 3d < 0.0001d);

            var runStartMidnightUtc = new TimeSpan(0, 0, 0, 0, 0);
            var dueDateMidnightUtc = DateTimeHelper.GetTimeTillNextReminder(utcNow, runStartMidnightUtc, period, utcTimeZoneId);
            Assert.IsTrue(dueDateMidnightUtc.TotalHours < 0.0001d);

            var beforePossiblePreviousRunDateUtcNow = utcNow.AddDays(period.TotalDays * -2);
            dueDateMidnightEst = DateTimeHelper.GetTimeTillNextReminder(beforePossiblePreviousRunDateUtcNow, runStartMidnightEst, period, estTimeZoneId);
            Assert.IsTrue(dueDateMidnightEst.TotalHours - 5d < 0.0001d);

            var runStartSixAmEst = new TimeSpan(0, 6, 0, 0, 0);
            var dueDateSixAmEst = DateTimeHelper.GetTimeTillNextReminder(beforePossiblePreviousRunDateUtcNow, runStartSixAmEst, period, estTimeZoneId);
            Assert.IsTrue(dueDateSixAmEst.TotalHours - 11d < 0.0001d);

            var utcNowSevenAmEst = new DateTime(2017, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            dueDateSixAmEst = DateTimeHelper.GetTimeTillNextReminder(utcNowSevenAmEst, runStartSixAmEst, period, estTimeZoneId);
            Assert.IsTrue(dueDateSixAmEst.TotalHours - 23d < 0.0001d);

            var runStartEightAmEst = new TimeSpan(0, 8, 0, 0, 0);
            var dueDateEightAmEst = DateTimeHelper.GetTimeTillNextReminder(utcNowSevenAmEst, runStartEightAmEst, period, estTimeZoneId);
            Assert.IsTrue(dueDateEightAmEst.TotalHours - 1d < 0.0001d);

            // Short methods (Note: These are commented out as they are breakpoint/time sensitive tests on the Assertions)
            //dueDateMidnightEst = DateTimeHelper.GetTimeTillNextReminder(runStartMidnightEst, period, estTimeZoneId);
            //dueDateMidnightPst = DateTimeHelper.GetTimeTillNextReminder(runStartMidnightPst, period, pstTimeZoneId);
            //Assert.IsTrue(dueDateMidnightPst.Subtract(dueDateMidnightEst).TotalHours - 3d < 0.0001d);

            //var preferredUtcRunDate = DateTimeHelper.GetPreferredUtcRunDate(runStartMidnightUtc);
            //Assert.IsTrue(preferredUtcRunDate.Subtract(DateTime.UtcNow).TotalHours < 0.0001d);
        }

        [TestMethod]
        public void DateTimeParser_Tests()
        {
            DateTime date;
            var dateStr = "12/12/2015 3:34:00 AM EST";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));

            dateStr = "Saturday Aug 6 2016";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));

            dateStr = "12/12/2015 3:34:00+5:00 AM EST";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));

            dateStr = "01/05/2017 06:32 PM CT";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));

            dateStr = "01 / 05/2017 06:32 PM";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));

            dateStr = "12/12/16";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));

            dateStr = "01 / 05/2017 06:32 PM";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));

            dateStr = "01/05/2017 11:18 AM";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));

            dateStr = "December 13th, 2016";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));

            dateStr = "Dec. 13th, 2016.";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));

            dateStr = "2015-12-12T03:34:00.100Z";
            Assert.IsTrue(DateTimeParser.TryParseUtc(dateStr, out date));
        }

        [TestMethod]
        public void TimeSpanExtensions_Tests()
        {
            var timeSpan1 = new TimeSpan(-377, 23, 59, 59, 9994);
            var timeSpan2 = new TimeSpan(377, 23, 59, 59, 9994);
            var timeSpan3 = new TimeSpan(-0, 23, 59, 59, 9997);
            var timeSpan4 = new TimeSpan(0, 23, 59, 59, 9997);
            var timeSpan5 = new TimeSpan(-1, 23, 59, 59, 9997);
            var timeSpan6 = new TimeSpan(1, 23, 59, 59, 9997);
            var timeSpan7 = new TimeSpan(-677, 23, 59, 59, 0);
            var timeSpan8 = new TimeSpan(677, 23, 59, 59, 0);

            Assert.AreEqual(timeSpan1.ToFriendlyString(), "-375 days, 23:59:51.006");
            Assert.AreEqual(timeSpan1.ToString(), "-375.23:59:51.0060000");
            Assert.AreEqual(timeSpan2.ToFriendlyString(), "378 days, 00:00:08.994");
            Assert.AreEqual(timeSpan2.ToString(), "378.00:00:08.9940000");
            Assert.AreEqual(timeSpan3.ToFriendlyString(), "1 day, 00:00:08.997");
            Assert.AreEqual(timeSpan3.ToString(), "1.00:00:08.9970000");
            Assert.AreEqual(timeSpan4.ToFriendlyString(), "1 day, 00:00:08.997");
            Assert.AreEqual(timeSpan4.ToString(), "1.00:00:08.9970000");
            Assert.AreEqual(timeSpan5.ToFriendlyString(), "00:00:08.997");
            Assert.AreEqual(timeSpan5.ToString(), "00:00:08.9970000");
            Assert.AreEqual(timeSpan6.ToFriendlyString(), "2 days, 00:00:08.997");
            Assert.AreEqual(timeSpan6.ToString(), "2.00:00:08.9970000");
            Assert.AreEqual(timeSpan7.ToFriendlyString(), "-676 days, 00:00:01.000");
            Assert.AreEqual(timeSpan7.ToString(), "-676.00:00:01");
            Assert.AreEqual(timeSpan8.ToFriendlyString(), "677 days, 23:59:59.000");
            Assert.AreEqual(timeSpan8.ToString(), "677.23:59:59");
        }
    }
}
