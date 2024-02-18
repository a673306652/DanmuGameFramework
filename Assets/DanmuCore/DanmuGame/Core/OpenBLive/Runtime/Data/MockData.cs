#if UNITY_2020_3_OR_NEWER
using System.Collections.Generic;
using UnityEngine;

namespace OpenBLive.Runtime.Data
{
    [UnityEngine.CreateAssetMenu(fileName = "MockData", menuName = "Bilibili/MockData")]
    public class MockData : ScriptableObject
    {
        public List<Dm> dms;
        public List<SendGift> sendGifts;
        public List<Guard> guards;
        public List<SuperChat> superChats;
        public List<SuperChatDel> superChatDel;
    }
}
#endif