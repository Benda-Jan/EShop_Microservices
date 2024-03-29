﻿using System;
namespace Catalog.API.Services;

public interface ICacheService
{
    T? GetCacheData<T>(string key);
    void SetCachedData<T>(string key, T data, TimeSpan cacheDuration);
    void RemoveData(string key);

}

