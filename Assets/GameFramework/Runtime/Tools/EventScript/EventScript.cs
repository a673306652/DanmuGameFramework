
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;
using UnityEngine;

namespace Daan
{
    public interface IEventScript
    {
        EventScript Initialization(string func, object[] parameter);
    }

    public class EventScript : IEventScript
    {
        public string functionName;
        public List<string> parameter;

        public virtual void SetBasicData(string func, object[] parameter)
        {
            this.functionName = func;
            this.parameter = new List<string>(parameter as string[]);
        }

        public EventScript Initialization(string func, object[] parameter)
        {
            EventScript events = new EventScript();
            events.SetBasicData(func, parameter);
            return events;
        }

        /// <summary>
        /// 解析行为脚本，格式：方法名(参数)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="script"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static List<EventScript> ParseScript(string script, char c = ',')
        {
            List<EventScript> list = new List<EventScript>();
            script = script.Replace(" ", "");
            if (script.IndexOf("(") == -1 || script.IndexOf(")") == -1)
            {
                return list;
            }
            if (script.IndexOf("{") != -1 || script.IndexOf("}") != -1)
            {
                //去掉头和尾
                script = script.Remove(script.IndexOf("{"), 1);
                script = script.Remove(script.IndexOf("}"), 1);
            }
            script = Regex.Replace(script, @"[\r\n\t]*", "");

            while (script.Length > 0)
            {
                int start = script.IndexOf("(");
                int end = script.IndexOf(")");

                int length = end - start - 1;
                string key = script.Substring(0, start);
                string parameter = script.Substring(start + 1, length);
                script = script.Remove(0, end + 1);
                EventScript scripts = new EventScript();
                list.Add(scripts.Initialization(key, parameter.Split(new char[] { c })));
            }

            return list;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string[] SplitString(string str, char c)
        {
            return str.Split(new char[] { c });
        }
    }
}