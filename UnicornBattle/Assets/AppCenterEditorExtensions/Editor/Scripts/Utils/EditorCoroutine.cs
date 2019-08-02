using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

namespace AppCenterEditor
{
    public class EditorCoroutine
    {
        private const float Tick = .02f;
        private readonly IEnumerator _routine;
        private SortedList<float, IEnumerator> _shouldRunAfterTimes = new SortedList<float, IEnumerator>();
        private IEnumerable<UnityWebRequest> _www;
        private float _timeCounter = 0;

        public string Id { get; private set; }

        private EditorCoroutine(IEnumerator routine)
        {
            _routine = routine;
        }

        public static EditorCoroutine Start(IEnumerator routine)
        {
            var coroutine = new EditorCoroutine(routine)
            {
                Id = Guid.NewGuid().ToString()
            };
            coroutine.Start();
            return coroutine;
        }

        public static EditorCoroutine Start(IEnumerator routine, UnityWebRequest www)
        {
            return Start(routine, new[] { www });
        }

        public static EditorCoroutine Start(IEnumerator routine, IEnumerable<UnityWebRequest> www)
        {
            var coroutine = new EditorCoroutine(routine)
            {
                Id = Guid.NewGuid().ToString(),
                _www = www
            };
            coroutine.Start();
            return coroutine;
        }

        private void Start()
        {
            EditorApplication.update += Update;
        }

        private void Stop()
        {
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            _timeCounter += Tick;
            try
            {
                if (_www != null)
                {
                    if (_www.All(w => w.isDone) && !_routine.MoveNext())
                    {
                        Stop();
                    }
                }
                else
                {
                    var seconds = _routine.Current as EditorWaitForSeconds;
                    if (seconds != null)
                    {
                        var wait = seconds;
                        _shouldRunAfterTimes.Add(_timeCounter + wait.Seconds, _routine);
                    }
                    else if (!_routine.MoveNext())
                    {
                        Stop();
                    }
                }
                var shouldRun = _shouldRunAfterTimes;
                var index = 0;
                foreach (var runAfterSeconds in shouldRun)
                {
                    if (_timeCounter >= runAfterSeconds.Key)
                    {
                        _shouldRunAfterTimes.RemoveAt(index);
                        if (!runAfterSeconds.Value.MoveNext())
                        {
                            Stop();
                        }
                    }
                    index++;
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        public class EditorWaitForSeconds : YieldInstruction
        {
            public float Seconds { get; private set; }

            public EditorWaitForSeconds(float seconds)
            {
                Seconds = seconds;
            }
        }
    }
}
