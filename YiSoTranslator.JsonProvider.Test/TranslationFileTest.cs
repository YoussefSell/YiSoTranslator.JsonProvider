using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace YiSoTranslator.JsonProvider.Test
{
    [TestClass]
    public class TranslationFileTest
    {
        static readonly string _desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        static readonly string _path = Path.Combine(_desktop, "test.json");
        static readonly string _path2 = Path.Combine(_desktop, "test2.txt");

        [TestInitialize]
        public void Init()
        {
            if (!File.Exists(_path))
                File.Create(_path);

            if (!File.Exists(_path2))
                File.Create(_path2);
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

        [TestMethod]
        [ExpectedException(typeof(NonValidTranslationFileExtensionExceptions))]
        public void InValidTranslationFileInstant2()
        {
            //- Arrange
            var file = new JsonTranslationFile(_path2, true);

            //- Act

            //- Assert
        }
    }
}
