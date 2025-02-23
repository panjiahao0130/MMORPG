using Common.Data;
using Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Managers
{
    class TestManager : Singleton<TestManager>
    {

        public void Init()
        {
            NPCManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeShop, OnNpcInvokeShop);
            NPCManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeInstance, OnNpcInvokeInstance);
        }

        private bool OnNpcInvokeShop(NpcDefine npc)
        {
            Debug.LogFormat("TestManager.OnNpcInvokeShop : NPC:{0}, Name:{1}, Type: {2}, Funs:{3}", npc.ID,npc.Name,npc.Type,npc.Function);
            MessageBox.Show("点击了商店NPC:" + npc.Name, "商店NPC对话");
            //UIManager.Instance.Show<UITest>();
            return true;
        }

        private bool OnNpcInvokeInstance(NpcDefine npc)
        {
            Debug.LogFormat("TestManager.OnNpcInvokeInsrance : NPC:{0}, Name:{1}, Type: {2}, Funs:{3}", npc.ID, npc.Name, npc.Type, npc.Function);
            MessageBox.Show("点击了副本NPC:" + npc.Name, "副本NPC对话");
            return true;
        }
    }
}
