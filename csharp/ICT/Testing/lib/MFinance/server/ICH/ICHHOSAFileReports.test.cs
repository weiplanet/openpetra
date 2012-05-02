//
// DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
// @Authors:
//       timop, christophert
//
// Copyright 2004-2012 by OM International
//
// This file is part of OpenPetra.org.
//
// OpenPetra.org is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// OpenPetra.org is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with OpenPetra.org.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Data;
using System.Configuration;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.IO;
using System.Collections;
using Ict.Testing.NUnitPetraServer;
using Ict.Testing.NUnitTools;
using Ict.Common;
using Ict.Common.Verification;
using Ict.Common.DB;
using Ict.Common.Remoting.Server;
using Ict.Common.Remoting.Shared;
using Ict.Petra.Server.App.Core;
using Ict.Petra.Server.MFinance.Common;
using Ict.Petra.Server.MFinance.Gift;
using Ict.Petra.Server.MFinance.Gift.WebConnectors;
using Ict.Petra.Server.MFinance.ICH;
using Ict.Petra.Server.MFinance.ICH.WebConnectors;
using Ict.Petra.Shared.MFinance;
using Ict.Petra.Shared.MFinance.Gift.Data;
using Ict.Petra.Shared.MFinance.Account.Data;
using Ict.Petra.Server.MFinance.Account.Data.Access;
using Ict.Common.Data;

namespace Tests.MFinance.Server.ICH
{
    /// This will test the business logic directly on the server
    [TestFixture]
    public class TICHHOSAFileReportsTest
    {
        Int32 FLedgerNumber = -1;

        const string MainFeesPayableCode = "GIF";
        const string MainFeesReceivableCode = "HO_ADMIN";

        /// <summary>
        /// open database connection or prepare other things for this test
        /// </summary>
        [TestFixtureSetUp]
        public void Init()
        {
            //new TLogging("TestServer.log");
            TPetraServerConnector.Connect("../../etc/TestServer.config");
            FLedgerNumber = TAppSettingsManager.GetInt32("LedgerNumber", 43);
        }

        /// <summary>
        /// cleaning up everything that was set up for this test
        /// </summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
            TPetraServerConnector.Disconnect();
        }

        /// <summary>
        /// Test whether the code opens a text file and replaces the first line
        ///  with the specified string
        /// </summary>
        [Test]
        public void TestFileHeaderReplace()
        {
            string fileName = @"C:\Test.csv";
            int PeriodNumber = 4;
            string StandardCostCentre = "4300";
            string CostCentre = "78";
            string Currency = "USD";
            string TableForExportHeader = "/** Header **" + "," +
                                          PeriodNumber.ToString() + "," +
                                          StandardCostCentre + "," +
                                          CostCentre + "," +
                                          DateTime.Today.ToShortDateString() + "," +
                                          Currency;

            TGenHOSAFilesReports.ReplaceHeaderInFile(fileName, TableForExportHeader);
        }

        /// <summary>
        /// Test generation of HOSA files
        /// </summary>
        [Test]
        public void TestGenerateHOSAFiles()
        {
            int LedgerNumber = FLedgerNumber;
            int PeriodNumber = 4;
            int IchNumber = 1;
            string CostCentre = "73";
            int Currency = 0;  //0 = base 1 = intl
            string FileName = Path.GetTempPath() + @"\TestGenHOSAFile.csv";
            TVerificationResultCollection VerificationResults;

            TGenHOSAFilesReports.GenerateHOSAFiles(LedgerNumber, PeriodNumber, IchNumber, CostCentre, Currency, FileName, out VerificationResults);

            Assert.IsFalse(VerificationResults.HasCriticalErrors,
                "Performing HOSA File Generation Failed!" + VerificationResults.BuildVerificationResultString());
        }

        /// <summary>
        /// Test generation of HOSA reports
        /// </summary>
        [Test]
        public void TestGenerateHOSAReports()
        {
            int LedgerNumber = FLedgerNumber;
            int PeriodNumber = 4;
            int IchNumber = 1;
            string Currency = "USD";
            TVerificationResultCollection VerificationResults;

            TGenHOSAFilesReports.GenerateHOSAReports(LedgerNumber, PeriodNumber, IchNumber, Currency, out VerificationResults);

            Assert.IsFalse(VerificationResults.HasCriticalErrors,
                "Performing HOSA Report Generation Failed!" + VerificationResults.BuildVerificationResultString());
        }

        /// <summary>
        /// Test the exporting of gifts as part of the HOSA process
        /// </summary>
        [Test]
        public void TestExportGifts()
        {
            int LedgerNumber = FLedgerNumber;
            string CostCentre = "73";
            string AcctCode = "10001";
            string MonthName = "April";
            int PeriodNumber = 4;
            DateTime PeriodStartDate = Convert.ToDateTime("1-Apr-2012");
            DateTime PeriodEndDate = Convert.ToDateTime("30-Apr-2012");
            string Base = MFinanceConstants.CURRENCY_BASE;
            int IchNumber = 1;
            DataTable TableForExport = new DataTable();

            TVerificationResultCollection VerificationResults = new TVerificationResultCollection();

            bool NewTransaction = false;

            TDBTransaction DBTransaction = DBAccess.GDBAccessObj.GetNewOrExistingTransaction(IsolationLevel.Serializable, out NewTransaction);

            //Create DataTable to receive exported transactions
            TableForExport.Columns.Add("CostCentre", typeof(string));
            TableForExport.Columns.Add("Account", typeof(string));
            TableForExport.Columns.Add("LedgerMonth", typeof(string));
            TableForExport.Columns.Add("ICHPeriod", typeof(string));
            TableForExport.Columns.Add("Date", typeof(DateTime));
            TableForExport.Columns.Add("IndividualDebitTotal", typeof(decimal));
            TableForExport.Columns.Add("IndividualCreditTotal", typeof(decimal));

            TGenHOSAFilesReports.ExportGifts(LedgerNumber,
                CostCentre,
                AcctCode,
                MonthName,
                PeriodNumber,
                PeriodStartDate,
                PeriodEndDate,
                Base,
                IchNumber,
                ref TableForExport,
                ref DBTransaction,
                ref VerificationResults);

            TableForExport.AcceptChanges();

            DataRow[] DR = TableForExport.Select();

            Assert.IsFalse(VerificationResults.HasCriticalErrors,
                "HOSA - Performing Export of gifts Failed!" + VerificationResults.BuildVerificationResultString());

            Assert.IsTrue((DR.Length > 0),
                "HOSA - Performing Export of gifts Failed to return any rows!");

            if (NewTransaction)
            {
                DBAccess.GDBAccessObj.RollbackTransaction();
            }
        }
    }
}