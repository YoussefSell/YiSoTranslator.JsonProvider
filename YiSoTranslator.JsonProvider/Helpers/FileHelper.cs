namespace YiSoTranslator.JsonProvider
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Collections.Generic;

    [System.Diagnostics.DebuggerStepThrough]
    internal static class FileHelper
    {
        #region static props 

        /// <summary>
        /// the Root Folder for the project currently executing
        /// </summary>
        public static string ProjectRootFolder;

        /// <summary>
        /// get the Translation Directory in the root folder of the current executing project
        /// </summary>
        public static string TranslationFolder;

        /// <summary>
        /// the path of the backupFolder 
        /// </summary>
        public static string BackUpFolder;

        static FileHelper()
        {
            ProjectRootFolder = GetProjectRootDirectory();
            TranslationFolder = GetTranslationFolder();
            BackUpFolder = Path.Combine(ProjectRootFolder, "BackUp");

            string GetProjectRootDirectory()
            {
                //get the path of the DLL file in the debug folder
                string dllFilePath = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(dllFilePath);
            }

            string GetTranslationFolder()
            {
                string path = Path.Combine(GetProjectRootDirectory(), "Translations");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                return path;
            }
        } 

        #endregion

        /// <summary>
        /// Get the JSON File of the translation file name
        /// </summary>
        /// <param name="fileName"> the JSON File name containing the translations</param>
        /// <returns>the full path of the JSON file</returns>
        /// <exception cref="TranslationFolderMissingExceptions">if the translation folder not found</exception>
        /// <exception cref="TranslationFileMissingExceptions">if the translation JSON file not found</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the translation file Extension not set to .json</exception>
        internal static string GetJsonFilePath(string fileName)
        {
            if (fileName.IsNull())
                throw new ArgumentException();

            var path = Path.Combine(TranslationFolder, $"{fileName}.json");

            if (!File.Exists(path))
                throw new TranslationFileMissingExceptions(fileName);

            return path;
        }

        /// <summary>
        /// Get the JSON File Name
        /// </summary>
        /// <param name="filePath"> the full path of the JSON file</param>
        /// <returns>the full path of the JSON file</returns>
        /// <exception cref="TranslationFolderMissingExceptions">if the translation folder not found</exception>
        /// <exception cref="TranslationFileMissingExceptions">if the translation JSON file not found</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the translation file Extension not set to .json</exception>
        internal static string GetJsonFileName(string filePath)
        {
            if (filePath.IsNull())
                throw new ArgumentException();

            if (!File.Exists(filePath))
                throw new TranslationFileMissingExceptions(filePath);

            var extension = Path.GetExtension(filePath);

            if (extension != ".json")
                throw new NonValidTranslationFileExtensionExceptions(extension);

            return Path.GetFileNameWithoutExtension(filePath);
        }

        /// <summary>
        /// get the content of the file as string
        /// </summary>
        /// <param name="file">the Translation file to read from</param>
        /// <returns>the content</returns>
        internal static string GetContent(this JsonTranslationFile file)
        {
            return File.ReadAllText(file.FullName);
        }

        /// <summary>
        /// save the content to the translation file
        /// </summary>
        /// <param name="file">the translation file where the data will be saved to</param>
        /// <param name="content">the content</param>
        /// <returns>true if saved, false if a problem exist</returns>
        /// <exception cref="TranslationFileNotSpecifiedExceptions">if file is null or invalid</exception>
        /// <exception cref="TranslationFileMissingExceptions">if the file not exist</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the doesn't have a .json extension</exception>
        internal static bool SaveContent(this JsonTranslationFile file, string content)
        {
            try
            {
                File.WriteAllText(file.FullName, content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// create a backUp file
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        internal static void CreateBackUp(string fileName, string content)
        {
            if (!Directory.Exists(BackUpFolder))
                Directory.CreateDirectory(BackUpFolder);

            var backupfile = Path.Combine(BackUpFolder, $"{fileName}-backup.json");
            File.WriteAllText(backupfile, content);
        }
       
        /// <summary>
        /// validate the translation File
        /// </summary>
        /// <param name="file">the file to check</param>
        /// <exception cref="ArgumentNullException">if the file is null</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the file extension not set to JSON</exception>
        /// <exception cref="FileNotFoundException">if the file doesn't exist</exception>
        internal static void Validate(this JsonTranslationFile file)
        {
            if (file is null)
                throw new ArgumentNullException("the file is null");

            var extention = Path.GetExtension(file.FullName);

            if (extention != ".json")
                throw new NonValidTranslationFileExtensionExceptions(extention);

            if (!File.Exists(file.FullName))
                throw new FileNotFoundException("the file is not exist");
        }
    }

    [System.Diagnostics.DebuggerStepThrough]
    internal static class CollectionHelpers
    {
        internal static string AsJson(this IEnumerable<TranslationsGroup> translations)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(translations);
        }
    }
}
