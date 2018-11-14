namespace YiSoTranslator.JsonProvider
{
    using System;

    /// <summary>
    /// A class that represent the JSON file that holds the translations
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class TranslationFile : IYiSoTranslationFile
    {
        private string _name;

        /// <summary>
        /// the name of the file, with extension
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (value.IsNull())
                    throw new ArgumentNullException();

                var path = FileHelper.GetJsonFilePath(value);

                _name = value;
                FullName = path;
            }
        }

        /// <summary>
        /// the full name of the file
        /// </summary>
        public string FullName { get; private set; }

        /// <summary>
        /// the type of the file
        /// </summary>
        public FileType Type { get; }

        /// <summary>
        /// construct with initializing the FullName
        /// </summary>
        public TranslationFile(string name)
        {
            Name = name;
            Type = FileType.Json;
        }

        #region Overrides

        /// <summary>
        /// return true if the objects are equals base on the FullName
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>Boolean value of the comparison</returns>
        public override bool Equals(object obj)
        {
            var file = (TranslationFile)obj;
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
