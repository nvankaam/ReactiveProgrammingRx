﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Data.Model;
using System.Net;
using Data.Repo;
using System.Globalization;
using System.Threading;

namespace UnitTests
{
    [TestClass]
    public class TestRepoApacheLogLine
    {
        private ApacheLogLine LogLine {get;set;}
        private string LogLineString { get; set; }

        [TestMethod]
        public void TestParseLine()
        {
            var repo = new RepoApacheLogLine();
            var parsed = repo.ParseLine(LogLineString);
            Assert.AreEqual(LogLine, parsed);
        }

        [TestMethod]
        public void TestGetObservable()
        {
            var repo = new RepoApacheLogLine();
            var observable = repo.GetObservableLogLines(100);

            Thread.Sleep(10000);
        }

        [TestInitialize()]
        public void Init()
        {
            LogLineString = "83.86.171.177 - - [15/May/2013:04:03:01 +0200] \"GET /weer/turfWeer HTTP/1.1\" 200 499 \"-\" \"Java/1.7.0_05\"";
            LogLine = new ApacheLogLine()
            {
                IP = "83.86.171.177",
                Date = DateTime.ParseExact("[15/May/2013:04:03:01]", "[dd/MMM/yyyy:HH:mm:ss]", CultureInfo.InvariantCulture),
                Command = "GET /weer/turfWeer HTTP/1.1",
                Status = HttpStatusCode.OK,
                Time = 499,
                UserAgent = "Java/1.7.0_05"
            };

        }
    }
}
