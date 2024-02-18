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
//            //当跑起了一个异步线程，并用 await 异步等待时，通过这个 可以在主线程捕获异步线程的异常
//            //  ExceptionDispatchInfo.Capture 可以重新抛出被捕获时的调用栈(StackTrace)
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

//        // 当 await unity yield 时候，所有都在 Unity主线程里统一触发
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
//    //保存 IEnumatorAwaiter，就是一个带泛型的 Awaiter
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
//                //通过反射获取 IEnumerators 协程方法的实际名称，把它添加到异常输出
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

//            //在这里管理嵌套异常的异常捕获
//            if (topWorker.Current is IEnumerator)
//            {
//                _processStack.Push((IEnumerator)topWorker.Current);
//            }
//            else
//            {
//                //将当前值返回到unity引擎，以便它可以处理 WaitForSeconds，WaitToEndOfFrame 等
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

//            //Any() 如果源序列包含任何元素，则为 true；否则为 false。
//            if (!objTrace.Any() || objType != objTrace.Last())
//            {
//                objTrace.Add(objType);
//            }
//        }
//        objTrace.Reverse();
//        return objTrace;
//    }
//}


