using System;
using System.Collections.Generic;
using System.Linq;
using ProjectBase.Domain;
using eBPM.DomainModel.WF;

namespace eBPM.Service
{
    public class WorkFlowService:IWorkFlowService
    {
        public  IGenericDaoWithTypedId<Entity, int> EntityDao { get; set; }
        public IGenericDaoWithTypedId<WorkFlow, int> WorkFlowDao {get; set;}
        public WorkFlowService()
        {
            
        }
        public WorkFlow GetWorkFlowbyVersion(int entityId, int version)
        {
            return WorkFlowDao.GetOneByQuery(x => x.Entity.Id == entityId && x.Version == version);
        }
        public WorkFlow GetLatestVersion(string entityName)
        {
            Entity entity = EntityDao.GetOneByQuery(x => x.EntityName == entityName);
            if (entity != null && entity.LatestVersion.HasValue)
            {
                return GetWorkFlowbyVersion(entity.Id, entity.LatestVersion.Value);
            }
            else { return null; }                
        }

        public IList<WorkFlow> GetWorkFlowList(int entityId)
        {
            return WorkFlowDao.GetByQuery(x => x.Entity.Id == entityId).OrderBy(s => s.Version).ToList();
        }

        public WorkFlow GetFirstVersion(Entity entity) {

            WorkFlow wf = new WorkFlow();
            wf.Entity = entity;
            wf.Version = 1;
            wf.IsDraft = true;
            wf.ChangedContent = "";
            wf.LastModified = DateTime.Now;

            wf.Steps = new List<WorkStep>();
            WorkStep ws = new WorkStep();
            ws.NewWorkStep(wf, 1);
            ws.Name = "Request/Update";
            ws.StepCategory = eBPM.DomainModel.StepCategoryEnum.Start;
            ws.MappedStatus = "Rejected";

            wf.Steps.Add(ws);
            return wf;
        }

        public int GetTotalVersion(int entityId) {
            return WorkFlowDao.GetCountByQuery(x => x.Entity.Id == entityId);
        }
    }
}
