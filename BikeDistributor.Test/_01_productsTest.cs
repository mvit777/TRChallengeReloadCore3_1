using BikeDistributor.Domain;
using BikeDistributor.Infrastructure.core;
using BikeDistributor.Infrastructure.factories;
using BikeDistributor.Infrastructure.repositories;
using BikeDistributor.Infrastructure.services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MV.Framework;
using MV.Framework.providers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BikeDistributor.Test
{
    public class _01_productsTest
    {
        private TestConfig _config;
        private string _productTestsConfigFile = @".\Fixtures\ProductTests.json";
        //private string _mongoUrl = "mongodb+srv://tr_mongouser:oU2KSIlx3O0EPvaU@cluster0.i90tq.mongodb.net/BikeDb?retryWrites=true&w=majority";
        private string _mongoUrl = "mongodb+srv://tr_mongouser2:jX9lnzMHo80P39fW@cluster0.i90tq.mongodb.net/BikeDb?retryWrites=true&w=majority";
        //private string _mongoUser = "tr_mongouser";
        //private string _mongoPassword = "oU2KSIlx3O0EPvaU";
        //private string _mongoDbConnStr = "mongodb://tr_mongouser:oU2KSIlx3O0EPvaU@cluster0.i90tq.mongodb.net";
        private MongoDBContext _context = null;
        //private string _mongoUrl = "mongodb://tr_mongouser:oU2KSIlx3O0EPvaU@cluster0.i90tq.mongodb.net";
        private string _mongoDbName = "BikeDb";
        private MongoSettings _mongoSettings = null;
        public _01_productsTest()
        {
            BsonSerializer.RegisterIdGenerator(typeof(string), new StringObjectIdGenerator());
            _config = new TestConfig(_productTestsConfigFile);
            _mongoSettings = new MongoSettings();
            _mongoSettings.Connection = _mongoUrl;
            _mongoSettings.DatabaseName = _mongoDbName;
            _context = GetContext();
        }

        private MongoDBContext GetContext()
        {
            
            return new MongoDBContext(_mongoSettings);
        }

        [Fact]
        public void _01_01_GetProduct()
        {
            JObject jBike = _config.GetJObject("bikes", 0);
            var bike =(Bike)BikeFactory.Create(jBike).GetBike();
            bike.isStandard.Should().Be(true);
        }

        [Fact]
        public async Task _01_02_SaveProductMongoAsync()
        {          
            JObject jBike = _config.GetJObject("bikes", 1);
            var bike = BikeFactory.Create(jBike).GetBike();
            bike.isStandard.Should().Be(false);
            var bv = (BikeVariant)bike;
            bv.GetOptions().Count.Should().BeGreaterThan(0);
            bv.GetOption("Material").Description.Should().Be("Carbon Fiber");
            var bikeRepo = new BikeRepository(_context);
            var defySe = new MongoEntityBike(bv);
            await bikeRepo.Create(defySe);
            var bikes = (List<MongoEntityBike>)await bikeRepo.Get();
            bikes.Count.Should().Be(1);
            //var getInserted = await bikeRepo.Get(defySe.Bike.Model); //id is not in correct format
            //var justInsertedBikeVariant = (BikeVariant)getInserted.Bike;
            //justInsertedBikeVariant.GetOptions().Count.Should().BeGreaterThan(0);

        }

        [Fact]
        public async Task _01_03_UsingServiceSaveAsync()
        {
            JObject jBike = _config.GetJObject("bikes", 0);
            var bike = BikeFactory.Create(jBike).GetBike();
            var bikeService =(MongoBikeService) ServiceUtils.GetBikeMongoService(_mongoUrl, _mongoDbName);
            await bikeService.AddBikeAsync(bike);
            List<MongoEntityBike> bikes = await bikeService.Get();
            bikes.Count.Should().Be(2);
        }

    }
}
