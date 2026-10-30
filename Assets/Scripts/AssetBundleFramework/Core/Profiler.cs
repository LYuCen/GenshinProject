using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace AssetBundleFramework.Core
{
    public class Profiler
    {
        private static readonly Stopwatch ms_stopwatch = Stopwatch.StartNew();
        private static readonly StringBuilder ms_stringBuilder = new StringBuilder();
        private static readonly List<Profiler> ms_stack = new List<Profiler>();
        
        private List<Profiler> m_children;
        private string m_name; // 名称
        private int m_level; // 层级
        private long m_timestamp; // 时间戳
        private long m_time; // 时间
        private int m_count;
        
        public Profiler(string name)
        {
            m_children = null;
            m_name = name;
            m_level = 0;
            m_timestamp = -1;
            m_time = 0;
            m_count = 0;
        }

        private Profiler(string name, int level) : this(name)
        {
            m_level = level;
        }

        /// <summary>
        /// 创建子分析器
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>返回 Profiler</returns>
        public Profiler CreateChild(string name)
        {
            if (m_children == null)
            {
                m_children = new List<Profiler>();
            }

            Profiler profiler = new Profiler(name, m_level + 1);
            m_children.Add(profiler);
            return profiler;
        }

        public void Start()
        {
            if (m_timestamp != -1)
            {
                throw new Exception($"{nameof(Profiler)} {nameof(Start)} error, repeat start, name:{m_name}");
            }

            m_timestamp = ms_stopwatch.ElapsedTicks;
        }
        
        public void Stop()
        {
            if (m_timestamp == -1)
            {
                throw new Exception($"{nameof(Profiler)} {nameof(Start)} error, repeat start, name:{m_name}");
            }

            m_time += ms_stopwatch.ElapsedTicks - m_timestamp; // 计算耗时
            m_count += 1; // 计数器加一
            m_timestamp = -1;
        }

        private void Format()
        {
            ms_stringBuilder.AppendLine();

            for (int i = 0; i < ms_stack.Count; ++i)
            {
                ms_stringBuilder.Append(i < m_level - 1 ? "|  " : "|--");
            }

            ms_stringBuilder.Append(m_name);

            if (m_count <= 0) return;

            ms_stringBuilder.Append(" [");
            ms_stringBuilder.Append("Count");
            ms_stringBuilder.Append(": ");
            ms_stringBuilder.Append(m_count);
            ms_stringBuilder.Append(", ");
            ms_stringBuilder.Append("Time");
            ms_stringBuilder.Append(": ");

            ms_stringBuilder.Append($"{(float)m_time / TimeSpan.TicksPerMillisecond:F2}");
            ms_stringBuilder.Append("毫秒     ");
            
            ms_stringBuilder.Append($"{(float)m_time / TimeSpan.TicksPerSecond:F2}");
            ms_stringBuilder.Append("秒     ");
            
            ms_stringBuilder.Append($"{(float)m_time / TimeSpan.TicksPerMinute:F2}");
            ms_stringBuilder.Append("分     ");
            
            ms_stringBuilder.Append("]");
        }
        
        public override string ToString()
        {
            ms_stringBuilder.Clear();
            ms_stack.Clear();
            
            ms_stack.Add(this);

            while (ms_stack.Count > 0)
            {
                int index = ms_stack.Count - 1;
                Profiler profiler = ms_stack[index];
                ms_stack.RemoveAt(index);
                
                profiler.Format();

                List<Profiler> children = profiler.m_children;
                if (children == null) continue;

                for (int i = children.Count - 1; i >= 0; --i)
                {
                    ms_stack.Add(children[i]);
                }
            }

            return ms_stringBuilder.ToString();
        }
    }
}