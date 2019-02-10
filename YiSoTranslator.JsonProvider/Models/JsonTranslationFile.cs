namespace YiSoTranslator.JsonProvider
{
    using System;

    /// <summary>
    /// A class that represent the JSON file that holds the translations
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class JsonTranslationFile : IYiSoTranslationFile
    {
        private string _name;
        private string _fullName;

        /// <summary>
        /// the name of the file, without extension
        /// </summary>
        /// <exception cref="ArgumentException">if the name is null, empty or withSpace</exception>
        /// <exception cref="TranslationFolderMissingExceptions">if the translation folder not found</exception>
        /// <exception cref="TranslationFileMissingExceptions">if the translation JSON file not found</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the translation file Extension not set to .json</exception>
        public string Name
        {
            get => _name;
            set
            {
                _fullName = FileHelper.GetJsonFilePath(value);
                _name = value;
            }
        }

        /// <summary>
        /// the full path of the file
        /// </summary>
        public string FullName
        {
            get => _fullName;
            set
            {
                _name = FileHelper.GetJsonFileName(value);
                _fullName = value;
            }
        }

        /// <summary>
        /// the type of the file
        /// </summary>
        public FileType Type { get; }

        /// <summary>
        /// construct with initializing the FullName
        /// </summary>
        public JsonTranslationFile(string file, bool IsFullPath = false)
        {
            Type = FileType.Json;

            if (IsFullPath)
            {
                FullName = file;
                return;
            }

            Name = file;
        }

        #region Overrides

        /// <summary>
        /// return true if the objects are equals base on the FullName
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Boolean value of the comparison</returns>
        public override bool Equals(object obj)
        {
            var file = (JsonTranslationFile)obj;
            return file.FullName == this.FullName;
        }

        /// <summary>
        /// return the name and the path of the file
        /// </summary>
        /// <returns>the name of the obj</returns>
        public override string ToString()
        {
            return $"File Name : '{Name}', and the full Name is : '{FullName}' ";
        }

        /// <summary>
        /// get the hash value
        /// </summary>
        /// <returns>hash value</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }
}
