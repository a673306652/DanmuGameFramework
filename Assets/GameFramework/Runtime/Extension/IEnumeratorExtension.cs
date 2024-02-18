using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityAsyncAwaitUtil;

//public class IEnumatorAwaiter<T> : INotifyCompletion
//{
//    bool _isDone;
//    Exception _exception;
//    Action _continuation;
//    T _result;

//    public bool IsCompleted
//    {
//        get { return _isDone; }
//    }

//    public T GetResult()
//    {
//        AssertUtil.Assert(_isDone);

//        if (_exception != null)
//        {
//            //��������һ���첽�̣߳����� await �첽�ȴ�ʱ��ͨ����� ���������̲߳����첽�̵߳��쳣
//            //  ExceptionDispatchInfo.Capture ���������׳�������ʱ�ĵ���ջ(StackTrace)
//            ExceptionDispatchInfo.Capture(_exception).Throw();
//        }

//        return _result;
//    }

//    public void Complete(T result, Exception e)
//    {
//        AssertUtil.Assert(!_isDone);

//        _isDone = true;
//        _exception = e;
//        _result = result;

//        // �� await unity yield ʱ�����ж��� Unity���߳���ͳһ����
//        if (_continuation != null)
//        {
//            SyncContextUtil.RunOnUnityScheduler(_continuation);
//        }
//    }

//    void INotifyCompletion.OnCompleted(Action continuation)
//    {
//        AssertUtil.Assert(_continuation == null);
//        AssertUtil.Assert(!_isDone);

//        _continuation = continuation;
//    }
//}

//public class IEnumeratorWrapper<T>
//{
//    //���� IEnumatorAwaiter������һ�������͵� Awaiter
//    readonly IEnumatorAwaiter<T> _awaiter;
//    readonly Stack<IEnumerator> _processStack;

//    public IEnumeratorWrapper(IEnumerator coroutine, IEnumatorAwaiter<T> awaiter)
//    {
//        _processStack = new Stack<IEnumerator>();
//        _processStack.Push(coroutine);
//        _awaiter = awaiter;
//    }

//    public IEnumerator Run()
//    {
//        while (true)
//        {
//            IEnumerator topWorker = _processStack.Peek();

//            bool isDone;
//            try
//            {
//                isDone = !topWorker.MoveNext();
//            }
//            catch (Exception e)
//            {
//                //ͨ�������ȡ IEnumerators Э�̷�����ʵ�����ƣ�������ӵ��쳣���
//                var objectTrace = GenerateObjectTrace(_processStack);
//                if (objectTrace.Any())
//                {
//                    _awaiter.Complete(default(T), new Exception(GenerateObjectTraceMessage(objectTrace), e));
//                }
//                else
//                {
//                    _awaiter.Complete(default(T), e);
//                }
//                yield break;
//            }

//            if (isDone)
//            {
//                _processStack.Pop();

//                if (_processStack.Count == 0)
//                {
//                    _awaiter.Complete((T)topWorker.Current, null);
//                    yield break;
//                }
//            }

//            //���������Ƕ���쳣���쳣����
//            if (topWorker.Current is IEnumerator)
//            {
//                _processStack.Push((IEnumerator)topWorker.Current);
//            }
//            else
//            {
//                //����ǰֵ���ص�unity���棬�Ա������Դ��� WaitForSeconds��WaitToEndOfFrame ��
//                yield return topWorker.Current;
//            }
//        }
//    }

//    string GenerateObjectTraceMessage(List<Type> objTrace)
//    {
//        var result = new StringBuilder();
//        foreach (var objType in objTrace)
//        {
//            if (result.Length != 0)
//            {
//                result.Append(" -> ");
//            }
//            result.Append(objType.ToString());
//        }
//        result.AppendLine();
//        return "Unity Coroutine Object Trace: " + result.ToString();
//    }

//    static List<Type> GenerateObjectTrace(IEnumerable<IEnumerator> enumerators)
//    {
//        var objTrace = new List<Type>();
//        foreach (var enumerator in enumerators)
//        {
//            var field = enumerator.GetType().GetField("$this", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
//            if (field == null)
//            {
//                continue;
//            }
//            var obj = field.GetValue(enumerator);
//            if (obj == null)
//            {
//                continue;
//            }
//            var objType = obj.GetType();

//            //Any() ���Դ���а����κ�Ԫ�أ���Ϊ true������Ϊ false��
//            if (!objTrace.Any() || objType != objTrace.Last())
//            {
//                objTrace.Add(objType);
//            }
//        }
//        objTrace.Reverse();
//        return objTrace;
//    }
//}


