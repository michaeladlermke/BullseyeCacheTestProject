using System;
using Xunit;
using Moq;
using System.Collections.Generic;
using BullseyeCacheLibrary;
using Microsoft.Extensions.Caching.Memory;

namespace BullseyeCacheTestProject
{
    public class BullseyeCacheUnitTests
    {
        BullseyeDevice dev01 = new BullseyeDevice("device 01", "{ 01 some device info; device info here; }");
        BullseyeDevice dev02 = new BullseyeDevice("device 02", "{ 02 some device info; device info here; }");
        BullseyeDevice dev03 = new BullseyeDevice("device 03", "{ 03 some device info; device info here; }");
        BullseyeDevice dev04 = new BullseyeDevice("device 04", "{ 04 some device info; device info here; }");
        BullseyeDevice dev05 = new BullseyeDevice("device 05", "{ 05 some device info; device info here; }");
        BullseyeDevice dev06 = new BullseyeDevice("device 06", "{ 06 some device info; device info here; }");
        BullseyeDevice dev07 = new BullseyeDevice("device 07", "{ 07 some device info; device info here; }");
        BullseyeDevice dev08 = new BullseyeDevice("device 08", "{ 08 some device info; device info here; }");
        BullseyeDeviceHelper helper = new BullseyeDeviceHelper();

        [Theory]
        [InlineData(null)]
        public void BullseyeDeviceEquals_NullDeviceEquals_ThrowsException(BullseyeDevice nullDevice)
        {
            Assert.Throws<ArgumentNullException>(() => dev08.Equals(nullDevice));
        }

        [Fact]
        public void AddMultipleObjects_ListOfThreeObjectsAdded_CacheSizeIsThree()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> list = new List<IBullseyeDevice>(){ dev05, dev01, dev02};
            cache.AddMultipleObjects(list, 3);
            var size = cache.BullseyeCacheCount();
            Assert.Equal(3, size);
        }

