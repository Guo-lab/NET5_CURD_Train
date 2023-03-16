using Newtonsoft.Json;
using ProjectBase.Data;
using ProjectBase.Domain;
using SharpArch.Domain;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ImportLibrary
{
    //使用存储过程导入的基类
    //调用Import方法之前，应先调用SetImportParameters方法传入至少2个参数 存储过程名称和参数名称
    //存储过程至少有一个参数接收导入文件数据转换成的JSON字符串。如果需要传递更多的参数，需要重载DoImport方法
    //存储过程需要返回一个结果集，至少包含Key和ErrorMessage两列，程序会根据Key回填ErrorMessage
    public class BaseSPImporter<T, ResultT> : BaseImporter<T, ResultT> where T : BaseImportItem
                                                             where ResultT : BaseImportResult<T>, new()
    {
        public IUtilQuery UtilQuery { get; set; }
        protected override void DoImport()
        {
            Check.Require(this.ImportParameters.Length >= 2 && this.ImportParameters[0] is string && this.ImportParameters[1] is string,
                                   "先调用SetImportParameters设置存储过程和参数名称");
            IEnumerable<T> items = this.Result.ImportItems.Where(x => x.Result != ImportLineResult.Failed);
            DataSet ds = UtilQuery.ExecuteProcedureDataSet(this.ImportParameters[0].ToString(), new List<ProcedureParameter>{
                new ProcedureParameter{
                    ParameterName="@" + this.ImportParameters[1].ToString(),
                    DbType=DbType.String,
                    Value = JsonConvert.SerializeObject(items)
                }
            });
            SetImportResult(items, ds);
        }
        protected virtual void SetImportResult(IEnumerable<T> items, DataSet ds)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                T item = items.First(x => x.Key == row["Key"].ToString());
                item.ErrorMessage = row["ErrorMessage"].ToString();
                if (!string.IsNullOrEmpty(item.ErrorMessage))
                {
                    item.Result = ImportLineResult.Failed;
                }
            }
        }
    }
}
