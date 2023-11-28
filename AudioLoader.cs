using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace BookmarksModNS
{
    public class PlayList
    {
        public List<AudioClip> ClipList = new();
        public List<string> NameList = new();

        public int Start(string path)
        {
            if (Directory.Exists(path))
            {
                DirectoryInfo info = new DirectoryInfo(path);
                List<string> filelist = new List<string>();
                foreach (FileInfo item in info.GetFiles("*.wav"))
                {
                    filelist.Add(item.Name);
                }
                path = path.Replace("\\","/");
                if (!path.EndsWith("/")) path += "/";
                ClipList = LoadAudioFile(path, filelist);
                NameList = filelist;
            }
            else
            {
                BookmarksMod.Log($"PlayList.Start: Invalid path: {path}");
            }
            return ClipList.Count;
        }

        List<AudioClip> LoadAudioFile(string path, List<string> filelist)
        {
            BookmarksMod.Log($"LoadAudioFiles count {filelist.Count}");
            List<AudioClip> clips = new List<AudioClip>();
            foreach (string file in filelist)
            {
                string filepath = path + string.Format("{0}", file);
                BookmarksMod.Log("Loading " + filepath);
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filepath, AudioType.WAV))
                {
                    var result = www.SendWebRequest();
                    while (!result.isDone) { Task.Delay(50); }

                    if (www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        BookmarksMod.Log(www.error);
                        filelist.Remove(file);
                    }
                    else
                    {
                        try
                        {
                            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                            clip.name = file;
                            clips.Add(clip);
                        }
                        catch (Exception e)
                        {
                            filelist.Remove(file);
                            BookmarksMod.Log($"Exception {e.Message} while load {file}");
                            BookmarksMod.Log(e.StackTrace);
                        }
                    }
                }
            }
            return clips;
        }
    }
}