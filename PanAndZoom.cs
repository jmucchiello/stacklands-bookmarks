using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BookmarksModNS
{
    public class PanAndZoom
    {
        public Key key;
        public Vector3 zoom;

        public bool IsSet { get; set; }

        public void Set()
        {
            zoom = GameCamera.instance.transform.position;
            BookmarksMod.instance.Log($"Remember ({zoom.x},{zoom.y},{zoom.z})");
            IsSet = true;
        }
        public void Jump()
        {
            if (IsSet && WorldManager.instance.CanInteract)
            {
                BookmarksMod.instance.Log($"JumpTo ({zoom.x},{zoom.y},{zoom.z})");
                Traverse.Create(GameCamera.instance).Field("cameraTargetPosition").SetValue(zoom);
            }
        }
    }

    [JsonObject(MemberSerialization.Fields)]
    public class SavePanAndZoom
    {
        public float x;
        public float y;
        public float z;
        public bool isSet;
        public SavePanAndZoom(PanAndZoom pz)
        {
            x = pz.zoom.x;
            y = pz.zoom.y; 
            z = pz.zoom.z;
            isSet = pz.IsSet;
        }

        public PanAndZoom ToPanAndZoom()
        {
            return new() { IsSet = isSet, zoom = new Vector3(x, y, z) };
        }
    }
}
