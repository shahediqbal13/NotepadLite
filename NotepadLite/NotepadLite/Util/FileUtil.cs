using log4net;
using System;
using System.IO;
using System.Threading.Tasks;

namespace NotepadLite.Util
{
    public static class FileUtil
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(FileUtil));

        public static bool FileExists(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            return File.Exists(fileName);
        }

        public async static Task<string> ReadFileAsync(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentNullException("Invalid file path.");

                using (var reader = new StreamReader(fileName))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public async static Task WriteToFileAsync(string fileName, string text)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentNullException("Invalid file path.");

                using (var sw = new StreamWriter(fileName))
                {
                    await sw.WriteAsync(text);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }
    }
}
