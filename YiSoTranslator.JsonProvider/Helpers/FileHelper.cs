namespace YiSoTranslator.JsonProvider
{
    using System;
    using System.IO;
    using System.Reflection;

    [System.Diagnostics.DebuggerStepThrough]
    internal static class FileHelper
    {
        /// <summary>
        /// Get the Root Folder for the project currently executing
        /// </summary>
        /// <returns>the root directory of the currently executing project</returns>
        internal static string GetProjectRootDirectory()
        {
            //get the path of the DLL file in the debug folder
            string dllFilePath = Assembly.GetExecutingAssembly().Location;
            return Path.GetDirectoryName(dllFilePath);
        }

        /// <summary>
        /// create the Translation Directory in the root folder of the current executing project
        /// </summary>
        /// <returns>the path to the translation directory</returns>
        internal static string CreateTranslationDirectory()
        {
            string path = GetProjectRootDirectory() + @"\Translations";

            Directory.CreateDirectory(path);

            return path;
        }

        /// <summary>
        /// Get the JSON File of the translation
        /// </summary>
        /// <param name="jsonFile"> the JSON File containing the translations</param>
        /// <returns>the full path of the JSON file</returns>
        /// <exception cref="TranslationFolderMissingExceptions">if the translation folder not found</exception>
        /// <exception cref="TranslationFileMissingExceptions">if the translation JSON file not found</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the translation file Extension not set to .json</exception>
        internal static string GetJsonFilePath(string jsonFile)
        {
            string path = GetProjectRootDirectory() + @"\Translations";

            if (!Directory.Exists(path))
                throw new TranslationFolderMissingExceptions();

            var file = new FileInfo(path + @"\" + jsonFile);

            if (!file.Exists)
                throw new TranslationFileMissingExceptions(jsonFile);

            if (file.Extension != ".json")
                throw new NonValidTranslationFileExtensionExceptions(file.Extension);

            return file.FullName;
        }

        /// <summary>
        /// get the content of the file as string
        /// </summary>
        /// <param name="file">the Translation file to read from</param>
        /// <returns>the content</returns>
        public static string GetContent(this TranslationFile file)
        {
            var path = GetJsonFilePath(file.Name);
            return GetContent(path);
        }

        /// <summary>
        /// get the content of the file as string
        /// </summary>
        /// <param name="file">the Translation file to read from</param>
        /// <returns>the content</returns>
        public static string GetContent(string file)
        {
            return File.ReadAllText(file);
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
        public static bool SaveContent(this TranslationFile file, string content)
        {
            return SaveContent(file.FullName, content);
        }

        /// <summary>
        /// save the translation to the given file. a full path mast be provided
        /// </summary>
        /// <param name="file">the full path to the file</param>
        /// <param name="content">the content to be saved</param>
        /// <returns>true if saved, false if a problem exist</returns>
        /// <exception cref="TranslationFileNotSpecifiedExceptions">if file is null or invalid</exception>
        /// <exception cref="TranslationFileMissingExceptions">if the file not exist</exception>
        /// <exception cref="NonValidTranslationFileExtensionExceptions">if the doesn't have a .json extension</exception>
        public static bool SaveContent(string file, string content)
        {
            if (file.IsNull())
                throw new TranslationFileNotSpecifiedExceptions();

            var fileinfo = new FileInfo(file);

            if (fileinfo.Extension != ".json")
                throw new NonValidTranslationFileExtensionExceptions(fileinfo.Extension);

            if (!fileinfo.Exists)
                fileinfo.Create();

            try
            {
                File.WriteAllText(file, content);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
