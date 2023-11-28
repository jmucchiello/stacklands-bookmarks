using HarmonyLib;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using CommonModNS;

namespace BookmarksModNS
{
    public class PanAndZoom
    {
        public Key key;
        public Vector3 zoom;

        public bool IsSet { get; set; }

        public void Set()
        {
            if (I.WM.CanInteract)
            {
                zoom = GameCamera.instance.transform.position;
                IsSet = true;
                AudioManager.me.PlaySound(AudioManager.me.Click, zoom, UnityEngine.Random.Range(0.8f, 1.2f), 0.7f);
                //BookmarksMod.instance.Trace($"Remember ({zoom.x},{zoom.y},{zoom.z}) worldSize ({I.WM.DetermineTargetWorldSize(I.WM.CurrentBoard)})");
            }
        }

        private static Traverse<Vector3> cameraTargetPosition = Traverse.Create(GameCamera.instance).Field<Vector3>("cameraTargetPosition");

        public void Jump()
        {
            if (IsSet && WorldManager.instance.CanInteract)
            {
                //BookmarksMod.Log($"JumpTo ({zoom.x},{zoom.y},{zoom.z})");
                if (BookmarksMod.instance.playList.ClipList.Count() > 0)
                {
                    float distance = Vector3.Distance(GameCamera.instance.transform.position, zoom);
                    //BookmarksMod.Log($"Swoosh distance {distance}");
                    AudioManager.me.PlaySound(BookmarksMod.instance.playList.ClipList,
                        zoom, UnityEngine.Random.Range(0.8f, 1.2f), MathF.Min(distance/3.0f,0.7f));
                }
                cameraTargetPosition.Value = zoom;
            }
        }
    }

    [JsonObject(MemberSerialization.Fields)]
    public class SavePanAndZoom
    {
        public int idx;
        public float x;
        public float y;
        public float z;
        public SavePanAndZoom(int index, PanAndZoom pz)
        {
            idx = index;
            x = pz.zoom.x;
            y = pz.zoom.y; 
            z = pz.zoom.z;
        }

        public PanAndZoom ToPanAndZoom()
        {
            return new() { IsSet = true, zoom = new Vector3(x, y, z) };
        }
    }
}
