using mSystem.InputSystem;
using mSystem.InputSystem.Context;
using UnityEditor;
using UnityEngine;

namespace Editor.InputEditor
{
    public class InputDebugWindow : EditorWindow
    {
        private InputContextHolder _holder;

        [MenuItem("Tools/Input Debug")]
        public static void Open()
        {
            GetWindow<InputDebugWindow>("Input Debug");
        }

        private void OnGUI()
        {
            GUILayout.Space(8);

            _holder = EditorGUILayout.ObjectField(
                "Input Holder",
                _holder,
                typeof(InputContextHolder),
                true
            ) as InputContextHolder;

            GUILayout.Space(8);

            if (!_holder)
            {
                EditorGUILayout.HelpBox(
                    "请选择 InputContextHolder",
                    MessageType.Info);

                return;
            }

            if (_holder.Context == null)
            {
                EditorGUILayout.HelpBox(
                    "Context 为空",
                    MessageType.Warning);

                return;
            }

            DrawContext(_holder.Context);

            Repaint();
        }

        private void DrawContext(InputContext ctx)
        {
            DrawMove(ctx.Move);

            DrawButton("PrimaryAction", ctx.PrimaryAction);

            DrawButton("SecondaryAction", ctx.SecondaryAction);

            DrawButton("GetPrimaryWeapon", ctx.GetPrimaryWeapon);

            DrawButton("GetSecondaryWeapon", ctx.GetSecondaryWeapon);

            DrawButton("Reload", ctx.Reload);

            DrawButton("Sprint", ctx.Sprint);

            DrawButton("Crouch", ctx.Crouch);

            DrawButton("Ability1", ctx.Ability1);

            DrawScroll(ctx.MouseScroll);
        }

        /// <summary>
        /// 绘制移动输入
        /// </summary>
        private void DrawMove(Vector2 move)
        {
            if (move.sqrMagnitude < 0.001f)
                return;

            GUILayout.BeginHorizontal("box");

            GUILayout.Label(
                "Move",
                GUILayout.Width(140));

            GUILayout.Label(
                $"({move.x:F2}, {move.y:F2})");

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制鼠标滚轮
        /// </summary>
        private void DrawScroll(float scroll)
        {
            if (Mathf.Abs(scroll) < 0.01f)
                return;

            GUILayout.BeginHorizontal("box");

            GUILayout.Label(
                "MouseScroll",
                GUILayout.Width(140));

            GUILayout.Label(scroll.ToString("F2"));

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制按钮状态
        /// </summary>
        private void DrawButton(
            string label,
            ButtonState state)
        {
            if (!state.IsActive())
                return;

            GUILayout.BeginHorizontal("box");

            GUILayout.Label(
                label,
                GUILayout.Width(140));

            DrawMiniState("D", state.Down, Color.green);

            DrawMiniState("H", state.Press, Color.yellow);

            DrawMiniState("U", state.Up, Color.red);

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制小状态标记
        /// </summary>
        private void DrawMiniState(
            string text,
            bool active,
            Color color)
        {
            Color old = GUI.color;

            GUI.color = active
                ? color
                : new Color(0.35f, 0.35f, 0.35f);

            GUILayout.Label(
                text,
                GUILayout.Width(20));

            GUI.color = old;
        }
    }
}