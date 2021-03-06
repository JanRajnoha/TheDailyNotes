using Framework.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;

namespace Framework.Service
{
    public static class LogService
    {
        private static Dictionary<string, List<string>> messageQueue = new Dictionary<string, List<string>>();
        private static Dictionary<string, bool> savingProcedure = new Dictionary<string, bool>();

        /// <summary>
        /// Create new log file with specified name
        /// </summary>
        /// <param name="fileName">New file name</param>
        /// <returns></returns>
        public static async Task CreateLogFileAsync(string fileName = "Log.txt")
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName);
        }

        /// <summary>
        /// Create new log file with specified name
        /// </summary>
        /// <param name="fileName">New file name</param>
        public static void CreateLogFile(string fileName = "Log.txt")
        {
            CreateLogFileAsync(fileName).Wait();
        }

        /// <summary>
        /// Add new log message. Logging must be enabled in <see cref="Constants"/>
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="fileName">Log file name</param>
        /// <returns></returns>
        public static async Task AddLogMessageAsync(string message, string fileName = "Log.txt")
        {
            if (Constants.Instance.LogEnable != null)
                if (!(bool)Constants.Instance.LogEnable)
                    return;

            if (!messageQueue.ContainsKey(fileName))
            {
                messageQueue.Add(fileName, new List<string>());
                savingProcedure.Add(fileName, false);
            }

            if (!File.Exists(ApplicationData.Current.LocalFolder.Path + $@"\{fileName}"))
                await CreateLogFileAsync();

            messageQueue[fileName].Add(message);

            if (!savingProcedure[fileName])
            {
                savingProcedure[fileName] = true;
                SaveAllLogFile(fileName);
            }
        }

        /// <summary>
        /// Add new log message. Logging must be enabled in <see cref="Constants"/>
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="fileName">Log file name</param>
        public static void AddLogMessage(string message, string fileName = "Log.txt")
        {
            AddLogMessageAsync(message, fileName).Wait();
        }

        /// <summary>
        /// Save all log file messages
        /// </summary>
        /// <param name="fileName">Log file name</param>
        /// <param name="Attempts"></param>
        private static async void SaveAllLogFile(string fileName, int Attempts = 0)
        {
            try
            {
                using (StreamWriter LogStream = new StreamWriter(await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.OpenIfExists)))
                {
                    while (messageQueue[fileName].Count != 0)
                    {
                        LogStream.WriteLine($"{DateTime.Now}: {messageQueue[fileName][0]}");
                        messageQueue[fileName].RemoveAt(0);
                    }
                }

                savingProcedure[fileName] = false;
            }

            // When file is unavailable
            catch (Exception e) when ((e.Message.Contains("denied") || e.Message.Contains("is in use")) && (Attempts < 10))
            {
                SaveAllLogFile(fileName, Attempts + 1);
            }

            catch /*(Exception e)*/
            {
                return;
            }
        }

        /// <summary>
        /// Save log message to specified log file
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="fileName">Log file</param>
        /// <param name="Attempts"></param>
        [Obsolete("Function is not reliable.")]
        private static async void SaveLogFile(string message, string fileName, int Attempts = 0)
        {
            try
            {
                using (StreamWriter LogStream = new StreamWriter(await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(fileName, CreationCollisionOption.OpenIfExists)))
                {
                    LogStream.WriteLine($"{DateTime.Now}: {message}");
                }
            }

            // When file is unavailable
            catch (Exception e) when ((e.Message.Contains("denied") || e.Message.Contains("is in use")) && (Attempts < 10))
            {
                SaveLogFile(message, fileName, Attempts + 1);
            }

            catch /*(Exception e)*/
            {
                return;
            }
        }
    }
}
