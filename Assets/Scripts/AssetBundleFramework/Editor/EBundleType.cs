namespace AssetBundleFramework.Editor
{
    /// <summary>
    /// 控制 ab 粒度
    /// </summary>
    public enum EBundleType
    {
        /// <summary>
        /// 以文件作为 ab 名字（最小粒度）
        /// </summary>
        File,
        
        /// <summary>
        /// 以目录作为 ab 名字
        /// </summary>
        Directory,
        
        /// <summary>
        /// 以上所有
        /// </summary>
        All
    }
}