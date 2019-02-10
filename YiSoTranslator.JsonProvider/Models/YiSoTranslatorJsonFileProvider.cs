namespace YiSoTranslator.JsonProvider
{
    using System;
    using System.IO;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    /// <summary>
    /// warper class on the core implementation of the JSON translation provider
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    internal class YiSoTranslatorJsonFileProvider
    {
        CustomFileWatcher _watcher;

        /// <summary>
        /// constructor with the Translation file
        /// </summary>
        /// <param name="translationFile">the translation file</param>
        public YiSoTranslatorJsonFileProvider(JsonTranslationFile translationFile)
        {
            TranslationFile = translationFile;

            TranslationsGroups = JsonConvert
                .DeserializeObject<List<TranslationsGroup>>(TranslationFile.GetContent())
                ?? new List<TranslationsGroup>();

            InitWatcher();
        }

        /// <summary>
        /// event raised when the file has been modified
        /// </summary>
        public event EventHandler<DataSourceChangedEventArgs> DataSourceChanged;

        /// <summary>
        /// the collection of translation groups
        /// </summary>
        public IList<TranslationsGroup> TranslationsGroups { get; set; }

        /// <summary>
        /// the translation file
        /// </summary>
        /// <exception cref="ArgumentNullException">if the file is null</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the file extension not set to JSON</exception>
        /// <exception cref="FileNotFoundException">if the file doesn't exist</exception>
        public JsonTranslationFile TranslationFile { get; }

        /// <summary>
        /// save the changes
        /// </summary>
        /// <returns>1 if saved, -1 if any problem</returns>
        public bool SaveChanges()
        {
            return TranslationFile.SaveContent(TranslationsGroups.AsJson());
        }

        /// <summary>
        /// save the list of translations to the translation file
        /// </summary>
        /// <param name="file">the translation file where the data will be saved to</param>
        /// <returns>true if saved, false if a problem exist</returns>
        /// <exception cref="ArgumentException">if file type is not set to JSON</exception>
        /// <exception cref="TranslationFileNotSpecifiedExceptions">if file is null or invalid</exception>
        /// <exception cref="TranslationFileMissingExceptions">if the file not exist</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the doesn't have a .json extension</exception>
        public bool SaveToFile(IYiSoTranslationFile file)
        {
            if (file.Type != FileType.Json)
                throw new ArgumentException("Only JSON Files Types are accepted");

            return (file as JsonTranslationFile).SaveContent(TranslationsGroups.AsJson());
        }

        private void InitWatcher()
        {
            _watcher = new CustomFileWatcher()
            {
                DirectoryToWatch = Path.GetDirectoryName(TranslationFile.FullName),
                FileToWatch = $"{TranslationFile.Name}.json",
                EnableRaisingEvents = true
            };

            _watcher.FileDeleted += (s, e)
                => DataSourceChanged?.Invoke(this, new DataSourceChangedEventArgs(YiSoTranslator.DataSourceChanged.Deleted));
            _watcher.FileUpdated += (s, e)
                => DataSourceChanged?.Invoke(this, new DataSourceChangedEventArgs(YiSoTranslator.DataSourceChanged.Updated));
        }

        /// <summary>
        /// get the list of translations from the JSON File
        /// </summary>
        /// <param name="file">file where to read the data</param>
        /// <exception cref="ArgumentException">if file type is not set to JSON</exception>
        public static IEnumerable<TranslationsGroup> GetTranslationsFromFile(IYiSoTranslationFile file)
        {
            if (file.Type != FileType.Json)
                throw new ArgumentException("Only JSON Files Types are accepted");

            return JsonConvert
                .DeserializeObject<List<TranslationsGroup>>(FileHelper.GetContent(file as JsonTranslationFile))
                ?? new List<TranslationsGroup>();
        }

        //inner class
        class CustomFileWatcher
        {
            FileSystemWatcher _watcher;
            int _eventRaisedCount = 0;

            public event EventHandler<EventArgs> FileUpdated;
            public event EventHandler<EventArgs> FileDeleted;

            public CustomFileWatcher()
            {
                _watcher = new FileSystemWatcher()
                {
                    IncludeSubdirectories = false,
                };

                _watcher.Changed += Watcher_Changed;
                _watcher.Renamed += Watcher_Changed;
                _watcher.Deleted += Watcher_Deleted;
            }

            ~CustomFileWatcher()
            {
                _watcher.Changed -= Watcher_Changed;
                _watcher.Renamed -= Watcher_Changed;
                _watcher.Deleted -= Watcher_Deleted;
                _watcher.Dispose();
            }

            public string DirectoryToWatch { get => _watcher.Path; set => _watcher.Path = value; }
            public string FileToWatch { get => _watcher.Filter; set => _watcher.Filter = value; }
            public bool EnableRaisingEvents { get => _watcher.EnableRaisingEvents; set => _watcher.EnableRaisingEvents = value; }

            private void Watcher_Changed(object sender, FileSystemEventArgs e)
            {
                if (_eventRaisedCount == 0)
                {
                    FileUpdated?.Invoke(this, new EventArgs());
                    _eventRaisedCount++;
                }
                else if (_eventRaisedCount > 0)
                    _eventRaisedCount = 0;
            }

            private void Watcher_Deleted(object sender, FileSystemEventArgs e)
                => FileDeleted?.Invoke(this, new EventArgs());
        }
    }
}