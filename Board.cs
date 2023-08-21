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

        public static Key[] keys = new Key[6] { Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9, Key.Digit0 };

        public Board()
        {
            for (int i = 0; i < marks.Length; ++i)
            {
                marks[i] = new() { key = keys[i] };
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
                marks.Add(new(board.marks[i]));
            }
        }

        public Board ToBoard()
        {
            Board board = new();
            board.Id = id;
            if (marks == null) return board;
            for (int i = 0; i < marks.Count; ++i)
            {
                board.marks[i] = marks[i].ToPanAndZoom();
            }
            return board;
        }
    }

}
