using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Player.AnimPro; // 确保引用了你的逻辑节点命名空间

namespace Editor
{
    public class AnimGraphNode : Node
    {
        public NodeData Data;
        public Type LogicType;

        public AnimGraphNode(Type type, NodeData data)
        {
            LogicType = type;
            Data = data;
            title = type.Name;

            // --- 所有节点共有的：Weight Speed ---
            var wSpeedField = new FloatField("Weight Speed");
            wSpeedField.value = Data.WeightSpeed;
            wSpeedField.RegisterValueChangedCallback(evt => Data.WeightSpeed = evt.newValue);
            mainContainer.Add(wSpeedField);

            // --- 条件判断：只有叶子节点才显示动画相关参数 ---
            // 判断 logicType 是否是 AnimTreeLeafNode<PlayerManager> 的子类
            if (IsLeafNode(type))
            {
                // 1. 是否仅使用 Tag 的开关
                var toggle = new Toggle("Use Tag Only");
                toggle.value = Data.useTagOnly;
                mainContainer.Add(toggle);

                // 2. 动画拖拽框 (Default Clip)
                var clipField = new ObjectField("Default Clip") {
                    objectType = typeof(AnimationClip),
                    value = Data.DefaultClip
                };
                clipField.RegisterValueChangedCallback(evt => Data.DefaultClip = evt.newValue as AnimationClip);
                mainContainer.Add(clipField);

                // 3. 播放速度 (Anim Speed)
                var animSpeedField = new FloatField("Anim Speed");
                animSpeedField.value = Data.AnimSpeed;
                animSpeedField.RegisterValueChangedCallback(evt => Data.AnimSpeed = evt.newValue);
                mainContainer.Add(animSpeedField);

                // 4. 标签输入框 (Tag)
                var tagField = new TextField("Lookup Tag");
                tagField.value = Data.Tag;
                tagField.RegisterValueChangedCallback(evt => Data.Tag = evt.newValue);
                mainContainer.Add(tagField);

                // 初始显示逻辑
                clipField.SetEnabled(!Data.useTagOnly);
                toggle.RegisterValueChangedCallback(evt => {
                    Data.useTagOnly = evt.newValue;
                    clipField.SetEnabled(!evt.newValue);
                });
            }

            // --- 端口设置 ---
            var inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            inputPort.portName = "Parent";
            inputContainer.Add(inputPort);

            if (!IsLeafNode(type))
            {
                var outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                outputPort.portName = "Children";
                outputContainer.Add(outputPort);
            }

            RefreshExpandedState();
            RefreshPorts();
        }

        /// <summary>
        /// 判断一个类型是否为叶子节点逻辑
        /// </summary>
        private bool IsLeafNode(Type type)
        {
            // 检查该类型是否继承自 AnimTreeLeafNode<PlayerManager>
            // 因为 AnimTreeLeafNode 是泛型的，我们需要这样判断：
            Type baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(AnimTreeLeafNode<>))
                {
                    return true;
                }
                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}