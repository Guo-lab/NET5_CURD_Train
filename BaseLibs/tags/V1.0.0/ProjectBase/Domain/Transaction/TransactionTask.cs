using System;

namespace ProjectBase.Domain.Transaction
{
    /// <summary>
    ///
	///与事务关联的任务，在事务生命期的某个点开始执行。
    ///缺省为在事务提交后异步执行。
    ///如果任务需要数据库连接，需指定连接类型
    ///@author Rainy
    /// </summary>
    public class TransactionTask
    {

        public Action Task { get; set; }
        public TaskRunOnEnum RunOn { get; set; } = TaskRunOnEnum.AfterCommit;
        public bool Async { get; set; } = true;

        public enum TaskRunOnEnum
        {
            AfterCommit,//提交成功后
            OnCommitFail,//提交失败后
            OnActionFail//尚未提交前的代码执行异常时
        }
    }
}
