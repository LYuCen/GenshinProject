using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AssetBundleFramework.Editor
{
    public class BuildItem
    {
        [DisplayName("资源路径")]
        [XmlAttribute("AssetPath")]
        public string AssetPath { get; set; }

        [DisplayName("资源类型")]
        [XmlAttribute("ResourceType")]
        public EResourceType ResourceType { get; set; } = EResourceType.Direct;
        
        [DisplayName("ab粒度")]
        [XmlAttribute("BundleType")]
        public EBundleType BundleType { get; set; } = EBundleType.File;

        [DisplayName("资源后缀")]
        [XmlAttribute("Suffix")]
        public string Suffix { get; set; } = ".prefab";

        [XmlIgnore] 
        public List<string> IgnorePaths { get; set; } = new List<string>();
        
        [XmlIgnore] 
        public List<string> Suffixes { get; set; } = new List<string>();
        
        /// <summary>
        /// 匹配打包的个数
        /// </summary>
        [XmlIgnore] public int Count { get; set; }
    }
}