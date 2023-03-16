﻿namespace SharpArch.Domain
{
    using System;
    using System.IO;
    using Newtonsoft.Json;

    public static class FileCache
    {
        /// <summary>
        ///     Deserializes a data file into an object of type {T}.
        /// </summary>
        /// <typeparam name = "T">Type of object to deseralize and return.</typeparam>
        /// <param name = "path">Full path to file containing seralized data.</param>
        /// <returns>If object is successfully deseralized, the object of type {T}, 
        ///     otherwise null.</returns>
        /// <exception cref = "ArgumentNullException">Thrown if the path parameter is null or empty.</exception>
        public static T RetrieveFromCache<T>(string path) where T : class
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            try
            {
                string s = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(s);
            }
            catch
            {
                // Return null if the object can't be deseralized
                return null;
            }
        }

        /// <summary>
        ///     Serialize the given object of type {T} to a file at the given path.
        /// </summary>
        /// <typeparam name = "T">Type of object to serialize.</typeparam>
        /// <param name = "obj">Object to serialize and store in a file.</param>
        /// <param name = "path">Full path of file to store the serialized data.</param>
        /// <exception cref = "ArgumentNullException">Thrown if obj or path parameters are null.</exception>
        public static void StoreInCache<T>(T obj, string path) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }
            File.WriteAllText(path,JsonConvert.SerializeObject(obj));
        }
    }
}