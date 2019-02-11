using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YiSoTranslator.JsonProvider
{
    /// <summary>
    /// class to work with Translations using JSON files
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class YiSoTranslatorJsonProvider : IYiSoTranslationProvider
    {
        #region Private fields

        private bool _unsavedChanges;
        private readonly YiSoTranslatorJsonFileProvider _db;

        #endregion

        #region Public Prop

        /// <summary>
        /// the collection of translations Groups
        /// </summary>
        public IEnumerable<TranslationsGroup> TranslationsGroupsList
        {
            get => _db.TranslationsGroups;
        }

        /// <summary>
        /// the total number of translations Groups in the list
        /// </summary>
        public int Count { get => _db.TranslationsGroups.Count; }

        /// <summary>
        /// the translation file
        /// </summary>
        public JsonTranslationFile TranslationFile { get => _db.TranslationFile; }

        /// <summary>
        /// event raised when the list changed
        /// </summary>
        public event EventHandler<TranslationsGroupsListChangedEventArgs> TranslationsGroupsListChanged;

        /// <summary>
        /// if the translation file has been modified or deleted this event will be fired
        /// </summary>
        public event EventHandler<DataSourceChangedEventArgs> DataSourceChanged;

        /// <summary>
        /// true to create back up file if the file get deleted
        /// </summary>
        public bool CreateBackupOnFileDeletion
        {
            get => _db.CreateBackupOnFileDeletion;
            set => _db.CreateBackupOnFileDeletion = value;
        }

        /// <summary>
        /// by setting this to true you will start monitoring changes on the file, 
        /// so that if the file get updated or deleted you will be notified
        /// </summary>
        public bool AllowDataSourceChangedEvent
        {
            get => _db.AllowDataSourceChangedEvent;
            set => _db.AllowDataSourceChangedEvent = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// constructor with Translation file
        /// </summary>
        /// <param name="translationFile">the translation file</param>
        public YiSoTranslatorJsonProvider(JsonTranslationFile translationFile)
        {
            _unsavedChanges = false;
            _db = new YiSoTranslatorJsonFileProvider(translationFile);
            _db.DataSourceChanged += (s, e) => DataSourceChanged?.Invoke(this, e);
        }

        /// <summary>
        /// constructor with the file name without extension (ex: main)
        /// </summary>
        /// <param name="filename">name of the file without extension</param>
        public YiSoTranslatorJsonProvider(string filename)
            : this(new JsonTranslationFile(filename)) { }

        #endregion

        #region LookUp Methods

        /// <summary>
        /// look for a translation group in the list by name
        /// </summary>
        /// <param name="name">the name of the translations group</param>
        /// <returns>the translations Group if exist, null if nothing found</returns>
        public TranslationsGroup Find(string name)
            => _db.TranslationsGroups.FirstOrDefault(t => t.Name.ToLower() == name.ToLower());

        /// <summary>
        /// look for a translation group in the list by a predicate
        /// </summary>
        /// <param name="predicate">the predicate to look with</param>
        /// <returns>the list of translation Group if exist, null if nothing found</returns>
        public IEnumerable<TranslationsGroup> Find(Func<TranslationsGroup, bool> predicate)
            => _db.TranslationsGroups.Where(predicate);

        /// <summary>
        /// get the index of the given translations Group
        /// </summary>
        /// <param name="item">the translations Group</param>
        /// <returns>the index</returns>
        public int IndexOf(TranslationsGroup item)
            => _db.TranslationsGroups.IndexOf(item);

        /// <summary>
        /// determine whether the element exist in the list
        /// </summary>
        /// <param name="item">the TranslationsGroup to look for</param>
        /// <returns>true if exist, false if not </returns>
        public bool Contains(TranslationsGroup item)
            => _db.TranslationsGroups.Contains(item);

        /// <summary>
        /// check if there is a translationsGroup with the given name
        /// </summary>
        /// <param name="name">the name of translationGroup</param>
        /// <returns>true if exist, false if not</returns>
        public bool IsExist(string name)
            => _db.TranslationsGroups.Any(t => t.Name.ToLower() == name.ToLower());

        #endregion

        #region Read and save data

        /// <summary>
        /// save the translations to the dataSource (JSON file associated with this instant)
        /// </summary>
        /// <returns>true if data is saved, false if not</returns>
        public bool SaveChanges()
        {
            if (_db.SaveChanges())
                _unsavedChanges = false;

            return !_unsavedChanges;
        }

        /// <summary>
        /// save the changes
        /// </summary>
        /// <returns>true if changes are saved</returns>
        public async Task<bool> SaveChangesAsync()
            => await Task.Run(() => SaveChanges());

        /// <summary>
        /// save the translation to file
        /// </summary>
        /// <param name="file">file where to save translations</param>
        /// <returns>true if saved</returns>
        public Task<bool> SaveToFileAsync(IYiSoTranslationFile file)
            => Task.Run(() => SaveToFile(file));

        /// <summary>
        /// save the translation to the given file, file must be of type JSON
        /// </summary>
        /// <param name="file">the JSON file to save data to</param>
        /// <returns>true if saved, false if a problem exist</returns>
        /// <exception cref="ArgumentException">if file type is not set to JSON</exception>
        /// <exception cref="TranslationFileNotSpecifiedExceptions">if file is null or invalid</exception>
        /// <exception cref="TranslationFileMissingExceptions">if the file not exist</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the doesn't have a .json extension</exception>
        public bool SaveToFile(IYiSoTranslationFile file)
            => _db.SaveToFile(file);

        /// <summary>
        /// read translation from the given file the non existing translations will be add
        /// to the list of translation others will be escaped
        /// </summary>
        /// <param name="file">file to read from</param>
        /// <exception cref="ArgumentException">if file type is not set to JSON</exception>
        public void ReadFromFile(IYiSoTranslationFile file)
            => AddRange(YiSoTranslatorJsonFileProvider
                .GetTranslationsFromFile(file).ToArray());

        /// <summary>
        /// re read the translation from the source note that the function has a flag to
        /// discard Changes by default is set to false, so that if you have unsaved changes
        /// you will get an exception
        /// </summary>
        /// <param name="discardChanges"></param>
        /// <exception cref="UnsavedChangesExceptions">if you have unsaved changes</exception>
        public void Reload(bool discardChanges = false)
        {
            if (_unsavedChanges && !discardChanges)
                throw new UnsavedChangesExceptions();

            _db.Reload();
        }

        #endregion

        #region DataManipilation

        /// <summary>
        /// Add the translations group to the list
        /// </summary>
        /// <param name="TranslationsGroup">translation group to be added</param>
        /// <exception cref="TranslationsGroupAlreadyExistException">if the translation group Already Exist</exception>
        public TranslationsGroup Add(TranslationsGroup TranslationsGroup)
        {
            var tg = Find(TranslationsGroup.Name);

            if (tg != null)
                throw new TranslationsGroupAlreadyExistException(TranslationsGroup.Name);

            _db.TranslationsGroups.Add(TranslationsGroup);

            OnTranslationsGoupListChanged(ListChangedType.Add, Count - 1, null, TranslationsGroup);
            return TranslationsGroup;
        }

        /// <summary>
        /// Add the translation groups to the list, only non existing one will be add, others will be escaped
        /// </summary>
        /// <param name="TranslationsGroups">translation groups to be added</param>
        public void AddRange(params TranslationsGroup[] TranslationsGroups)
        {
            foreach (var item in TranslationsGroups)
                if (Find(item.Name) == null)
                {
                    _db.TranslationsGroups.Add(item);
                    OnTranslationsGoupListChanged(ListChangedType.Add, Count - 1, null, item);
                }
        }

        /// <summary>
        /// update the old TranslationsGroup name with the new name
        /// this method will raise the TranslationsGoupListChanged notice that the old record 
        /// will be set to null because here we only changing the name, the TranslationsGroup
        /// is still the same reference
        /// </summary>
        /// <param name="oldTranslationsGroupName">the old translation Croup name</param>
        /// <param name="newTranslationsGroupName">the new translation Group</param>
        /// <returns>the updated TranslationsGroup</returns>
        /// <exception cref="TranslationsGroupNotExistException">if the old translation group not exist</exception>
        /// <exception cref="TranslationsGroupAlreadyExistException">if the new translation group Already exist</exception>
        public TranslationsGroup Update(string oldTranslationsGroupName, string newTranslationsGroupName)
        {
            var old = Find(oldTranslationsGroupName)
                ?? throw new TranslationsGroupNotExistException(oldTranslationsGroupName);

            var newT = Find(newTranslationsGroupName);

            if (!(newT is null))
                throw new TranslationsGroupAlreadyExistException(newTranslationsGroupName);

            old.Name = newTranslationsGroupName;

            OnTranslationsGoupListChanged(ListChangedType.Update, IndexOf(old), null, old);
            return old;
        }

        /// <summary>
        /// update the old TranslationsGroup with the new TranslationsGroup
        /// </summary>
        /// <param name="oldTranslationsGroup">the old translation Group</param>
        /// <param name="newTranslationsGroup">the new translation Group</param>
        /// <returns>the updated TranslationsGroup</returns>
        /// <exception cref="ArgumentNullException">when you pass a null parameter</exception>
        /// <exception cref="TranslationsGroupNotExistException">if the old translation group not exist</exception>
        /// <exception cref="TranslationsGroupAlreadyExistException">if the new translation group Already exist</exception>
        public TranslationsGroup Update(TranslationsGroup oldTranslationsGroup, TranslationsGroup newTranslationsGroup)
        {
            if (oldTranslationsGroup is null)
                throw new ArgumentNullException("the old Translations group is null");

            if (newTranslationsGroup is null)
                throw new ArgumentNullException("the new Translations group is null");

            var old = Find(oldTranslationsGroup.Name)
                ?? throw new TranslationsGroupNotExistException(oldTranslationsGroup.Name);

            var newT = Find(newTranslationsGroup.Name);

            if (!(newT is null))
                throw new TranslationsGroupAlreadyExistException(newTranslationsGroup.Name);

            _db.TranslationsGroups[IndexOf(old)] = newTranslationsGroup;

            OnTranslationsGoupListChanged(ListChangedType.Update, IndexOf(old), old, newTranslationsGroup);
            return old;
        }

        /// <summary>
        /// delete the translations Group from the list
        /// </summary>
        /// <param name="item">translation group to be removed</param>
        /// <returns>true if the item deleted, false if not</returns>
        /// <exception cref="TranslationsGroupNotExistException">if the translation group not exist</exception>
        public bool Remove(TranslationsGroup item) => Remove(item.Name);

        /// <summary>
        /// delete the translations Group from the list
        /// </summary>
        /// <param name="TranslationsGroupName">Name of translation group to be removed</param>
        /// <returns>true if the item deleted, false if not</returns>
        /// <exception cref="TranslationsGroupNotExistException">if the translation group not exist</exception>
        public bool Remove(string TranslationsGroupName)
        {
            var TG = Find(TranslationsGroupName)
                ?? throw new TranslationsGroupNotExistException(TranslationsGroupName);

            _db.TranslationsGroups.Remove(TG);

            OnTranslationsGoupListChanged(ListChangedType.Delete, Count - 1, TG, null);
            return true;
        }

        /// <summary>
        /// remove all elements from the list
        /// </summary>
        public void Clear()
        {
            _db.TranslationsGroups.Clear();
            OnTranslationsGoupListChanged(ListChangedType.Clear, -1, null, null);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Raise the Translations GoupList Changed Event
        /// </summary>
        /// <param name="listChangedType">the change type</param>
        /// <param name="index">index where the change occur</param>
        /// <param name="oldRecord">the old record</param>
        /// <param name="newRecord">the new record</param>
        private void OnTranslationsGoupListChanged(ListChangedType listChangedType, int index, TranslationsGroup oldRecord, TranslationsGroup newRecord)
        {
            _unsavedChanges = true;
            TranslationsGroupsListChanged?
                  .Invoke(this, new TranslationsGroupsListChangedEventArgs(listChangedType, index, oldRecord, newRecord));
        }
        #endregion
    }
}
