using System.Data;

namespace ProjectBase.Data
{
    /// <summary>
    /// 存储过程的参数
    /// </summary>
    public class ProcedureParameter
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }
    }
}
