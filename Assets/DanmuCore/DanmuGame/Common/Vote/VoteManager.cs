using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;
using System;

namespace DanMuGame
{
    public class VoteManager : Manager<VoteManager>
    {
        [System.Serializable]
        public class Option
        {
            public string content;
            public List<string> votedUserList = new List<string>();
            public int count;
        }
        [System.Serializable]
        public class Data
        {
            public bool active = true;
            public List<Option> options = new List<Option>();
            public List<string> votedUserList = new List<string>();
            public Action<Data, string, string> onVote;
        }

        public List<Data> voteList = new List<Data>();

        public Data CreateVote(Action<Data, string, string> action = null, params string[] options)
        {
            var vote = new Data();
            {
                vote.options = new List<Option>();
                vote.onVote = action;
                foreach (var item in options)
                {
                    vote.options.Add(new Option()
                    {
                        content = item,
                        votedUserList = new List<string>(),
                    });
                }
            }
            this.voteList.Add(vote);
            return vote;
        }

        public bool Vote(string id, string content)
        {
            var user = UserManager.Instance.GetUser(id);
            if (user == null) return false;
            Data data = null;
            Option option = null;
            foreach (var item in this.voteList)
            {
                if (!item.active) continue;
                if (item.votedUserList.Contains(id)) return false;
                foreach (var item2 in item.options)
                {
                    if (item2.content.Equals(content))
                    {
                        data = item;
                        option = item2;
                        break;
                    }
                }
            }

            if (option != null && !data.votedUserList.Contains(id))
            {
                data.votedUserList.Add(id);
                option.votedUserList.Add(id);
                option.count = option.votedUserList.Count;
                data.onVote?.Invoke(data, id, content);
                return true;
            }
            return false;
        }

    }
}