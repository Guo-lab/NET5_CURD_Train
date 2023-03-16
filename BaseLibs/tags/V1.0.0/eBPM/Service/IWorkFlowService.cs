using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eBPM.DomainModel.WF;

namespace eBPM.Service
{
    public interface IWorkFlowService
    {
        /// <summary>
        /// 返回指定版本的WorkFlow
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        WorkFlow GetWorkFlowbyVersion(int entityId, int version);
        /// <summary>
        /// 返回最高生效版本的WorkFlow
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        WorkFlow GetLatestVersion(string entityName);

        /// <summary>
        /// 返回一个实体的所有版本的WorkFlow
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        IList<WorkFlow> GetWorkFlowList(int entityId);

        /// <summary>
        /// 新实体添加第一个版本的WorkFlow
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        WorkFlow GetFirstVersion(Entity entity);

        /// <summary>
        /// 返回当前有几个版本
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        int GetTotalVersion(int entityId);

    }
}
