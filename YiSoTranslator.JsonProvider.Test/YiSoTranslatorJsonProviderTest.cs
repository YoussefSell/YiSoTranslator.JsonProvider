using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;
using System.Threading;

namespace YiSoTranslator.JsonProvider.Test
{
    [TestClass]
    public class YiSoTranslatorJsonProviderTest
    {
        YiSoTranslatorJsonProvider _provider;

        [TestInitialize]
        public void Init()
        {
            _provider = new YiSoTranslatorJsonProvider("main");

            var tg = _provider.Find("hello_text");
            _provider.Clear();
            _provider.Add(tg);
            _provider.SaveChanges();
        }

        [TestMethod]
        public void ValidAddOperation()
        {
            //- Arrange
            var translationGroup = new TranslationsGroup("test_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            //- Act
            _provider.Add(translationGroup);
            _provider.SaveChanges();

            //- Assert
            Assert.AreEqual(2, _provider.Count);
        }

        [TestMethod]
        public void ValidAddOperationWithEvent()
        {
            //- Arrange
            var translationGroup = new TranslationsGroup("testwithevent_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            var NewRecordName = "";
            ListChangedType operationtype = ListChangedType.Update;

            _provider.TranslationsGroupsListChanged += (s, e) =>
            {
                operationtype = e.OperationType;
                NewRecordName = e.NewRecord.Name;
            };

            //- Act
            _provider.Add(translationGroup);
            _provider.SaveChanges();

            //- Assert
            Assert.AreEqual(2, _provider.Count);
            Assert.AreEqual(ListChangedType.Add, operationtype);
            Assert.AreEqual("testwithevent_txt", NewRecordName);
        }

        [TestMethod]
        public void ValidAddRangeOperation()
        {
            //- Arrange
            var translationGroup = new TranslationsGroup("test1_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            var translationGroup2 = new TranslationsGroup("test2_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            var translationGroup3 = new TranslationsGroup("test3_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            //- Act
            _provider.AddRange(new TranslationsGroup[] {
                translationGroup,
                translationGroup2,
                translationGroup3
            });
            _provider.SaveChanges();

            //- Assert
            Assert.AreEqual(4, _provider.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(TranslationsGroupAlreadyExistException))]
        public void InValidAddOperation()
        {
            //- Arrange
            var translationGroup = new TranslationsGroup("hello_text")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            //- Act
            _provider.Add(translationGroup);
            _provider.SaveChanges();

            //- Assert
        }

        [TestMethod]
        public void ValidRemoveOperation()
        {
            //- Arrange
            var tg = _provider.Find("hello_text");

            //- Act
            _provider.Remove(tg);

            //- Assert
            Assert.AreEqual(0, _provider.Count);
        }

        [TestMethod]
        public void ValidRemoveOperation2()
        {
            //- Arrange

            //- Act
            _provider.Remove("hello_text");

            //- Assert
            Assert.AreEqual(0, _provider.Count);
        }

        [TestMethod]
        public void ValidRemoveOperationWithEvent()
        {
            //- Arrange
            var tg = _provider.Find("hello_text");
            var removedRecordName = "";
            ListChangedType operationtype = ListChangedType.Update;

            _provider.TranslationsGroupsListChanged += (s, e) =>
            {
                operationtype = e.OperationType;
                removedRecordName = e.OldRecord.Name;
            };

            //- Act
            _provider.Remove(tg);

            //- Assert
            Assert.AreEqual(0, _provider.Count);
            Assert.AreEqual("Hello_text", removedRecordName);
            Assert.AreEqual(ListChangedType.Delete, operationtype);
        }

        [TestMethod]
        public void ValidLookUpOperations()
        {
            //- Arrange
            var tg = _provider.Find("hello_text");

            //- Act
            var r1 = _provider.Contains(tg);
            var r2 = _provider.IndexOf(tg);
            var r3 = _provider.IsExist("hello_text");
            var r4 = _provider.Find(t => t.Name.ToLower() == "hello_text");


            //- Assert
            Assert.AreEqual(true, r1);
            Assert.AreEqual(0, r2);
            Assert.AreEqual(true, r3);
            Assert.AreEqual(1, r4.Count());
        }

        [TestMethod]
        public void ValidUpdateOperation()
        {
            _provider.Update("hello_text", "newName");

            var r1 = _provider.IsExist("hello_text");
            var r2 = _provider.IsExist("newName");

            Assert.AreEqual(false, r1);
            Assert.AreEqual(true, r2);
        }

        [TestMethod]
        public void ValidUpdateOperationWithEvent()
        {
            var NEwRecordName = "";
            ListChangedType operationtype = ListChangedType.Delete;

            _provider.TranslationsGroupsListChanged += (s, e) =>
            {
                operationtype = e.OperationType;
                NEwRecordName = e.NewRecord?.Name;
            };

            _provider.Update("hello_text", "newName");

            var r1 = _provider.IsExist("hello_text");
            var r2 = _provider.IsExist("newName");

            Assert.AreEqual(false, r1);
            Assert.AreEqual(true, r2);
            Assert.AreEqual("newName", NEwRecordName);
            Assert.AreEqual(ListChangedType.Update, operationtype);
        }

        [TestMethod]
        public void ValidUpdateOperation2()
        {
            var tg = _provider.Find("hello_text");
            var indexoldTg = _provider.IndexOf(tg);
            var ntg = new TranslationsGroup("updateTest_text")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            _provider.Update(tg, ntg);
            var indexNewTg = _provider.IndexOf(ntg);

            var r1 = _provider.IsExist("hello_text");
            var r2 = _provider.IsExist("updateTest_text");

            Assert.AreEqual(false, r1);
            Assert.AreEqual(true, r2);
            Assert.AreEqual(indexoldTg, indexNewTg);
        }

        [TestMethod]
        public void ValidUpdateOperationWithEvent2()
        {
            var NEwRecordName = "";
            var OldRecordName = "";
            ListChangedType operationtype = ListChangedType.Delete;

            _provider.TranslationsGroupsListChanged += (s, e) =>
            {
                operationtype = e.OperationType;
                NEwRecordName = e.NewRecord.Name;
                OldRecordName = e.OldRecord.Name;
            };

            var tg = _provider.Find("hello_text");
            var indexoldTg = _provider.IndexOf(tg);
            var ntg = new TranslationsGroup("updateTest_text")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            _provider.Update(tg, ntg);
            var indexNewTg = _provider.IndexOf(ntg);

            var r1 = _provider.IsExist("hello_text");
            var r2 = _provider.IsExist("updateTest_text");

            Assert.AreEqual(false, r1);
            Assert.AreEqual(true, r2);
            Assert.AreEqual("updatetest_text", NEwRecordName.ToLower());
            Assert.AreEqual("hello_text", OldRecordName.ToLower());
            Assert.AreEqual(ListChangedType.Update, operationtype);
        }

        [TestMethod]
        public void ValidSaveTofileOperation()
        {
            var _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _path = Path.Combine(_path, "test.json");

            if (!File.Exists(_path))
                File.Create(_path);

            var file = new JsonTranslationFile(_path, true);

            var translationGroup = new TranslationsGroup("test1_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            var translationGroup2 = new TranslationsGroup("test2_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            var translationGroup3 = new TranslationsGroup("test3_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            _provider.AddRange(new TranslationsGroup[]{
                translationGroup,
                translationGroup2,
                translationGroup3
            });

            _provider.SaveToFile(file);

            var provider = new YiSoTranslatorJsonProvider(file);

            Assert.AreEqual(4 ,provider.Count);
        }

        [TestMethod]
        public void ValidReadFromfileOperation()
        {
            var _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _path = Path.Combine(_path, "test2.json");

            if (!File.Exists(_path))
                File.Create(_path);

            var file = new JsonTranslationFile(_path, true);
            var provider = new YiSoTranslatorJsonProvider(file);

            var translationGroup = new TranslationsGroup("test1_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            var translationGroup2 = new TranslationsGroup("test2_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            var translationGroup3 = new TranslationsGroup("test3_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            provider.AddRange(new TranslationsGroup[] {
                translationGroup,
                translationGroup2,
                translationGroup3
            });

            provider.SaveChanges();

            //give the process some time to release the file
            Thread.Sleep(100);

            _provider.ReadFromFile(file);

            Assert.AreEqual(4, _provider.Count);
        }

        [TestMethod]
        public void ValidDataSourceChangedUpdateEventRaising()
        {
            var _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _path = Path.Combine(_path, "test.json");

            if (!File.Exists(_path))
                File.Create(_path);

            var filewithFullName = new JsonTranslationFile(_path, true);
            var type = "";

            var provider1 = new YiSoTranslatorJsonProvider(filewithFullName);
            provider1.DataSourceChanged += (s, e) =>
            {
                type = e.ChangeType.ToString();
                Console.WriteLine($"{e.ChangeType}");
            };

            var translationGroup = new TranslationsGroup("test1_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            var translationGroup2 = new TranslationsGroup("test2_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            var translationGroup3 = new TranslationsGroup("test3_txt")
                .Add(new Translation(Languages.English_UnitedStates.Code(), "test translation"));

            _provider.AddRange(new TranslationsGroup[] {
                translationGroup,
                translationGroup2,
                translationGroup3
            });

            _provider.SaveToFile(filewithFullName);

            Assert.AreEqual(DataSourceChanged.Updated.ToString(), type);
        }

        [TestMethod]
        public void ValidDataSourceChangedDeleteEventRaising()
        {
            var _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _path = Path.Combine(_path, "test.json");

            if (!File.Exists(_path))
                File.Create(_path);

            var filewithFullName = new JsonTranslationFile(_path, true);
            var type = "";

            var provider1 = new YiSoTranslatorJsonProvider(filewithFullName);
            provider1.DataSourceChanged += (s, e) =>
            {
                type = e.ChangeType.ToString();
                Console.WriteLine($"{e.ChangeType}");
            };

            File.Delete(filewithFullName.FullName);

            // give it some time to catch the event, 10 millisecond is enough
            Thread.Sleep(10);

            Assert.AreEqual(DataSourceChanged.Deleted.ToString(), type);
        }
    }
}
