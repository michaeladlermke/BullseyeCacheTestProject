using System;
using Xunit;
using Moq;
using System.Collections.Generic;
using BullseyeCacheLibrary;
using Microsoft.Extensions.Caching.Memory;

namespace BullseyeCacheTestProject
{
    public class BullseyeMemoryCacheUnitTests
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

        MemoryCache Cache = new MemoryCache(new MemoryCacheOptions
        {
            SizeLimit = 1024
        });

        [Theory]
        [InlineData(null)]
        public void BullseyeDeviceEquals_NullDeviceEquals_ThrowsException(BullseyeDevice nullDevice)
        {
            Assert.Throws<ArgumentNullException>(() => dev08.Equals(nullDevice));
        }

        [Fact]
        public void AddMultipleDevices_ListOfThreeDevicesAdded_CacheSizeIsThree()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> list = new List<IBullseyeDevice>(){ dev05, dev01, dev02};
            cache.AddMultipleDevices(list, 3);
            var size = cache.BullseyeMemoryCacheCount();
            Assert.Equal(3, size);
        }

        [Fact]
        public void AddMultipleDevices_EmptyListAdded_CacheSizeIsZero()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> list = new List<IBullseyeDevice>();
            cache.AddMultipleDevices(list, 3);
            var size = cache.BullseyeMemoryCacheCount();
            Assert.Equal(0, size);
        }
        
        [Theory]
        [InlineData(null)]
        public void AddMultipleDevices_SearchForNullList_ThrowsException(List<IBullseyeDevice> list)
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            Assert.Throws<ArgumentNullException>(() => cache.AddMultipleDevices(list, 3));
        }

        [Fact]
        public void AddMultipleDevices_AddDevicesWithBadTime_ThrowsException()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            var list = new List<IBullseyeDevice>() { dev01, dev02, dev03 };
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.AddMultipleDevices(list, -3));
        }


        [Fact]
        public void AddDevice_AddSingleDevice_ReturnsOne()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddDevice(dev02,3);
            var size = cache.BullseyeMemoryCacheCount();
            Assert.Equal(1, size);
        }

        [Fact]
        public void AddDevice_AddModifiedDeviceAlreadyInCache_ReturnsOne()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddDevice(dev02, 3);
            var updatedDevice = new BullseyeDevice(dev02.Id, "EDITED DEVICE");

            cache.AddDevice(updatedDevice, 3);

            var size = cache.BullseyeMemoryCacheCount();
            Assert.Equal(1, size);
        }

        [Fact]
        public void AddDevice_AddSingleDeviceWithBadTime_ThrowsException()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddDevice(dev02, -3);

            var size = cache.BullseyeMemoryCacheCount();
            Assert.Equal(0, size);
        }


        [Fact]
        public void BullseyeMemoryCache_DefaultConstructor_EmptyCacheCreated()
        {
            var moqCache = new Mock<IMemoryCache>();
            //moqCache.Setup(x => x.CreateEntry()); play with this more
            //todo

            //moqCache.Setup(x => x.CreateEntry()).Returns("this is what I want it to return");

            var cache = new BullseyeMemoryCache(moqCache.Object);

            //todo
            // create mocked memory cache Device

            var size = cache.BullseyeMemoryCacheCount();
            Assert.Equal(0, size);
        }


        [Fact]
        public void BullseyeMemoryCacheCount_EmptyCache_ReturnsZero()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            var size = cache.BullseyeMemoryCacheCount();

            Assert.Equal(0, size);
        }

        [Fact]
        public void BullseyeMemoryCacheCount_SingleDeviceInCache_ReturnsOne()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddDevice(dev01,3);
            var size = cache.BullseyeMemoryCacheCount();

            Assert.Equal(1, size);
        }
       


        [Fact]
        public void CheckCacheForMultipleDevices_FiveDevicesAddedThreeSearchedFor_AllSearchedForFound()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> insertList = new List<IBullseyeDevice>() { dev05, dev01, dev02, dev03, dev04 };
            List<IBullseyeDevice> searchList = new List<IBullseyeDevice>() { dev06, dev07, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);
            List<IBullseyeDevice> returnList = cache.CheckCacheForMultipleDevices(searchList);
            var size = returnList.Count;

            Assert.Equal(3, size);

        }

        [Theory]
        [InlineData(null)]
        public void CheckCacheForMultipleDevices_NullList_ReturnsException(List<IBullseyeDevice> list)
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);
            
            Assert.Throws<ArgumentNullException>(() => cache.CheckCacheForMultipleDevices(list));

        }

        [Fact]
        public void GetDevice_CheckDeviceInCacheMatchDeviceSentIn_DevicesMatch()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> insertList = new List<IBullseyeDevice>{ dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            BullseyeDevice myObj = (BullseyeDevice)cache.GetDevice(dev01);

            Assert.True(myObj.Equals(dev01));

        }

        [Theory]
        [InlineData(null, null)]
        public void GetDevice_SendInNullDevice_ThrowsException(String DeviceString, BullseyeDevice DeviceDevice)
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddDevice(dev02, 5);

            Assert.Throws<ArgumentNullException>(() => cache.GetDevice(DeviceString));
            Assert.Throws<ArgumentNullException>(() => cache.GetDevice(DeviceDevice));
            Assert.Null(cache.GetDevice(dev05));
        }

        [Fact]
        public void NewDeviceCallback_StartUpActionDoesSomething_ActionsArePerformed()
        {
            //todo
            var flag = false;
            var keyword = "Not In";
            var addedDevice = "";
            
            void StartUpAction(IBullseyeDevice device)
            {
                addedDevice = device.Id;
                flag = true;
                keyword = "Now In";
            }

            var cache = new BullseyeMemoryCache(Cache, StartUpAction, helper.UpdateAction, helper.EvictionAction);


            Assert.False(flag);
            Assert.Equal("Not In", keyword);
            cache.AddDevice(dev01, 3);
            Assert.True(flag);
            Assert.Equal("Now In", keyword);
            
        }

        [Fact]
        public void RemoveAllDevices_RemoveDevicesFromCache_CacheIsEmpty()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            var insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            cache.RemoveAllDevices();
            var countAfterRemoval = cache.BullseyeMemoryCacheCount();

            Assert.Equal(0, countAfterRemoval);

        }

        [Fact]
        public void RemovedDeviceCallback_DeviceIsRemoved_EvictionCallbackIsCalled()
        {
            //todo
            var flag = true;
            var keyword = "Not In";
            var removedDevice = "";

            void RemoveAction(IBullseyeDevice device)
            {
                removedDevice = device.Id;
                flag = false;
                keyword = "Now Removed";
            }

            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, RemoveAction);
            
            Assert.True(flag);
            Assert.Equal("Not In", keyword);
            cache.AddDevice(dev01, 3);

            cache.RemoveDevice(dev01);

            Assert.False(flag);
            Assert.Equal(dev01.Id, removedDevice);
            Assert.Equal("Now Removed", keyword);
        }

        [Fact]
        public void RemoveDevice_RemoveSingleDevice_RemovesDevice()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            var insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            cache.RemoveDevice(dev01);
            var countAfterRemoval = cache.BullseyeMemoryCacheCount();

            Assert.Equal(4, countAfterRemoval);

        }

        [Theory]
        [InlineData(null)]
        public void RemoveDevice_RemoveNullDevice_ThrowsException(BullseyeDevice device)
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            var insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            Assert.Throws<ArgumentNullException>(() => cache.RemoveDevice(device));

        }

        [Fact]
        public void RemoveDevice_RemoveDeviceNotInCache_NoDeviceRemoved()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            List<IBullseyeDevice> insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            cache.RemoveDevice(dev06);
            var countAfterRemoval = cache.BullseyeMemoryCacheCount();

            Assert.Equal(5, countAfterRemoval);

        }


        [Fact]
        public void UpdatedDevice_UpdateDeviceInCache_PayloadChanged()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddDevice(dev01, 3);
            var editedPayload = dev01.Payload + "edit!";
            var dev01copy = new BullseyeDevice(dev01.Id, editedPayload);

            cache.UpdateDevice(dev01copy, 3);
            
            Assert.NotEqual(dev01copy.Payload, dev01.Payload);

        }

        [Fact]
        public void UpdatedDevice_UpdateDeviceInCache_DeviceInCacheIsUpdated()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddDevice(dev01, 3);
            var editedPayload = dev01.Payload + "edit!";
            var dev01copy = new BullseyeDevice(dev01.Id, editedPayload);

            cache.UpdateDevice(dev01copy, 3);
            var cachedDevice = cache.GetDevice(dev01);

            Assert.Equal(editedPayload, cachedDevice.Payload);

        }

        [Fact]
        public void UpdateDevice_UpdateDeviceThatDoesNotExist_DeviceIsInserted()
        {
            var cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.UpdateDevice(dev02, 3);
            var size = cache.BullseyeMemoryCacheCount();
            Assert.Equal(1, size);
        }

        [Fact]
        public void UpdateDevice_UpdateWithBadTime_ThrowsException()
        {
            BullseyeMemoryCache cache = new BullseyeMemoryCache(Cache, helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);
            cache.AddDevice(dev02, 5);
            cache.UpdateDevice(dev02, -3);

            var size = cache.BullseyeMemoryCacheCount();
            Assert.Equal(1, size);
        }



        [Fact]
        public void UpdatedDeviceCallback_StateUnderTest_ExpectedBehavior()
        {
            //todo
        }
    }
}
