using System;
using System.Linq;
using System.Collections.Generic;

namespace YiSoTranslator.JsonProvider
{
    /// <summary>
    /// a class to work with the SQLITE Database
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class YiSoTranslatorJsonProvider : IYiSoTranslationProvider
    {
        #region Private fields

        private readonly YiSoTranslatorJsonFileProvider _db;

        #endregion

        #region Public Prop

        /// <summary>
        /// the collection of translations
        /// </summary>
        public IEnumerable<TranslationGroup> TranslationsGroupList
        {
            get => _db.TranslationsGroups;
        }

        /// <summary>
        /// the total number of translations in the list
        /// </summary>
        public int Count { get => _db.TranslationsGroups.Count; }

        /// <summary>
        /// the translation file
        /// </summary>
        public TranslationFile TranslationFile { get => _db.TranslationFile; }

        /// <summary>
        /// event raised when the list changed
        /// </summary>
        public event EventHandler<TranslationGroupDataSourceChangedEventArgs> DataSourceChanged;

        #endregion

        #region Constructors

        /// <summary>
        /// constructor with Translation file
        /// </summary>
        /// <param name="translationFile">the translation file</param>
        public YiSoTranslatorJsonProvider(TranslationFile translationFile)
        {
            _db = new YiSoTranslatorJsonFileProvider(translationFile);
        }

        /// <summary>
        /// constructor with the file name with extension (ex: main.json)
        /// </summary>
        /// <param name="filename">name of the file</param>
        public YiSoTranslatorJsonProvider(string filename)
            : this(new TranslationFile(filename)) { }

        #endregion

        #region LookUp Methods

        /// <summary>
        /// look for a translation group in the list by name
        /// </summary>
        /// <param name="name">the name of the translation group</param>
        /// <returns>the translation Group if exist, null if nothing found</returns>
        public TranslationGroup Find(string name)
        {
            return _db.TranslationsGroups.FirstOrDefault(t => t.Name.ToLower() == name.ToLower());
        }

        /// <summary>
        /// look for a translation group in the list by a predicate
        /// </summary>
        /// <param name="predicate">the predicate to look with</param>
        /// <returns>the list of translation Group if exist, null if nothing found</returns>
        public IEnumerable<TranslationGroup> Find(Func<TranslationGroup, bool> predicate)
        {
            return _db.TranslationsGroups.Where(predicate);
        }

        /// <summary>
        /// get the index of the given item
        /// </summary>
        /// <param name="item">the translation Group</param>
        /// <returns>the index</returns>
        public int IndexOf(TranslationGroup item)
        {
            return _db.TranslationsGroups.IndexOf(item);
        }

        /// <summary>
        /// determine whether the element exist in the list
        /// </summary>
        /// <param name="item">the TranslationGroup to look for</param>
        /// <returns>true if exist, false if not </returns>
        public bool Contains(TranslationGroup item)
        {
            return _db.TranslationsGroups.Contains(item);
        }

        #endregion

        #region Read and save data

        /// <summary>
        /// dispose the object
        /// </summary>
        public void Dispose()
        {
            _db.Dispose();
        }

        /// <summary>
        /// save the translations to the dataSource
        /// </summary>
        /// <returns>true if data is saved, false if not</returns>
        public bool SaveChanges()
        {
            return _db.SaveChanges();
        }

        /// <summary>
        /// save the translation to the given file. a full path mast be provided
        /// </summary>
        /// <param name="file">the full path to the file</param>
        /// <returns>true if saved, false if a problem exist</returns>
        public bool SaveToFile(string file)
        {
            return _db.SaveToFile(file);
        }

        /// <summary>
        /// read translation from the given file
        /// the non existing translations will be add to the list of translation
        /// others will be escaped
        /// </summary>
        /// <param name="file">file to read from</param>
        public void ReadFromFile(string file)
        {
            var translationGroupsList = _db.GetTranslationsFromFile(file);
            AddRange(translationGroupsList);
        }

        #endregion

        #region DataManipilation

        /// <summary>
        /// Add the translation group to the list
        /// </summary>
        /// <param name="translationGroup">translation group to be added</param>
        /// <exception cref="TranslationGroupAlreadyExistException">if the translation group Already Exist</exception>
        public TranslationGroup Add(TranslationGroup translationGroup)
        {
            var tg = Find(translationGroup.Name);

            if (tg != null)
                throw new TranslationGroupAlreadyExistException(translationGroup.Name);

            _db.TranslationsGroups.Add(translationGroup);

            OnDataChanged(ListChangedType.Add, Count - 1, null, translationGroup);
            return translationGroup;
        }

        /// <summary>
        /// Add the translation groups to the list, only non existing one will be add, others will be escaped
        /// </summary>
        /// <param name="translationGroups">translation groups to be added</param>
        public void AddRange(IEnumerable<TranslationGroup> translationGroups)
        {
            foreach (var item in translationGroups)
            {
                if (Find(item.Name) == null)
                {
                    _db.TranslationsGroups.Add(item);
                }
            }
        }

        /// <summary>
        /// update the old TranslationGroup name with the new name
        /// </summary>
        /// <param name="oldTranslationGroupName">the old translation Croup name</param>
        /// <param name="newTranslationGroupName">the new translation Group</param>
        /// <returns>the updated TranslationGroup</returns>
        /// <exception cref="TranslationGroupNotExistException">if the old translation group not exist</exception>
        /// <exception cref="TranslationGroupAlreadyExistException">if the new translation group Already exist</exception>
        public TranslationGroup Update(string oldTranslationGroupName, string newTranslationGroupName)
        {
            var old = Find(oldTranslationGroupName) 
                ?? throw new TranslationGroupNotExistException(oldTranslationGroupName);

            var newT = Find(newTranslationGroupName)
                ?? throw new TranslationGroupAlreadyExistException(newTranslationGroupName);

            old.Name = newTranslationGroupName;

            OnDataChanged(ListChangedType.Update, IndexOf(old), null, old);
            return old;
        }

        /// <summary>
        /// delete the translations Group from the list
        /// </summary>
        /// <param name="item">translation group to be removed</param>
        /// <returns>true if the item deleted, false if not</returns>
        /// <exception cref="TranslationGroupNotExistException">if the translation group not exist</exception>
        public bool Remove(TranslationGroup item)
        {
            return Remove(item.Name);
        }

        /// <summary>
        /// delete the translations Group from the list
        /// </summary>
        /// <param name="TranslationGroupName">Name of translation group to be removed</param>
        /// <returns>true if the item deleted, false if not</returns>
        /// <exception cref="TranslationGroupNotExistException">if the translation group not exist</exception>
        public bool Remove(string TranslationGroupName)
        {
            var TG = Find(TranslationGroupName) 
                ?? throw new TranslationGroupNotExistException(TranslationGroupName);

            _db.TranslationsGroups.Remove(TG);

            OnDataChanged(ListChangedType.Delete, Count - 1, TG, null);
            return true;
        }

        /// <summary>
        /// remove all elements from the list
        /// </summary>
        public void Clear()
        {
            _db.TranslationsGroups.Clear();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Raise the DataSourceChanged Event
        /// </summary>
        /// <param name="listChangedType">the change type</param>
        /// <param name="index">index where the change occur</param>
        /// <param name="oldRecord">the old record</param>
        /// <param name="newRecord">the new record</param>
        private void OnDataChanged(ListChangedType listChangedType, int index, TranslationGroup oldRecord, TranslationGroup newRecord)
        {
            DataSourceChanged?
                .Invoke(this, new TranslationGroupDataSourceChangedEventArgs(listChangedType, index, oldRecord, newRecord));
        }

        #endregion
    }
}
