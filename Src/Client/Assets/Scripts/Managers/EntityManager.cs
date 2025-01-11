using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers
{
    interface IEntityNotify
    {
        void OnEntityRemoved();
        void OnEntityChanged(Entity eNtity);
        void OnEntityEvent(Entity eNtity);
    }
    class EntityManager : Singleton<EntityManager>
    {
        Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();
        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
        {
            this.notifiers[entityId] = notify;
        }
        public void AddEntity(Entity entity)
        {
            entities[entity.entityId] = entity;
        }
        public void OnEntityRemoved(NEntity entity)
        {
            this.entities.Remove(entity.Id);
            if (notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityRemoved();
                notifiers.Remove(entity.Id);
            }
        }

        internal void OnEntitySync(NEntitySync entity)
        {
            Entity eNtity = null;
            entities.TryGetValue(entity.Id,out eNtity);
            if (eNtity != null)
            {
                if (entity.Entity != null)
                    eNtity.EntityData = entity.Entity;
                if (notifiers.ContainsKey(entity.Id))
                {
                    notifiers[eNtity.entityId].OnEntityChanged(eNtity);
                    notifiers[eNtity.entityId].OnEntityEvent(eNtity);
                }
            }
        }
    }
}
