﻿using System;
using System.Threading.Tasks;
using BuildRevisionCounter.Exceptions;
using BuildRevisionCounter.Web.Controllers;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests.Controllers
{
	[TestFixture]
	public class CounterControllerTest
	{
		private CounterController _controller;

		[TestFixtureSetUp]
		public void SetUp()
		{
			_controller = new CounterController(new MongoDB.MongoRevisionRepository(MongoDatabaseFactory.DefaultInstance));
		}
		
		[Test]
		[ExpectedException(typeof(RevisionNotFoundException))]		
		public async Task CurrentThrowsExceptionIfRevisionNotFound()
		{
			var rev = await _controller.Current("CurrentThrowsExceptionIfRevisionNotFound");
		}

		[Test]
		public async Task BumpingNewRevisionReturnsZero()
		{			
			var rev = await _controller.Bumping("BumpingNewRevisionReturnsZero_" + Guid.NewGuid());
			Assert.AreEqual(0, rev);
		}

		[Test]
		public async Task BumpingIncrementsRevisionNumber()
		{
			var revisionName = "BumpingIncrementsRevisionNumber" + Guid.NewGuid();
			var rev1 = await _controller.Bumping(revisionName);
			var rev2 = await _controller.Bumping(revisionName);
			Assert.AreEqual(rev1 + 1, rev2);
		}

		[Test]
		public async Task CurrentReturnsSameValueAsPreviousBumping()
		{
			var rev1 = await _controller.Bumping("CurrentReturnSameValueAsPreviousBumping");
			var rev2 = await _controller.Current("CurrentReturnSameValueAsPreviousBumping");
			Assert.AreEqual(rev1, rev2);
		}
	}
}