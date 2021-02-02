using NUnit.Framework;
using MetalGearLiquid.Model;
using MetalGearLiquid.Persistence;
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace MetalGearLiquidTest
{
    public class Tests
    {
        private MetalGearLiquidModel _model;
        private MetalGearLiquidDataAccess _dataAccess;
        [SetUp]
        public void Initialize()
        {
            _dataAccess = new MetalGearLiquidFileDataAccess();
            _model = new MetalGearLiquidModel(_dataAccess);
        }

        [Test]
        public async Task SmallMapLoadTest()
        {
            String path = "..\\..\\..\\..\\WpfApp1\\Maps\\map1.txt";
            await _model.LoadGameAsync(path);

            Assert.AreEqual(11, _model._table.TableSize.x);
            Assert.AreEqual(15, _model._table.TableSize.y);
            Assert.AreEqual(0, _model.time);
            Assert.AreEqual(3, _model._table.maxGuards);
        }

        [Test]
        public async Task MediumMapLoadTest()
        {
            String path = "..\\..\\..\\..\\WpfApp1\\Maps\\map2.txt";
            await _model.LoadGameAsync(path);

            Assert.AreEqual(12, _model._table.TableSize.x);
            Assert.AreEqual(22, _model._table.TableSize.y);
            Assert.AreEqual(0, _model.time);
            Assert.AreEqual(4, _model._table.maxGuards);
        }

        [Test]
        public async Task LargeMapLoadTest()
        {
            String path = "..\\..\\..\\..\\WpfApp1\\Maps\\map3.txt";
            await _model.LoadGameAsync(path);

            Assert.AreEqual(15, _model._table.TableSize.x);
            Assert.AreEqual(30, _model._table.TableSize.y);
            Assert.AreEqual(0, _model.time);
            Assert.AreEqual(3, _model._table.maxGuards);
        }

        [Test]
        public async Task GameModelStepTest()
        {
            String path = "..\\..\\..\\..\\WpfApp1\\Maps\\map3.txt";
            await _model.LoadGameAsync(path);
            Assert.AreEqual(13, _model._table.Snek.x);
            Assert.AreEqual(1, _model._table.Snek.y);
            _model.step('w', MetalGearLiquid.Persistence.FieldType.Player, _model._table.Snek.x, _model._table.Snek.y);
            Assert.AreEqual(12, _model._table.Snek.x);
            Assert.AreEqual(1, _model._table.Snek.y);
            _model.step('d', MetalGearLiquid.Persistence.FieldType.Player, _model._table.Snek.x, _model._table.Snek.y);
            Assert.AreEqual(12, _model._table.Snek.x);
            Assert.AreEqual(2, _model._table.Snek.y);
        }

        [Test]
        public async Task GameTimeAdvancedTest()
        {
            String path = "..\\..\\..\\..\\WpfApp1\\Maps\\map1.txt";
            await _model.LoadGameAsync(path);
            Assert.AreEqual(0, _model.time);
            Assert.AreEqual(0, _model._table.spotPointsCounter);
            Assert.AreEqual(3, _model._table.Guards[0].pos.x);
            Assert.AreEqual(7, _model._table.Guards[1].pos.x);
            Assert.AreEqual(7, _model._table.Guards[2].pos.x);
            _model.AdvanceTime();
            Assert.AreEqual(1, _model.time);
            Assert.AreEqual(16, _model._table.spotPointsCounter);
            Assert.AreEqual(2, _model._table.Guards[0].pos.x);
            Assert.AreEqual(6, _model._table.Guards[1].pos.x);
            Assert.AreEqual(6, _model._table.Guards[2].pos.x);
        }

        [Test]

        public async Task GameOverTest()
        {
            String path = "..\\..\\..\\..\\WpfApp1\\Maps\\test.txt";
            await _model.LoadGameAsync(path);
            Assert.IsTrue(!_model.gotOut());
            Assert.IsTrue(!_model.isCaught());
            _model.step('a', MetalGearLiquid.Persistence.FieldType.Player, _model._table.Snek.x, _model._table.Snek.y);
            Assert.IsTrue(_model.gotOut());
            Assert.IsTrue(!_model.isCaught());
            await _model.LoadGameAsync(path);
            _model.CalculateSpotPoints();
            Assert.IsTrue(!_model.gotOut());
            Assert.IsTrue(!_model.isCaught());
            _model.step('d', MetalGearLiquid.Persistence.FieldType.Player, _model._table.Snek.x, _model._table.Snek.y);
            Assert.IsTrue(!_model.gotOut());
            Assert.IsTrue(_model.isCaught());
            await _model.LoadGameAsync(path);
            Assert.IsTrue(!_model.gotOut());
            Assert.IsTrue(!_model.isCaught());
            _model.AdvanceTime();
            Assert.IsTrue(!_model.gotOut());
            Assert.IsTrue(_model.isCaught());
        }
    }
}