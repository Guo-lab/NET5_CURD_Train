using ProjectBase.Domain;

namespace ImportLibrary
{
    //将需要导入的数据持久化到其它介质，供离线导入程序读取
    public interface IImportItemPersister:IBusinessDelegate
    {
        void Persist(string orderType, string parameterJSON, string importJSON);
    }
}
