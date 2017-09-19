﻿using Framework.Hotfix;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.EventSystems;
using WindHotfix.Core;
using Object = UnityEngine.Object;

namespace WindHotfix.GUI
{
    public class TViewController<T> : ViewController where T : class
    {
        public List<UnityObject>            Objects;

        protected bool                      mIsOpened       = false;
        protected bool                      mIsClosed       = false;

        protected List<HotfixEventObject>   mEventObjs;

        public override void Initialize(List<UnityObject> rObjs)
        {
            this.Objects = rObjs;
            
            // Binding data
            this.BindHotfixMB();
            this.OnInitialize();
        }

        public override bool IsOpened
        {
            get { return mIsOpened; }
            set { mIsOpened = value; }
        }

        public override bool IsClosed
        {
            get { return mIsClosed; }
            set { mIsClosed = value; }
        }

        public override void Opening()
        {
            this.mIsOpened = true;
            this.OnOpening();
        }

        public override void Closing()
        {
            this.mIsClosed = true;
            this.OnClosing();
        }

        public override void Closed()
        {
            this.OnClosed();
        }

        public virtual void OnInitialize()
        {
        }

        public virtual void OnOpening()
        {
        }

        public virtual void OnClosing()
        {
        }

        public virtual void OnClosed()
        {
            for (int i = 0; i < this.mEventObjs.Count; i++)
            {
                if (!this.mEventObjs[i].NeedUnbind) continue;
                HotfixEventManager.Instance.UnBinding(this.mEventObjs[i].TargetObject, this.mEventObjs[i].EventType, this.mEventObjs[i].EventHandler);
            }
            this.mEventObjs.Clear();
        }

        public UnityObject Get(string rObjName)
        {
            if (this.Objects == null) return null;
            for (int i = 0; i < this.Objects.Count; i++)
            {
                if (this.Objects[i].Name.Equals(rObjName))
                {
                    return this.Objects[i];
                }
            }
            return null;
        }

        public UnityObject Get(int nIndex)
        {
            if (this.Objects == null) return null;
            if (nIndex < 0 || nIndex >= this.Objects.Count) return null;
            return this.Objects[nIndex];
        }

        public void BindHotfixMB()
        {
            this.mEventObjs = new List<HotfixEventObject>();

            Type rType = this.GetType();
            if (rType == null) return;

            var rBindingFlags = BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public | BindingFlags.Instance;
            var rFiledInfos = rType.GetFields(rBindingFlags);
            for (int i = 0; i < rFiledInfos.Length; i++)
            {
                var rBindingAttr = rFiledInfos[i].GetCustomAttribute<HotfixBindingAttribute>();
                if (rBindingAttr != null)
                {
                    UnityObject rUnityObject = null;
                    if (!string.IsNullOrEmpty(rBindingAttr.Name))
                        rUnityObject = this.Get(rBindingAttr.Name);
                    else
                        rUnityObject = this.Get(rBindingAttr.Index);

                    // 如果属性没有，直接报错
                    if (rUnityObject == null)
                        UnityEngine.Debug.LogErrorFormat("Not find binding data, please check prefab and hofix script. {0}", rFiledInfos[i].Name);
                    else
                    {
                        if (rUnityObject.Type == rFiledInfos[i].FieldType.ToString())
                            UnityEngine.Debug.LogErrorFormat("Binding data type is not match. {0}", rFiledInfos[i].Name);
                        else
                        {
                            rFiledInfos[i].SetValue(this, rUnityObject.Object);
                        }
                    }
                }
            }

            rBindingFlags = BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance;
            var rMethodInfos = rType.GetMethods();
            for (int i = 0; i < rMethodInfos.Length; i++)
            {
                var rBindingEventAttr = rMethodInfos[i].GetCustomAttribute<HotfixBindingEventAttribute>();
                if (rBindingEventAttr != null)
                {
                    UnityObject rUnityObject = null;
                    if (!string.IsNullOrEmpty(rBindingEventAttr.Name))
                        rUnityObject = this.Get(rBindingEventAttr.Name);
                    
                    if (rUnityObject != null)
                    {                  
                        // 委托所在的对象，如果不是当前对象，要改动 
                        Delegate rDelegate = Delegate.CreateDelegate(typeof(Action<Object>), this, rMethodInfos[i]);
                        Action<Object> rActionDelegate = rDelegate as Action<Object>;
                        HotfixEventManager.Instance.Binding(rUnityObject.Object, rBindingEventAttr.EventType, rActionDelegate);
                    }
                }
            }
        }
    }
}
