using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace API.GGP.GGPInterfacesNS
{
    public interface IParser
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
        /// <exception cref="FileNotFoundException">
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// </exception>
        /// <exception cref="FormatException">
        /// </exception>
        /// <param name="filePath"></param>
        /// <param name="checkCache"></param>
        /// <returns></returns>
        void ParseFromFilePath(string filePath);

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="ArgumentException">
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="FormatException">
        /// </exception>
        /// <param name="theString"></param>
        /// <param name="checkCache"></param>
        /// <returns></returns>
        void ParseFromString(string theStringToParse);

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
        void SaveParseTrees(string filePath, string baseName, FileMode mode);

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
        void SaveCollapsedParseTrees(string filePath, string baseName, FileMode mode);

        // Async methods go here if I need them

    }
}
