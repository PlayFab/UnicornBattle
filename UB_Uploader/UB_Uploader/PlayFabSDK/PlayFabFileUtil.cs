#if XAMARIN

using PlayFab.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PlayFab
{
    public static partial class PlayFabUtil
    {
#if !NETFX_CORE
        [ThreadStatic]
        private static StringBuilder _sb;
        /// <summary>
        /// A threadsafe way to block and load a text file
        /// 
        /// Load a text file, and return the file as text.
        /// Used for small (usually json) files.
        /// </summary>
        public static string ReadAllFileText(string filename)
        {
            if (!File.Exists(filename))
                return "";

            if (_sb == null)
                _sb = new StringBuilder();
            _sb.Length = 0;

            var fs = new FileStream(filename, FileMode.Open);
            var br = new BinaryReader(fs);
            while (br.BaseStream.Position != br.BaseStream.Length)
                _sb.Append(br.ReadChar());

            return _sb.ToString();
        }
#else
        public static string ReadAllFileText(string filename)
        {
            var task = ReadAllFileTextAsync(filename);

            var output = "";
            try
            {
                output = task.Result;
            }
            catch (AggregateException agEx)
            {
                foreach (var eachEx in agEx.InnerExceptions)
                {
                    System.Diagnostics.Debug.WriteLine("Each Exception:");
                    System.Diagnostics.Debug.WriteLine(eachEx.Message);
                }
            }

            return output;
        }

        public static async Task<string> ReadAllFileTextAsync(string fullpath)
        {
            var foldername = Path.GetDirectoryName(fullpath);
            var filename = Path.GetFileName(fullpath);
            var folder = await Windows.Storage.StorageFolder.GetFolderFromPathAsync(foldername);
            var file = await folder.GetFileAsync(filename);
            return await Windows.Storage.FileIO.ReadTextAsync(file);
        }
#endif
    }
}
#endif
