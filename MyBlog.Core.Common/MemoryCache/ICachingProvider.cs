﻿namespace MyBlog.Core.Common.MemoryCache
{
    /// <summary>
    /// 简单的缓存接口，只有查询和添加，以后会进行扩展
    /// </summary>
    public interface ICaching
    {
        object Get(string cacheKey);

        void Set(string cacheKey, object cacheValue);
        void Remove(string cacheKey);
        bool Exists(string cacheKey);
    }
}
