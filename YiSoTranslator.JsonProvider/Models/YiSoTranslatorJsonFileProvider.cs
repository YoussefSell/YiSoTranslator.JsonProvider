namespace YiSoTranslator.JsonProvider
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// warper class on the core implementation of the JSON translation provider
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    internal class YiSoTranslatorJsonFileProvider : IDisposable
    {
        /// <summary>
        /// the collection of translation groups
        /// </summary>
        public IList<TranslationGroup> TranslationsGroups { get; set; }
        
        /// <summary>
        /// the translation file
        /// </summary>
        public TranslationFile TranslationFile { get; set; }

        /// <summary>
        /// Event raised when the list is changed
        /// </summary>
        public event EventHandler<TranslationGroupDataSourceChangedEventArgs> ListChanged;

        /// <summary>
        /// constructor with the Translation file
        /// </summary>
        /// <param name="translationFile">the translation file</param>
        public YiSoTranslatorJsonFileProvider(TranslationFile translationFile)
        {
            GetTranslationsFromFile(translationFile);
        }

        /// <summary>
        /// dispose the object
        /// </summary>
        public void Dispose()
        {
            TranslationFile = null;
            TranslationsGroups = null;
        }

        /// <summary>
        /// save the changes
        /// </summary>
        /// <returns>1 if saved, -1 if any problem</returns>
        public bool SaveChanges()
        {
            return SaveToFile(TranslationFile);
        }

        /// <summary>
        /// get the list of translations from the JSON File and 
        /// populate the list property in current manager object
        /// and assign file to Translation File priority
        /// </summary>
        /// <param name="file"></param>
        public void GetTranslationsFromFile(TranslationFile file)
        {
            TranslationsGroups = JsonConvert.DeserializeObject<List<TranslationGroup>>(file.GetContent())
                     ?? new List<TranslationGroup>();

            ListChanged?.Invoke(this, new TranslationGroupDataSourceChangedEventArgs(ListChangedType.NewRefrence, -1, null, null));
            TranslationFile = file;
        }

        /// <summary>
        /// get the list of translations from the JSON File
        /// </summary>
        /// <param name="file">file where to read the data</param>
        public IList<TranslationGroup> GetTranslationsFromFile(string file)
        {
            return JsonConvert.DeserializeObject<List<TranslationGroup>>(FileHelper.GetContent(file))
                     ?? new List<TranslationGroup>();
        }

        /// <summary>
        /// save the list of translations to the translation file
        /// </summary>
        /// <param name="file">the translation file where the data will be saved to</param>
        /// <returns>true if saved, false if a problem exist</returns>
        /// <exception cref="TranslationFileNotSpecifiedExceptions">if file is null or invalid</exception>
        /// <exception cref="TranslationFileMissingExceptions">if the file not exist</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the doesn't have a .json extension</exception>
        public bool SaveToFile(TranslationFile file)
        {
            string json = JsonConvert.SerializeObject(TranslationsGroups);
            return file.SaveContent(json);
        }

        /// <summary>
        /// save the translation to the given file. a full path mast be provided
        /// </summary>
        /// <param name="file">the full path to the file</param>
        /// <returns>true if saved, false if a problem exist</returns>
        /// <exception cref="TranslationFileNotSpecifiedExceptions">if file is null or invalid</exception>
        /// <exception cref="TranslationFileMissingExceptions">if the file not exist</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the doesn't have a .json extension</exception>
        public bool SaveToFile(string file)
        {
            string json = JsonConvert.SerializeObject(TranslationsGroups.ToArray());
            return FileHelper.SaveContent(file, json);
        }
    }
}