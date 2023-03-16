using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NHibernate;
using Org.BouncyCastle.Asn1.Ocsp;
using ProjectBase.Data;
using ProjectBase.Domain;
using ProjectBase.Domain.Transaction;
using ProjectBase.Web.Mvc.Angular;
using SharpArch.NHibernate;
using System;
using System.Collections.Generic;
namespace ProjectBase.Web.Mvc
{
    //I think we need to modify the sharparch transaction to commit when actionexecuted instead of resultexecuted
    public class TransactionAttribute : ActionFilterAttribute
    {
        public static readonly string RequestAttr_CommitOnErrorResult = "NetArch_CommitOnErrorResult";
        /// <summary>
        ///     Optionally holds the factory key to be used when beginning/committing a transaction
        /// </summary>
        private readonly string factoryKey;

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

        public bool RollbackOnModelStateError { get; set; }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var effectiveFactoryKey = this.GetEffectiveFactoryKey();
            var currentTransaction = NHibernateSession.CurrentFor(effectiveFactoryKey).GetCurrentTransaction();

            if (currentTransaction == null || !currentTransaction.IsActive)
            {
                ((ITransactionTaskStorage)NHibernateSession.Storage).EndTrans(effectiveFactoryKey);
                return;
            }
            if (filterContext.Exception != null || ShouldRollback(filterContext))
            {
                currentTransaction.Rollback();
                RunTasks(effectiveFactoryKey, TransactionTask.TaskRunOnEnum.OnActionFail, filterContext);
                ShowUserError(filterContext, filterContext.Exception);
            }
            else
            {
                if (filterContext.Result is RichClientJsonResult
                    && ((RichClientJsonResult)filterContext.Result).IsErrorResult
                    && filterContext.HttpContext.Items[RequestAttr_CommitOnErrorResult] == null)
                {
                    currentTransaction.Rollback();
                    RunTasks(effectiveFactoryKey, TransactionTask.TaskRunOnEnum.OnActionFail, filterContext);
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
                        RunTasks(effectiveFactoryKey, TransactionTask.TaskRunOnEnum.OnCommitFail, filterContext);
                        if (!ShowUserError(filterContext, e))
                        {
                            ((ITransactionTaskStorage)NHibernateSession.Storage).EndTrans(effectiveFactoryKey);
                            throw;
                        }
                    }
                    finally
                    {
                        currentTransaction.Dispose();
                    }
                    RunTasks(effectiveFactoryKey, TransactionTask.TaskRunOnEnum.AfterCommit, filterContext);//成功提交后执行事务关联的任务
                }
            }
            ((ITransactionTaskStorage)NHibernateSession.Storage).EndTrans(effectiveFactoryKey);
        }
        private void RunTasks(string factoryKey, TransactionTask.TaskRunOnEnum runOn, ActionExecutedContext filterContext)
        {
            var tasks = ((ITransactionTaskStorage)NHibernateSession.Storage).GetTasks(factoryKey);
            var hbTransHelper = (ITransactionHelper)filterContext.HttpContext.RequestServices.GetService(typeof(ITransactionHelper));
            hbTransHelper.RunTasks(tasks, runOn);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var effectiveFactoryKey = this.GetEffectiveFactoryKey();
            NHibernateSession.CurrentFor(effectiveFactoryKey).BeginTransaction();
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