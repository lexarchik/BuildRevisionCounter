﻿using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace BuildRevisionCounter.Tests
{
	[TestFixture]
	public class BuildRevisionCounterTest : IntegrationTest
	{
		[Test]
		public async Task GetAllRevisions()
		{
			var apiUri = "api/counter";

			var response = await SendGetRequest(apiUri);
			var body = await response.Content.ReadAsStringAsync();
			dynamic result = JArray.Parse(body);
			int totalCounters = result.Count;

			Assert.GreaterOrEqual(totalCounters, 0);

			await SendPostRequest(apiUri + "/" + Guid.NewGuid());
			await SendPostRequest(apiUri + "/" + Guid.NewGuid());
			await SendPostRequest(apiUri + "/" + Guid.NewGuid());

			response = await SendGetRequest(apiUri);
			body = await response.Content.ReadAsStringAsync();
			result = JArray.Parse(body);

			Assert.AreEqual(result.Count, totalCounters + 3, "Revisions weren't added.");
			Assert.AreEqual(result.Count - 3, totalCounters);
		}

		[Test]
		public async Task BumpNewRevision()
		{
			var revName = "revision_" + Guid.NewGuid();
			var apiUri = "api/counter/" + revName;

			var response = await SendPostRequest(apiUri);
			var body = await response.Content.ReadAsStringAsync();

			Assert.AreEqual("0", body);
		}

		[Test]
		public async Task BumpExistingRevision()
		{
			var revName = "revision_" + Guid.NewGuid();
			var apiUri = "api/counter/" + revName;

			await SendPostRequest(apiUri); //counter == 0

			var response = await SendPostRequest(apiUri); //counter++
			var body = await response.Content.ReadAsStringAsync();

			Assert.AreEqual("1", body);
		}

		[Test]
		public async Task GetCurrentRevision()
		{
			var revName = "revision_" + Guid.NewGuid();
			var apiUri = "api/counter/" + revName;

			await SendPostRequest(apiUri); //counter == 0
			await SendPostRequest(apiUri); // counter++

			var response = await SendGetRequest(apiUri);
			var body = await response.Content.ReadAsStringAsync();

			Assert.AreEqual("1", body);
		}

		[Test]
		public async Task GetCurrentRevisionNotFound()
		{
			var body = await SendGetRequest("api/counter/nonexistingrevision");
			Assert.AreEqual(body.StatusCode, HttpStatusCode.NotFound);
		}
	}
}