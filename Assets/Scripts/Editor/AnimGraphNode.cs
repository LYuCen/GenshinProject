using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Player.AnimPro;
using UnityEditor;

namespace Editor
{
    public class AnimGraphNode : Node
    {
        public NodeData Data;
        public Type LogicType;
        private AnimGraphData _owner; // 增加对资源的引用

        public AnimGraphNode(Type type, NodeData data, AnimGraphData owner)
        {
            LogicType = type;
            Data = data;
            _owner = owner; // 记录所属资源，用于 Undo
            title = type.Name;

            // --- Weight Speed ---
            var wSpeedField = new FloatField("Weight Speed");
            wSpeedField.value = Data.WeightSpeed;
            wSpeedField.RegisterValueChangedCallback(evt => {
                Undo.RecordObject(_owner, "Change Weight Speed"); // 使用 _owner
                Data.WeightSpeed = evt.newValue;
            });
            mainContainer.Add(wSpeedField);

            if (IsLeafNode(type))
            {
                var toggle = new Toggle("Use Tag Only");
                toggle.value = Data.useTagOnly;
                toggle.RegisterValueChangedCallback(evt => {
                    Undo.RecordObject(_owner, "Toggle Use Tag");
                    Data.useTagOnly = evt.newValue;
                });
                mainContainer.Add(toggle);

                var clipField = new ObjectField("Default Clip") {
                    objectType = typeof(AnimationClip),
                    value = Data.DefaultClip
                };
                clipField.RegisterValueChangedCallback(evt => {
                    Undo.RecordObject(_owner, "Change Clip");
                    Data.DefaultClip = evt.newValue as AnimationClip;
                });
                mainContainer.Add(clipField);

                var animSpeedField = new FloatField("Anim Speed");
                animSpeedField.value = Data.AnimSpeed;
                animSpeedField.RegisterValueChangedCallback(evt => {
                    Undo.RecordObject(_owner, "Change Anim Speed"); // 修正变量名
                    Data.AnimSpeed = evt.newValue;
                });
                mainContainer.Add(animSpeedField);

                clipField.SetEnabled(!Data.useTagOnly);
                toggle.RegisterValueChangedCallback(evt => clipField.SetEnabled(!evt.newValue));
            }
            
            // --- Tag ---
            var tagField = new TextField("Lookup Tag");
            tagField.value = Data.Tag;
            tagField.RegisterValueChangedCallback(evt => {
                Undo.RecordObject(_owner, "Change Tag");
                Data.Tag = evt.newValue;
            });
            mainContainer.Add(tagField);
            
            // --- Mask ---
            var maskField = new ObjectField("Avatar Mask") {
                objectType = typeof(AvatarMask),
                value = Data.Mask
            };
            maskField.RegisterValueChangedCallback(evt => {
                Undo.RecordObject(_owner, "Change Mask");
                Data.Mask = evt.newValue as AvatarMask;
            });
            mainContainer.Add(maskField);

            // 端口设置保持不变...
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

        private bool IsLeafNode(Type type)
        {
            Type baseType = type;
            while (baseType != null && baseType != typeof(object))
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(AnimTreeLeafNode<>))
                    return true;
                baseType = baseType.BaseType;
            }
            return false;
        }
    }
}