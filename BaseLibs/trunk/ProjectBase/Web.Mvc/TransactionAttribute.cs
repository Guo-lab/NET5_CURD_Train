using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using NHibernate;
using ProjectBase.Domain;
using ProjectBase.Domain.Transaction;
using ProjectBase.Web.Mvc.Angular;
using SharpArch.NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ProjectBase.Web.Mvc
{
    //modify the sharparch transaction to commit when actionexecuted instead of resultexecuted
    public class TransactionAttribute : ActionFilterAttribute, IActionHttpMethodProvider
    {
        public static readonly string RequestAttr_CommitOnErrorResult = "NetArch_CommitOnErrorResult";
        /// <summary>
        ///     Optionally holds the factory key to be used when beginning/committing a transaction
        /// </summary>
        private readonly string factoryKey;

        public IsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        ///     When used, assumes the <see cref = "factoryKey" /> to be NHibernateSession.DefaultFactoryKey
        /// </summary>
        public TransactionAttribute()
        {
        }

        /// <summary>
        ///     Overrides the default <see cref = "factoryKey" /> with a specific factory key
        /// </summary>
        public TransactionAttribute(string factoryKey)
        {
            this.factoryKey = factoryKey;
            Order = int.MaxValue; // 保证事务处理在action执行前的最后时机执行
        }
        public TransactionAttribute(IsolationLevel isolationLevel)
        {
            IsolationLevel = isolationLevel;
            Order = int.MaxValue; // 保证事务处理在action执行前的最后时机执行
        }
        public bool RollbackOnModelStateError { get; set; }

        public IEnumerable<string> HttpMethods => new []{"POST"};

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var effectiveFactoryKey = this.GetEffectiveFactoryKey();
            var currentTransaction = NHibernateSession.CurrentFor(effectiveFactoryKey).GetCurrentTransaction();

            if (currentTransaction == null || !currentTransaction.IsActive)
            {
                ((ITransactionTaskStorage)NHibernateSession.Storage).EndTrans(effectiveFactoryKey);
                return;
            }
            TransactionTask.TaskRunOnEnum runOn;
            IList<TransactionTask> tasks = ((ITransactionTaskStorage)NHibernateSession.Storage).GetTasks(effectiveFactoryKey);
            if (filterContext.Exception != null || ShouldRollback(filterContext))
            {
                currentTransaction.Rollback();
                runOn = TransactionTask.TaskRunOnEnum.OnActionFail;
                RunTasks(tasks, runOn, filterContext);
                ShowUserError(filterContext, filterContext.Exception);
            }
            else
            {
                if (filterContext.Result is RichClientJsonResult
                    && ((RichClientJsonResult)filterContext.Result).IsErrorResult
                    && filterContext.HttpContext.Items[RequestAttr_CommitOnErrorResult] == null)
                {
                    currentTransaction.Rollback();
                    runOn = TransactionTask.TaskRunOnEnum.OnActionFail;
                    RunTasks(tasks, runOn, filterContext);
                }
                else
                {
                    try
                    {
                        currentTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        currentTransaction.Rollback();
                        runOn = TransactionTask.TaskRunOnEnum.OnCommitFail;
                        RunTasks(tasks, runOn, filterContext);
                        if (!ShowUserError(filterContext, e))
                        {
                           ((ITransactionTaskStorage)NHibernateSession.Storage).EndTrans(effectiveFactoryKey);
                            RunTasks(tasks, runOn, filterContext, true);
                            throw;
                        }
                    }
                    finally
                    {
                        currentTransaction.Dispose();
                    }
                    runOn = TransactionTask.TaskRunOnEnum.AfterCommit;
                    RunTasks(tasks, runOn, filterContext);//成功提交后执行事务关联的任务
                }
            }
            ((ITransactionTaskStorage)NHibernateSession.Storage).EndTrans(effectiveFactoryKey);
            RunTasks(tasks, runOn, filterContext, true);
        }
        private void RunTasks(IList<TransactionTask> tasks, TransactionTask.TaskRunOnEnum runOn, ActionExecutedContext filterContext,bool async=false)
        {
            var hbTransHelper = (ITransactionHelper)filterContext.HttpContext.RequestServices.GetService(typeof(ITransactionHelper));
            if (async) {
                //异步任务先排队
                ((ITransactionTaskStorage)NHibernateSession.Storage).QueueAsyncTasks(GetEffectiveFactoryKey(), tasks.Where(o => o.Async && o.RunOn == runOn).ToList());
                //此处不能用RunAsyncTasks(tasks, runOn)，因为此时启动异步线程可能与当前请求是同一线程。
            }
            else
            {
                hbTransHelper.RunSyncTasks(tasks, runOn);
            }
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var effectiveFactoryKey = this.GetEffectiveFactoryKey();
            if (IsolationLevel == null)
            {
                NHibernateSession.CurrentFor(effectiveFactoryKey).BeginTransaction();
            }
            else
            {
                NHibernateSession.CurrentFor(effectiveFactoryKey).BeginTransaction(IsolationLevel.Value);
            }
            ((ITransactionTaskStorage)NHibernateSession.Storage).BeginTrans(effectiveFactoryKey);
        }

        private string GetEffectiveFactoryKey()
        {
            return string.IsNullOrEmpty(factoryKey) ? SessionFactoryKeyHelper.GetKey() : factoryKey;
        }

        private bool ShouldRollback(ActionExecutedContext filterContext)
        {
            if (filterContext.ExceptionHandled) return false;
            return this.RollbackOnModelStateError && !filterContext.ModelState.IsValid;
        }

        private bool ShowUserError(ActionExecutedContext filterContext, Exception e)
        {
            var controller = (BaseController)filterContext.Controller;
            var error = controller.ExTranslator.Translate(e);
            if (error is IDBErrorForUser)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = controller.RcJsonError(error.Message);
                return true;
            }
            else if (e is BusinessDelegate.BizException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = controller.RcJsonError(RichClientJsonResult.Command_BizException, ((BusinessDelegate.BizException)e).ExceptionKey);
                return true;
            }
            else if (e is NHibernate.StaleObjectStateException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = controller.RcJsonError("StaleException");
                return true;
            }
            return false;
        }
    }
}