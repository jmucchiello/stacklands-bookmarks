using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace BookmarksModNS
{
    public class PlayList
    {
        public List<AudioClip> Cliplist = new();

        public List<string> audioname = new();

//        public async Task<int> Start(string path)
        public int Start(string path)
        {
            int count = 0;
            if (Directory.Exists(path))
            {
                DirectoryInfo info = new DirectoryInfo(path);

                foreach (FileInfo item in info.GetFiles("*.wav"))
                {
                    audioname.Add(item.Name);
                }
                this.path = path.Replace("\\","/");
                count = LoadAudioFile();
            }
            else
            {
                BookmarksMod.instance.Log($"PlayList.Start: Invalid path: {path}");
            }
            return count;
        }

        string path;

//        async Task<int> LoadAudioFile()
        int LoadAudioFile()
        {
            BookmarksMod.instance.Log($"LoadAudioFiles count {audioname.Count}");
            int count = 0;
            for (int i = 0; i < audioname.Count; i++)
            {
                string filepath = path + string.Format("/{0}", audioname[i]);
                BookmarksMod.instance.Log("Loading " + filepath);
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(filepath, AudioType.WAV))
                {
                    var result = www.SendWebRequest();

                    while (!result.isDone) { Task.Delay(50); }

                    if (www.result == UnityWebRequest.Result.ConnectionError)
                    {
                        BookmarksMod.instance.Log(www.error);
                    }
                    else
                    {
                        AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                        clip.name = audioname[i];
                        Cliplist.Add(clip);
                        ++count;
                    }
                }
            }
            return count;
        }
    }
}