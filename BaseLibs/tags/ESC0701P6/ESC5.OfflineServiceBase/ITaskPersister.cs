using System.Collections.Generic;

namespace ESC5.OfflineService
{
    public interface ITaskPersister<T>
    {
        void SaveTask(T task);
        IEnumerable<T> GetTaskList();
        void RemoveTask(T task);
    }
}
