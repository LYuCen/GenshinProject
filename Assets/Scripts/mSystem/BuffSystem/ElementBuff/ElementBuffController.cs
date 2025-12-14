using System;
using mSystem.BuffSystem.ElementBuff.Event;
using mSystem.HurtSystem;
using mSystem.ScopeHitSystem;
using QFramework;
using UnityEngine;

namespace mSystem.BuffSystem.ElementBuff
{
    public class ElementBuffController : MonoBehaviour, ICanRegisterEvent
    {
        public float[] elements = new float[8];
        public GameObject[] elementUI = new GameObject[8];
        public float time;
        private Action<ElectrificationReaction> _electrificationHandler;
        
        public bool electStart; // 是否触发感电反应
        public float electCd; // 感电触发 cd
        public float electScopeCd; // 感电周围触发 cd
        public int electStep; // 感电反应步数（第二步触发集体感电）
        private void Awake()
        {
            time = Time.time + 1f;

            // 保存回调委托
            _electrificationHandler = e =>
            {
                if (e.Transform == transform && !electStart)
                {
                    electStart = true;
                    electStep = 0;
                }
            };

            this.RegisterEvent(_electrificationHandler);
        }

        public bool Decrease()
        {
            if (time > Time.time) return true;
            bool res = false;
            for (int i = 0; i < 8; i++)
            {
                if (elementUI[i] && elements[i] == 0)
                {
                    ElementUIObjectPool.Instance.RemoveObj(elementUI[i]);
                    elementUI[i] = null;
                    continue;
                }

                if (elements[i] < 0)
                {
                    res = true;
                    continue;
                }
                elements[i] = Mathf.Max(elements[i] - 0.16f, 0);
                res = res || elements[i] > 0;
            }

            time = Time.time + 1f;
            return res;
        }

        public void AddElement(int elementType, float elementMeasure, HurtComponent component)
        {
            elements[elementType] = Mathf.Min(elementMeasure + elements[elementType], 1.8f);
            if (!elementUI[elementType])
            {
                var elementComponent = new ElementComponent
                {
                    Transform = component.Transform,
                    Controller = this,
                    Element = component.Element,
                    IsStart = true
                };
                ElementSystem.Instance.EnQueue(elementComponent);
            }
        }

        private void OnDisable()
        {
            if (ElementUIObjectPool.Instance == null) return;
            this.UnRegisterEvent(_electrificationHandler); // 结束监听
            for (int i = 0; i < 8; i++)
            {
                if (elementUI[i])
                {
                    ElementUIObjectPool.Instance.RemoveObj(elementUI[i]);
                    elementUI[i] = null;
                }
            }
        }

        public void ElectrificationReaction()
        {
            if (elements[2] != 0 && elements[4] != 0)
            {
                if (electCd > 0) return;
                electCd = 0.5f;
                if(elements[2] > 0) elements[2] = Mathf.Max(0, elements[2] - 0.4f);
                if(elements[4] > 0) elements[4] = Mathf.Max(0, elements[4] - 0.4f);
                electStep++;
                var hurtComponent = new HurtComponent()
                {
                    Element = 4,
                    ElementMeasure = 0,
                    Harm = 20,
                    Transform = transform,
                    HitType = 0
                };
                HurtSystem.HurtSystem.Instance.EnQueue(hurtComponent);
                if (electStep > 1)
                {
                    var component = new ScopeHitComponent()
                    {
                        Element = 4,
                        ElementMeasure = 0,
                        Harm = 20,
                        IsHurtSelf = false,
                        Pos = transform.position + Vector3.down * -0.5f,
                        Radius = 2,
                        Transform = transform,
                        ScopeType = 1
                    };
                    ScopeHitSystem.ScopeHitSystem.Instance.EnQueue(component);
                }
            }
            else electStart = false;
        }

        public IArchitecture GetArchitecture()
        {
            return GenshinArchitecture.Interface;
        }
    }
}