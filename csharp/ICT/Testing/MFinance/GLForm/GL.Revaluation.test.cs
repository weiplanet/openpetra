﻿//
// DO NOT REMOVE COPYRIGHT NOTICES OR THIS FILE HEADER.
//
// @Authors:
//       wolfgangu
//
// Copyright 2004-2010 by OM International
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

using System;
using NUnit.Framework;
using Ict.Testing.NUnitPetraServer;
using Ict.Common;
//using Ict.Petra

namespace Tests.MFinance.GLBatches
{
    [TestFixture]
    public class GL_Revaluation_test
    {
        [Test]
        public void TestMethod()
        {
        }

        /// <summary>
        /// ...
        /// </summary>
        [TestFixtureSetUp]
        public void Init()
        {
            new TLogging("TestServer.log");
            TPetraServerConnector.Connect("../../../../../etc/TestServer.config");
        }

        /// <summary>
        /// ...
        /// </summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
            TPetraServerConnector.Disconnect();
        }
    }
}