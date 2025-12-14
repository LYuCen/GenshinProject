using System.Collections;
using mModel;
using mSystem.ScopeHitSystem;
using mSystem.SoundSystem;
using UnityEngine;

namespace Firearms.ThrowWeapon
{
    public class GrenadeObj : MonoBehaviour
    {
        private float _mainRadius; // 伤害半径 (中心)
        private float _otherRadius; // 波及半径 (会递减)
        private float _boomCountDown; // 爆炸倒计时 
        private int _element; // 元素
        private int _attack; // 伤害
        private float _elementMeasure; // 元素量
        [SerializeField] private MeshRenderer meshRenderer;

        public void Init(float mainRadius, float otherRadius, float boomCountDown, int element, int attack)
        {
            _mainRadius = mainRadius;
            _otherRadius = otherRadius;
            _boomCountDown = boomCountDown;
            _element = element;
            _attack = attack;
            _elementMeasure = 2;
            meshRenderer.materials = new[] { ElementModel.Instance.materials[element]};
            StartCoroutine(Boom());
        }

        IEnumerator Boom()
        {
            yield return new WaitForSeconds(_boomCountDown);
            var scopeHitComponent = new ScopeHitComponent()
            {
                Transform = transform,
                Element = _element,
                ElementMeasure = _elementMeasure,
                Harm = _attack,
                Pos = transform.position,
                Radius = _mainRadius,
                IsHurtSelf = false
            };
            ScopeHitSystem.Instance.EnQueue(scopeHitComponent);
            GrenadeObjectPool.Instance.RemoveObj(gameObject);
            SoundSystem.Instance.PlaySound("Assets/Resources/Sound/SoundEffect/Grenade/Boom.mp3",0.8f);
        }
        
        
    }
}