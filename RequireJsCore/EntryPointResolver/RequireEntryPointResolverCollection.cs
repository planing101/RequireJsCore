﻿using Microsoft.AspNetCore.Mvc.Rendering;
using RequireJsCore.EntryPointResolver;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace RequireJsCore
{
    public class RequireEntryPointResolverCollection
    {
        private List<IEntryPointResolver> resolvers = new List<IEntryPointResolver>();

        private readonly ReaderWriterLockSlim accessLock = new ReaderWriterLockSlim();

        public void Clear()
        {
            accessLock.EnterWriteLock();
            try
            {
                resolvers.Clear();
            }
            finally
            {
                accessLock.ExitWriteLock();
            }
        }

        public void Prepend(IEntryPointResolver resolver)
        {
            accessLock.EnterWriteLock();
            try
            {
                resolvers.Insert(0, resolver);
            }
            finally
            {
                accessLock.ExitWriteLock();
            }
        }

        public void Add(IEntryPointResolver resolver)
        {
            accessLock.EnterWriteLock();
            try
            {
                resolvers.Add(resolver);
            }
            finally
            {
                accessLock.ExitWriteLock();
            }
        }

        internal string Resolve(ViewContext viewContext, RequireRendererConfiguration config)
        {
            accessLock.EnterReadLock();
            try
            {
                foreach (var resolver in resolvers)
                {
                    var result = resolver.Resolve(viewContext, config);
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
            finally
            {
                accessLock.ExitReadLock();
            }
        }

        ~RequireEntryPointResolverCollection()
        {
            accessLock.Dispose();
        }
    }
}
