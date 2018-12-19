using System;
using Xunit;
using Moq;
using System.Collections.Generic;
using Baxter.Bullseye.MemoryCache;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BullseyeCacheTestProject
{
    public class BullseyeMemoryCacheUnitTests
    {
        readonly BullseyeDevice dev01 = new BullseyeDevice("device 01", "{ 01 some device info; device info here; }");
        readonly BullseyeDevice dev02 = new BullseyeDevice("device 02", "{ 02 some device info; device info here; }");
        readonly BullseyeDevice dev03 = new BullseyeDevice("device 03", "{ 03 some device info; device info here; }");
        readonly BullseyeDevice dev04 = new BullseyeDevice("device 04", "{ 04 some device info; device info here; }");
        readonly BullseyeDevice dev05 = new BullseyeDevice("device 05", "{ 05 some device info; device info here; }");
        readonly BullseyeDevice dev06 = new BullseyeDevice("device 06", "{ 06 some device info; device info here; }");
        readonly BullseyeDevice dev07 = new BullseyeDevice("device 07", "{ 07 some device info; device info here; }");
        readonly BullseyeDevice dev08 = new BullseyeDevice("device 08", "{ 08 some device info; device info here; }");
        private readonly BullseyeDeviceHelper _helper = new BullseyeDeviceHelper();
        
        readonly MemoryCache _cache = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = 1024
            }
        );

        #region DummyBullseyeDeviceTests
        [Theory]
        [InlineData(null)]
        public void BullseyeDeviceEquals_NullDeviceEquals_ThrowsException(BullseyeDevice nullDevice)
        {
            Assert.Throws<ArgumentNullException>(() => dev08.Equals(nullDevice));
        }

        #endregion

        #region CacheLibraryTests
        [Fact]
        public void AddMultipleDevices_ListOfThreeDevicesAdded_CacheSizeIsThree()
        {
            var mockBc = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mockBc.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);
            var list = new List<IBullseyeDevice>(){ dev05, dev01, dev02 };
            cache.AddMultipleDevices(list, 3);
            var size = cache.Count;
            Assert.Equal(3, size);
        }

        [Fact]
        public void AddMultipleDevices_EmptyListAdded_CacheSizeIsZero()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);
            var list = new List<IBullseyeDevice>();
            cache.AddMultipleDevices(list, 3);
            var size = cache.Count;
            Assert.Equal(0, size);
        }
        
        [Theory]
        [InlineData(null)]
        public void AddMultipleDevices_SearchForNullList_ThrowsException(List<IBullseyeDevice> list)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            Assert.Throws<ArgumentNullException>(() => cache.AddMultipleDevices(list, 3));
        }

        [Fact]
        public void AddMultipleDevices_AddDevicesWithBadTime_ThrowsException()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);
            var list = new List<IBullseyeDevice>() { dev01, dev02, dev03 };

            Assert.Throws<ArgumentOutOfRangeException>(() => cache.AddMultipleDevices(list, -3));
        }
        
        [Fact]
        public void AddDevice_AddSingleDevice_ReturnsOne()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            cache.AddDevice(dev02,3);
            var size = cache.Count;
            Assert.Equal(1, size);
        }

        [Fact]
        public void AddDevice_AddModifiedDeviceAlreadyInCache_ReturnsOne()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            cache.AddDevice(dev02, 3);
            var updatedDevice = new BullseyeDevice(dev02.Id, "EDITED DEVICE");

            cache.AddDevice(updatedDevice, 3);

            var size = cache.Count;
            Assert.Equal(1, size);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(-3)]
        [InlineData(0)]
        public void AddDevice_AddSingleDeviceWithBadTime_ThrowsException(int seconds)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            Assert.Throws<ArgumentOutOfRangeException>(() => cache.AddDevice(dev02, seconds));
        }
        
        [Fact]
        public void BullseyeMemoryCache_DefaultConstructor_EmptyCacheCreated()
        {
            var moqCache = new Mock<IMemoryCache>();
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            var cache = new BullseyeMemoryCache(moqCache.Object, logger);
           
            var size = cache.Count;
            Assert.Equal(0, size);
        }
        
        [Fact]
        public void CheckCacheForMultipleDevices_FiveDevicesAddedThreeSearchedFor_AllSearchedForFound()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var insertList = new List<IBullseyeDevice>() { dev05, dev01, dev02, dev03, dev04 };
            var searchList = new List<IBullseyeDevice>() { dev06, dev07, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            var returnList = cache.CheckCacheForMultipleDevices(searchList);
            var size = returnList.Count;

            Assert.Equal(3, size);
        }

        [Fact]
        public void Count_EmptyCache_ReturnsZero()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var size = cache.Count;

            Assert.Equal(0, size);
        }

        [Fact]
        public void Count_SingleDeviceInCache_ReturnsOne()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            cache.AddDevice(dev01, 3);
            var size = cache.Count;

            Assert.Equal(1, size);
        }
        
        [Fact]
        public void GetDevice_CheckDeviceInCacheMatchDeviceSentIn_DevicesMatch()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var insertList = new List<IBullseyeDevice>{ dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            var myObj = (BullseyeDevice)cache.GetDevice(dev01);

            Assert.True(myObj.Equals(dev01));
        }

        [Fact]
        public void GetDevice_CheckDeviceInCacheMatchesDeviceReturnedByKeySentIn_DevicesMatch()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            var myObj = (BullseyeDevice)cache.GetDevice("device 01");

            var key1 = myObj.Id;
            var key2 = dev01.Id;
            var value1 = myObj.Payload;
            var value2 = dev01.Payload;

            Assert.Equal(key1, key2);
            Assert.Equal(value1, value2);
            Assert.True(myObj.Equals(dev01));
        }

        [Theory]
        [InlineData(null, null)]
        public void GetDevice_SendInNullDevice_ThrowsException(String DeviceString, BullseyeDevice Device)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            cache.AddDevice(dev02, 5);
            
            Assert.Null(cache.GetDevice(dev05));
        }

        [Fact]
        public void NewDeviceCallback_StartUpActionDoesSomething_ActionsArePerformed()
        {
            var flag = false;
            var keyword = "Not In";
            var addedDevice = "";
            
            void StartUpAction(IBullseyeDevice device)
            {
                addedDevice = device.Id;
                flag = true;
                keyword = "Now In";
            }

            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            
            var logger = mock.Object;
            
            var cache = new BullseyeMemoryCache(_cache, StartUpAction, _helper.UpdateAction, _helper.EvictionAction, logger);


            Assert.False(flag);
            Assert.Equal("Not In", keyword);
            cache.AddDevice(dev01, 3);
            Assert.True(flag);
            Assert.Equal("Now In", keyword);
            Assert.Equal(dev01.Id, addedDevice);
        }

        [Fact]
        public void NewDeviceCallback_StartUpActionDefaultCacheConstructor_NoNewDeviceCallbackIsCalledButDeviceGetsAdded()
        {
            var flag = false;
            var keyword = "Not In";

            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            
            var logger = mock.Object;
            
            var cache = new BullseyeMemoryCache(_cache, logger);


            Assert.False(flag);
            Assert.Equal("Not In", keyword);
            cache.AddDevice(dev01, 3);
            Assert.Equal("Not In", keyword);
            Assert.False(flag);
            Assert.Equal(1, cache.Count);
        }

        [Fact]
        public void RemoveAllDevices_RemoveDevicesFromCache_CacheIsEmpty()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            cache.RemoveAllDevices();
            var countAfterRemoval = cache.Count;

            Assert.Equal(0, countAfterRemoval);
        }

        [Fact]
        public void RemovedDeviceCallback_DeviceIsRemoved_EvictionCallbackIsCalled()
        {
            var flag = true;
            var keyword = "Not In";
            var removedDevice = "";

            void RemoveAction(IBullseyeDevice device)
            {
                removedDevice = device.Id;
                flag = false;
                keyword = "Now Removed";
            }

            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction, RemoveAction, logger);
            
            Assert.True(flag);
            Assert.Equal("Not In", keyword);
            cache.AddDevice(dev01, 3);

            cache.RemoveDevice(dev01);

            Assert.False(flag);
            Assert.Equal(dev01.Id, removedDevice);
            Assert.Equal("Now Removed", keyword);
        }

        [Fact]
        public void DefaultConstructor_DefaultCacheConstructor_NoRemoveDeviceCallbackExistsButDeviceGetsRemoved()
        {

            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, logger);


            cache.AddDevice(dev01, 3);
            Assert.Equal(1, cache.Count);

            cache.RemoveDevice(dev01);
            Assert.Equal(0, cache.Count);
        }
        
        [Theory]
        [InlineData(null)]
        public void DefaultConstructor_NullCache_ExceptionHandled(MemoryCache nullMemoryCache)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            Assert.Throws<ArgumentNullException>(() => new BullseyeMemoryCache(nullMemoryCache, logger));
        }

        [Fact]
        public void DefaultConstructor_DefaultCacheConstructor_NoUpdateDeviceCallbackExistsButDeviceGetsUpdated()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, logger);


            cache.AddDevice(dev01, 3);
            Assert.Equal(1, cache.Count);

            cache.UpdateDevice(dev01, 10);
            Assert.Equal(1, cache.Count);
        }

        [Fact]
        public void RemoveDevice_RemoveSingleDevice_RemovesDevice()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            cache.RemoveDevice(dev01);
            var countAfterRemoval = cache.Count;

            Assert.Equal(4, countAfterRemoval);
        }

        [Fact]
        public void RemoveDevice_RemoveSingleDeviceByKey_RemovesDevice()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            cache.RemoveDevice(dev01.Id);
            var countAfterRemoval = cache.Count;

            Assert.Equal(4, countAfterRemoval);
        }
        
        [Theory]
        [InlineData(null)]
        public void RemoveDevice_RemoveNullDevice_ThrowsException(BullseyeDevice device)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            Assert.Throws<ArgumentNullException>(() => cache.RemoveDevice(device));
        }

        [Fact]
        public void RemoveDevice_RemoveDeviceNotInCache_NoDeviceRemoved()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            List<IBullseyeDevice> insertList = new List<IBullseyeDevice> { dev05, dev01, dev02, dev03, dev04 };
            cache.AddMultipleDevices(insertList, 3);

            cache.RemoveDevice(dev06);
            var countAfterRemoval = cache.Count;

            Assert.Equal(5, countAfterRemoval);
        }
        
        [Fact]
        public void UpdateDevice_UpdateDeviceInCache_PayloadChanged()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            cache.AddDevice(dev01, 3);
            var editedPayload = dev01.Payload + "edit!";
            var dev01Copy = new BullseyeDevice(dev01.Id, editedPayload);

            cache.UpdateDevice(dev01Copy, 3);
            
            Assert.NotEqual(dev01Copy.Payload, dev01.Payload);
        }
        
        [Fact]
        public void UpdateDevice_UpdateDeviceInCache_DeviceInCacheIsUpdated()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            cache.AddDevice(dev01, 3);
            var editedPayload = dev01.Payload + "edit!";
            var dev01Copy = new BullseyeDevice(dev01.Id, editedPayload);

            cache.UpdateDevice(dev01Copy, 3);
            var cachedDevice = cache.GetDevice(dev01);

            Assert.Equal(editedPayload, cachedDevice.Payload);
        }

        [Fact]
        public void UpdateDevice_UpdateDeviceThatDoesNotExist_DeviceIsInserted()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            cache.UpdateDevice(dev02, 3);
            var size = cache.Count;
            Assert.Equal(1, size);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(-3)]
        [InlineData(0)]
        public void UpdateDevice_UpdateWithBadTime_ThrowsException(int seconds)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            cache.AddDevice(dev02, 5);

            var size = cache.Count;
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.UpdateDevice(dev02, seconds));
        }

        [Theory]
        [InlineData(null)]
        public void AddDevice_GetsNullIBullseyeDevice_ThrowsNullException(IBullseyeDevice device)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            Assert.Throws<ArgumentNullException>(() => cache.AddDevice(device, 3));
        }
        
        [Theory]
        [InlineData(null)]
        [InlineData(-3)]
        [InlineData(0)]
        public void AddDevice_GetsBadIntSeconds_ThrowsException(int seconds)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            Assert.Throws<ArgumentOutOfRangeException>(() => cache.AddDevice(dev01, seconds));
        }

        [Fact]
        public void AddDevice_AddDeviceThatAlreadyExists_UpdateDeviceCallbackRunsInstead()
        {
            var flag = false;
            var keyword = "No Device";
            var updatedDevice = "";

            void UpdateAction(IBullseyeDevice device)
            {
                updatedDevice = device.Id;
                flag = true;
                keyword = "Device Already Exists";
            }

            void StartUpAction(IBullseyeDevice device)
            {
                updatedDevice = device.Id;
                flag = true;
                keyword = "New Device";
            }

            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            
            var logger = mock.Object;
           
            var cache = new BullseyeMemoryCache(_cache, StartUpAction, UpdateAction, _helper.EvictionAction, logger);

            cache.AddDevice(dev07, 3);
            Assert.True(flag);
            Assert.Equal(dev07.Id, updatedDevice);
            Assert.Equal("New Device", keyword);
            Assert.Equal(1, cache.Count);

            cache.AddDevice(dev07, 10);
            Assert.True(flag);
            Assert.Equal(dev07.Id, updatedDevice);
            Assert.Equal("Device Already Exists", keyword);
            Assert.Equal(1, cache.Count);
        }

        [Theory]
        [InlineData(null)]
        public void AddMultipleDevices_GetsNullList_ThrowsNullException(List<IBullseyeDevice> list)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            Assert.Throws<ArgumentNullException>(() => cache.AddMultipleDevices(list, 3));
        }

        [Fact]
        public void AddMultipleDevices_GetsZeroInt_ThrowsOutOfRangeException()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var list = new List<IBullseyeDevice> { dev01, dev02, dev03 };
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.AddMultipleDevices(list, 0));
        }

        [Fact]
        public void AddMultipleDevices_GetsNegativeInt_ThrowsOutOfRangeException()
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var list = new List<IBullseyeDevice> { dev01, dev02, dev03 };
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.AddMultipleDevices(list, -3));
        }

        [Theory]
        [InlineData(null)]
        public void AddMultipleDevices_GetsNullInt_ThrowsNullException(int seconds)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            var list = new List<IBullseyeDevice>{ dev01, dev02, dev03 };
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.AddMultipleDevices(list, seconds));
        }

        [Fact]
        public void AddMultipleDevices_AddDevicesThatAlreadyExist_UpdateDeviceCallbackRunsInstead()
        {
            var flag = false;
            var keyword = "No Device";

            void UpdateAction(IBullseyeDevice device)
            {
                flag = true;
                keyword = "Device Already Exists";
            }

            void StartUpAction(IBullseyeDevice device)
            {
                flag = true;
                keyword = "New Device";
            }

            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            
            var logger = mock.Object;
            
            var cache = new BullseyeMemoryCache(_cache, StartUpAction, UpdateAction, _helper.EvictionAction, logger);
            var addList = new List<IBullseyeDevice>{dev01, dev02, dev03};
            var sameList = new List<IBullseyeDevice> { dev01, dev02, dev03 };

            cache.AddMultipleDevices(addList, 4);
            Assert.True(flag);
            Assert.Equal("New Device", keyword);
            Assert.Equal(3, cache.Count);

            cache.AddMultipleDevices(sameList, 6);
            Assert.True(flag);
            Assert.Equal("Device Already Exists", keyword);
            Assert.Equal(3, cache.Count);
        }

        [Fact]
        public void AddMultipleDevices_AddMixOfNewAndExistingDevices_AllDevicesAddedAsExpected()
        {
            var flag = false;
            var updatedDeviceList = new List<IBullseyeDevice>();
            var keyword = "no devices";

            void UpdateAction(IBullseyeDevice device)
            {
                flag = true;
                updatedDeviceList.Add(device);
            }

            void StartUpAction(IBullseyeDevice device)
            {
                flag = true;
                keyword = "New Device";
            }

            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
           
            var cache = new BullseyeMemoryCache(_cache, StartUpAction, UpdateAction, _helper.EvictionAction, logger);
            var addList = new List<IBullseyeDevice> { dev01, dev02, dev03, dev04 };
            var sameList = new List<IBullseyeDevice> { dev01, dev02, dev05, dev06 };

            cache.AddMultipleDevices(addList, 4);
            Assert.True(flag);
            Assert.Equal("New Device", keyword);
            Assert.Equal(4, cache.Count);

            cache.AddMultipleDevices(sameList, 6);

            Assert.Contains<IBullseyeDevice>(dev01, updatedDeviceList);
            Assert.Contains<IBullseyeDevice>(dev02, updatedDeviceList);
            Assert.DoesNotContain<IBullseyeDevice>(dev05, updatedDeviceList);
            Assert.DoesNotContain<IBullseyeDevice>(dev06, updatedDeviceList);
            Assert.True(flag);
            Assert.Equal(6, cache.Count);
        }

        [Theory]
        [InlineData(null)]
        public void BullseyeMemoryCacheDefault_GetsNullIMemoryCache_ThrowsNullException(IMemoryCache cache)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            Assert.Throws<ArgumentNullException>(() => new BullseyeMemoryCache(cache, logger));
        }

        [Theory]
        [InlineData(null)]
        public void BullseyeMemoryCacheWithCallbacks_GetsNullIMemoryCache_ThrowsNullException(IMemoryCache cache)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            Assert.Throws<ArgumentNullException>(() => new BullseyeMemoryCache(cache, _helper.StartUpAction, _helper.UpdateAction, _helper.EvictionAction, logger));
        }

        [Theory]
        [InlineData(null)]
        public void BullseyeMemoryCache_GetsNullPreCallbackAction_ThrowsNullException(Action<IBullseyeDevice> preCallbackAction)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            Assert.Throws<ArgumentNullException>(() => new BullseyeMemoryCache(_cache, preCallbackAction, _helper.UpdateAction, _helper.EvictionAction, logger));
        }

        [Theory]
        [InlineData(null)]
        public void BullseyeMemoryCache_GetsNullUpdateCallbackAction_ThrowsNullException(Action<IBullseyeDevice> updateCallbackAction)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            Assert.Throws<ArgumentNullException>(() => new BullseyeMemoryCache(_cache, _helper.StartUpAction, updateCallbackAction, _helper.EvictionAction, logger));
        }

        [Theory]
        [InlineData(null)]
        public void BullseyeMemoryCache_GetsNullEvictionCallbackAction_ThrowsNullException(Action<IBullseyeDevice> removedCallbackAction)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            Assert.Throws<ArgumentNullException>(() => new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction, removedCallbackAction, logger));
        }

        [Theory]
        [InlineData(null)]
        public void CheckCacheForMultipleDevices_GetsNullList_ReturnsNull(List<IBullseyeDevice> list)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction, _helper.EvictionAction, logger);
            Assert.Null(cache.CheckCacheForMultipleDevices(list));
        }

        [Theory]
        [InlineData(null)]
        public void GetDevice_GetsNullIBullseyeDevice_ReturnsNull(IBullseyeDevice device)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction, _helper.EvictionAction, logger);
            Assert.Null(cache.GetDevice(device));
        }

        [Theory]
        [InlineData(null)]
        public void GetDevice_GetsNullString_ReturnsNull(string key)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction, _helper.EvictionAction, logger);
            Assert.Null(cache.GetDevice(key));
        }

        [Theory]
        [InlineData(null)]
        public void RemoveDevice_GetsNullIBullseyeDevice_ThrowsNullException(IBullseyeDevice device)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction, _helper.EvictionAction, logger);
            Assert.Throws<ArgumentNullException>(() => cache.RemoveDevice(device));
        }

        [Theory]
        [InlineData(null)]
        public void RemoveDevice_GetsNullString_ThrowsNullException(string key)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction, _helper.EvictionAction, logger);
            Assert.Throws<ArgumentNullException>(() => cache.RemoveDevice(key));
        }

        [Fact]
        public void RemoveDevice_RemoveDeviceThatIsNotInCache_DoesNotThrowException()
        {
            //What should happen in this situation? todo
            //todo
            var flag = false;
            var keyword = "No Device";
            var removedDevice = "";

            void RemoveAction(IBullseyeDevice device)
            {
                removedDevice = device.Id;
                flag = true;
                keyword = "Device Has Been Deleted";
            }

            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction, RemoveAction, logger);

            cache.AddDevice(dev07, 3);
            cache.RemoveDevice(dev01); //this device is not in the cache
            cache.RemoveDevice(dev01.Id); //this device identified by string is not in the cache

            Assert.False(flag);
            Assert.Equal("", removedDevice);
            Assert.NotEqual("Device Has Been Deleted", keyword);
            Assert.Equal(1, cache.Count);
            
            cache.RemoveDevice(dev07); //this device is in the cache
            Assert.True(flag);
            Assert.Equal(dev07.Id, removedDevice);
            Assert.Equal("Device Has Been Deleted", keyword);
            Assert.Equal(0, cache.Count);
        }

        [Theory]
        [InlineData(null)]
        public void UpdateDevice_GetsNullIBullseyeDevice_ThrowsNullException(IBullseyeDevice device)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            Assert.Throws<ArgumentNullException>(() => cache.UpdateDevice(device, 3));
        }

        [Theory]
        [InlineData(null)]
        [InlineData(-3)]
        [InlineData(0)]
        public void UpdateDevice_GetsBadIntSeconds_ThrowsException(int seconds)
        {
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;

            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, _helper.UpdateAction,
                _helper.EvictionAction, logger);

            cache.AddDevice(dev07, 3);
            Assert.Equal(1, cache.Count);
            Assert.Throws<ArgumentOutOfRangeException>(() => cache.UpdateDevice(dev07, seconds));
        }
        
        [Fact]
        public void UpdateDevice_UpdateDeviceThatDoesNotExist_NewDeviceCallbackRunsInstead()
        {
            var flag = false;
            var keyword = "No Device";
            var updatedDevice = "";

            void UpdateAction(IBullseyeDevice device)
            {
                updatedDevice = device.Id;
                flag = true;
                keyword = "Device Already Exists";
            }

            void StartUpAction(IBullseyeDevice device)
            {
                updatedDevice = device.Id;
                flag = true;
                keyword = "New Device";
            }

            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            var cache = new BullseyeMemoryCache(_cache, StartUpAction, UpdateAction, _helper.EvictionAction, logger);

            cache.UpdateDevice(dev07, 3);
            Assert.True(flag);
            Assert.Equal(dev07.Id, updatedDevice);
            Assert.Equal("New Device", keyword);
            Assert.Equal(1, cache.Count);
        }
        
        [Fact]
        public void UpdatedDeviceCallback_UpdateActionDoesSomething_ActionIsPerformed()
        {
            var flag = false;
            var keyword = "Not Updated";
            var updatedDevice = "";

            void UpdateAction(IBullseyeDevice device)
            {
                updatedDevice = device.Id;
                flag = true;
                keyword = "Now Updated";
            }

            //todo
            //work in progress
            var mock = new Mock<ILogger<IBullseyeMemoryCache>>();
            var logger = mock.Object;
            
            var cache = new BullseyeMemoryCache(_cache, _helper.StartUpAction, UpdateAction, _helper.EvictionAction, logger);

            Assert.False(flag);
            Assert.Equal("Not Updated", keyword);
            cache.AddDevice(dev01, 3);
            
            cache.UpdateDevice(dev01, 9);
            
            Assert.True(flag);
            Assert.Equal(dev01.Id, updatedDevice);
            Assert.Equal("Now Updated", keyword);
        }
        
        #endregion
    }


}
