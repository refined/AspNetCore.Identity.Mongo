﻿using AspNetCore.Identity.Mongo.Entities;

namespace AspNetCore.Identity.Mongo.Repository
{
    internal static class MongoExtensions
    {
        public static string OrLocalHost(this string mongoUrl)
        {
            return mongoUrl ?? "mongodb://localhost";
        }

        public static string OrDefaultDbName<TEntity, TIdentifier>(this string dbName) where TEntity : class, IEntity<TIdentifier>
        {
            return !string.IsNullOrWhiteSpace(dbName)
                ? dbName
                : $"{typeof(TEntity).Name}_DB";
        }

        public static string OrDefaultCollectionName<TEntity, TIdentifier>(this string collectionName) where TEntity : class, IEntity<TIdentifier>
        {
            return !string.IsNullOrWhiteSpace(collectionName)
                ? collectionName
                : typeof(TEntity).Name;
        }
    }
}