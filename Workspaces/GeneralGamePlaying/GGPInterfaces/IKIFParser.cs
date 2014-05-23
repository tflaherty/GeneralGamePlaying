using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using API.GGP.PredicateLogic;

namespace API.GGP.GGPInterfacesNS
{
    public interface IKIFParser : IParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="PathTooLongException">
        /// </exception>
        /// <exception cref="DirectoryNotFoundException">
        /// </exception>        
        /// <exception cref="IOException">
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        /// <param name="filePath"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        void SaveHornClauses(string filePath, string baseName, FileMode mode);

        ConcurrentDictionary<int, HornClause> GetHornClauses();
    }
}
