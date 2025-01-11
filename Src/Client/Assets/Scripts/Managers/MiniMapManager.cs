using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    class MiniMapManager : Singleton<MiniMapManager>
    {
        public UIMiniMap minimap;
        private Collider minimapBoundingBox;
        public Collider MinimapBoundingBox
        {
            get { return minimapBoundingBox; }
        }
        public Transform PlayerTransform
        {
            get
            {
                if (User.Instance.CurrentCharaterObject == null)
                    return null;
                return User.Instance.CurrentCharaterObject.transform;
            }
        }
        public Sprite LoadCurrentMinimap()
        {
            Debug.LogFormat("UI/Minimap/" + User.Instance.CurrentMapData.MiniMap);
            return Resloader.Load<Sprite>("UI/Minimap/"+User.Instance.CurrentMapData.MiniMap);
        }
        public void UpdateMiniMap(Collider minimapBoundingBox)
        {
            this.minimapBoundingBox = minimapBoundingBox;
            if (this.minimap != null)
                this.minimap.UpdateMiniMap();
        }
    }
}
