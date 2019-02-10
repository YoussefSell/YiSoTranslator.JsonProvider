using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace YiSoTranslator.JsonProvider.Test
{
    [TestClass]
    public class TranslationFileTest
    {
        string _path;

        [TestInitialize]
        public void Init()
        {
            _path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _path = Path.Combine(_path, "test.json");

            if (!File.Exists(_path))
                File.Create(_path);
        }

        [TestMethod]
        public void ValidTranslationFileInstant()
        {
            //- Arrange
            var filewithName = new JsonTranslationFile("main");
            var filewithFullName = new JsonTranslationFile(_path, true);

            //- Act
            Console.WriteLine(filewithFullName.ToString());
            Console.WriteLine(filewithName.ToString());

            //- Assert
        }

        [TestMethod]
        [ExpectedException(typeof(TranslationFileMissingExceptions))]
        public void InValidTranslationFileInstant()
        {
            //- Arrange
            var file = new JsonTranslationFile("nameNotExist");

            //- Act

            //- Assert
        }
    }
}