        [Fact]
        public void AddMultipleObjects_EmptyListAdded_CacheSizeIsZero()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> list = new List<IBullseyeDevice>();
            cache.AddMultipleObjects(list, 3);
            var size = cache.BullseyeCacheCount();
            Assert.Equal(0, size);
        }
        
        [Theory]
        [InlineData(null)]
        public void AddMultipleObjects_SearchForNullList_ThrowsException(List<IBullseyeDevice> list)
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            Assert.Throws<ArgumentNullException>(() => cache.AddMultipleObjects(list, 3));
        }

        [Fact]
        public void AddMultipleObjects_AddObjectsWithBadTime_ThrowsException()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            var list = new List<IBullseyeDevice>() { dev01, dev02, dev03 };
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.AddMultipleObjects(list, -3));
        }


        [Fact]
        public void AddObject_AddSingleObject_ReturnsOne()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddObject(dev02,3);
            var size = cache.BullseyeCacheCount();
            Assert.Equal(1, size);
        }

        [Fact]
        public void AddObject_AddModifiedObjectAlreadyInCache_ReturnsOne()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddObject(dev02, 3);
            var updatedDevice = new BullseyeDevice(dev02.Id, "EDITED DEVICE");

            cache.AddObject(updatedDevice, 3);

            var size = cache.BullseyeCacheCount();
            Assert.Equal(1, size);
        }

        [Fact]
        public void AddObject_AddSingleObjectWithBadTime_ThrowsException()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.AddObject(dev02, -3));
        }


        [Fact]
        public void BullseyeCache_DefaultConstructor_EmptyCacheCreated()
        {
            var moqCache = new Mock<IMemoryCache>();
            //moqCache.Setup(x => x.CreateEntry()); play with this more
            //todo

            //moqCache.Setup(x => x.CreateEntry()).Returns("this is what I want it to return");

            var cache = new BullseyeCache(moqCache.Object);

            //todo
            // create mocked memory cache object

            var size = cache.BullseyeCacheCount();
            Assert.Equal(0, size);
        }


        [Fact]
        public void BullseyeCacheCount_EmptyCache_ReturnsZero()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            var size = cache.BullseyeCacheCount();

            Assert.Equal(0, size);
        }

        [Fact]
        public void BullseyeCacheCount_SingleDeviceInCache_ReturnsOne()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddObject(dev01,3);
            var size = cache.BullseyeCacheCount();

            Assert.Equal(1, size);
        }
       


        [Fact]
        public void CheckCacheForMultipleObjects_FiveObjectsAddedThreeSearchedFor_AllSearchedForFound()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> insertList = new List<IBullseyeDevice>() { dev05, dev01, dev02, dev03, dev04 };
            List<IBullseyeDevice> searchList = new List<IBullseyeDevice>() { dev06, dev07, dev02, dev03, dev04 };
            cache.AddMultipleObjects(insertList, 3);
            List<IBullseyeDevice> returnList = cache.CheckCacheForMultipleObjects(searchList);
            var size = returnList.Count;

            Assert.Equal(3, size);

        }

        [Theory]
        [InlineData(null)]
        public void CheckCacheForMultipleObjects_NullList_ReturnsException(List<IBullseyeDevice> list)
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleObjects(insertList, 3);
            
            Assert.Throws<ArgumentNullException>(() => cache.CheckCacheForMultipleObjects(list));

        }

        [Fact]
        public void GetObject_CheckObjectInCacheMatchObjectSentIn_ObjectsMatch()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> insertList = new List<IBullseyeDevice>{ dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleObjects(insertList, 3);

            BullseyeDevice myObj = (BullseyeDevice)cache.GetObject(dev01);

            Assert.True(myObj.Equals(dev01));

        }

        [Theory]
        [InlineData(null, null)]
        public void GetObject_SendInNullObject_ThrowsException(String objectString, BullseyeDevice objectDevice)
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddObject(dev02, 5);

            Assert.Throws<ArgumentNullException>(() => cache.GetObject(objectString));
            Assert.Throws<ArgumentNullException>(() => cache.GetObject(objectDevice));
            Assert.Null(cache.GetObject(dev05));
        }

        [Fact]
        public void NewDeviceCallback_StateUnderTest_ExpectedBehavior()
        {
            //todo
            Action myAction = () => { };

            //use this as a fake method and check that it gets called by a setup callback for the device
        }

        [Fact]
        public void RemoveAllObjects_RemoveObjectsFromCache_CacheIsEmpty()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            var insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleObjects(insertList, 3);

            cache.RemoveAllObjects();
            var countAfterRemoval = cache.BullseyeCacheCount();

            Assert.Equal(0, countAfterRemoval);

        }

        [Fact]
        public void RemovedDeviceCallback_StateUnderTest_ExpectedBehavior()
        {
            //todo
        }

        [Fact]
        public void RemoveObject_RemoveSingleObject_RemovesObject()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleObjects(insertList, 3);

            cache.RemoveObject(dev01);
            var countAfterRemoval = cache.BullseyeCacheCount();

            Assert.Equal(4, countAfterRemoval);

        }

        [Theory]
        [InlineData(null)]
        public void RemoveObject_RemoveNullObject_ThrowsException(BullseyeDevice device)
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleObjects(insertList, 3);

            Assert.Throws<ArgumentNullException>(() => cache.RemoveObject(device));

        }

        [Fact]
        public void RemoveObject_RemoveObjectNotInCache_NoObjectRemoved()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleObjects(insertList, 3);

            cache.RemoveObject(dev06);
            var countAfterRemoval = cache.BullseyeCacheCount();

            Assert.Equal(5, countAfterRemoval);

        }


        [Fact]
        public void UpdatedObject_UpdateObjectInCache_PayloadChanged()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddObject(dev01, 3);
            var editedPayload = dev01.Payload + "edit!";
            var dev01copy = new BullseyeDevice(dev01.Id, editedPayload);

            cache.UpdateObject(dev01copy, 3);
            
            Assert.NotEqual(dev01copy.Payload, dev01.Payload);

        }

        [Fact]
        public void UpdatedObject_UpdateObjectInCache_ObjectInCacheIsUpdated()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddObject(dev01, 3);
            var editedPayload = dev01.Payload + "edit!";
            var dev01copy = new BullseyeDevice(dev01.Id, editedPayload);

            cache.UpdateObject(dev01copy, 3);
            var cachedObject = cache.GetObject(dev01);

            Assert.Equal(editedPayload, cachedObject.Payload);

        }

        [Fact]
        public void UpdateObject_UpdateObjectThatDoesNotExist_ObjectIsInserted()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.UpdateObject(dev02, 3);
            var size = cache.BullseyeCacheCount();
            Assert.Equal(1, size);
        }

        [Fact]
        public void UpdateObject_UpdateWithBadTime_ThrowsException()
        {
            var cache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddObject(dev02, 5);
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.UpdateObject(dev02, -3));
        }



        [Fact]
        public void UpdatedDeviceCallback_StateUnderTest_ExpectedBehavior()
        {
            //todo
        }
    }
}
