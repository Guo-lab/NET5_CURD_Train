using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ESC5.OfflineService
{
    public class FileTaskPersister<T,IdT> : ITaskPersister<T> where T:TaskBase<IdT>
    {
        private string _path;
        private static object locker = new object();
        public FileTaskPersister(string path)
        {
            _path = path;
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
        }
        public IEnumerable<T> GetTaskList()
        {
            foreach(string file in Directory.GetFiles(_path, "*.task"))
            {
                yield return  JsonConvert.DeserializeObject<T>(
                    File.ReadAllText(file), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
        }

        public void RemoveTask(T task)
        {
            try
            {
                lock (locker)
                {
                    File.Delete(_path + "\\" + task.Key);
                }
            }
            catch
            {

            }
        }

        public void SaveTask(T task)
        {
            lock (locker)
            {
                File.WriteAllText(_path + "\\" + task.Key, task.JsonString);
            }
        }
    }
}
