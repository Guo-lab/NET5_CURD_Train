using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ImportLibrary
{
    /// <summary>
    /// 导入非常耗时的情况下可将要导入的内容存储到队列中，通过离线程序导入
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="ResultT"></typeparam>
    public abstract class BaseOfflineImporter<T, ResultT> : BaseImporter<T, ResultT> where T : BaseImportItem
                                                             where ResultT : BaseImportResult<T>, new()
    {
        public IImportItemPersister Persister { get; set; }
        protected override void DoImport()
        {
            //可以先验证合法性，只把验证通过的放入队列，也可以直接放入队列，离线程序真正导入时再验证
            IEnumerable<T> items = this.Result.ImportItems.Where(x => x.Result != ImportLineResult.Failed);
            Persister.Persist(typeof(T).Name, GetParameterJSON(),GroupItemsbyOrder(items));
        }

        //导入文件通常是Flat格式的，存储到队列中按单据分组。离线程序会按单据逐个导入
        protected virtual string GroupItemsbyOrder(IEnumerable<T> items)
        {
            return JsonConvert.SerializeObject(items);
        }

        protected abstract string GetParameterJSON();
    }
}
