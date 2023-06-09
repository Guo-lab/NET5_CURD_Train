﻿namespace SharpArch.NHibernate
{
    using System.Collections.Generic;

    using global::NHibernate;

    public interface ISessionStorage
    {
        IEnumerable<ISession> GetAllSessions();

        ISession GetSessionForKey(string factoryKey);

        void SetSessionForKey(string factoryKey, ISession session);
        void RemoveSessionForKey(string factoryKey);

        string GetSingleActiveKey();
        IEnumerable<string> GetAllKeys();

        void BeginUsageOutofRequestContext();
    }
}