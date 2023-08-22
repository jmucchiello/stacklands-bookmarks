using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem;

namespace BookmarksModNS
{
    public class Board
    {
        public string Id;
        public PanAndZoom[] marks = new PanAndZoom[6];

        private static Key[] keys = new Key[6] { Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9, Key.Digit0 };

        public Board()
        {
            for (int i = 0; i < marks.Length; ++i)
            {
                marks[i] = new() { key = keys[i] };
            }
            UpdateKeys();
        }

        public void UpdateKeys()
        {
            for (int i = 0; i < marks.Length; ++i)
            {
                if (marks[i] is null)
                {
                    marks[i] = new();
                }
                marks[i].key = keys[i];
            }
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class SaveBoard
    {
        [JsonProperty]
        public string id;
        [JsonProperty]
        public List<SavePanAndZoom> marks = new();

        public SaveBoard()
        { }

        public SaveBoard(Board board)
        {
            id = board.Id;
            if (board.marks == null) return;

            for (int i = 0; i < board.marks.Length; ++i)
            {
                if (board.marks[i].IsSet)
                {
                    SavePanAndZoom pz = new(i, board.marks[i]);
                    marks.Add(pz);
                }
            }
        }

        public Board ToBoard()
        {
            Board board = new();
            board.Id = id;
            if (marks == null) return board;
            foreach (SavePanAndZoom pz in marks)
            {
                board.marks[pz.idx] = pz.ToPanAndZoom();
            }
            board.UpdateKeys();
            return board;
        }
    }

}
