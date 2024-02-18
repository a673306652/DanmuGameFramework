using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Daan;

namespace Daan
{
    public class MessageManager : Manager<MessageManager>
    {
        private void Update()
        {
            Message.UpdateAll();
        }
    }
}
