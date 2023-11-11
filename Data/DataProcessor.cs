using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using Aplem.Common;
using Cysharp.Threading.Tasks;
using MessagePack;
using UnityEngine;
using ZLogger;

namespace Aplem.Data
{
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public static class AddressablesConstants
    {
        public static readonly string MasterKey = new byte[] {
            0x92, 0x62, 0xB1, 0x91,
            0xF5, 0x07, 0x46, 0xF9,
            0xF6, 0x9E, 0xB0, 0x90,
            0xA1, 0x25, 0x19, 0x76
        }.ToString();
    }

    /// <summary>
    /// ランタイムで動作するデータ処理クラス
    /// ランタイム上なので動作速度等には気を使う
    /// </summary>
    public static class DataProcessor
    {
        private static readonly int BufferKeySize = 16;
        private static readonly int BlockSize = 128;
        private static readonly int KeySize = 128;
        private static ILogger _logger = LogManager.GetLogger("DataProcessor");

        public const byte EncryptFlagBit = 1 << 0;
        public const byte CompressFlagBit = 1 << 1;


        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

#pragma warning disable CS0162
        public static async UniTask<T> RestoreData<T>(byte[] raw) where T : class
        {
            byte header = raw[0];
            byte[] data = new byte[raw.Length - 1];
            Buffer.BlockCopy(raw, 1, data, 0, raw.Length - 1);

            bool isEncrypted = (header & EncryptFlagBit) == 1;
            bool isCompressed = (header & CompressFlagBit) == 1;

            try
            {
                if (isEncrypted)
                {
                    return await RestoreEncryptedData<T>(data, isCompressed);
                }
                else if (isCompressed)
                {
                    return await RestoreCompressedData<T>(data);
                }
                else
                {
                    return await RestorePlainData<T>(data);
                }
            }
            catch (Exception e)
            {
                _logger.ZLogError("{0}", e);
                return null;
            }
        }

        public static UniTask<T> RestoreEncryptedData<T>(byte[] raw, bool useCompress) where T : class
        {
            using var memStream = new MemoryStream(raw);
            return RestoreEncryptedData<T>(memStream, useCompress);
        }

        public static async UniTask<T> RestoreEncryptedData<T>(Stream stream, bool useCompress) where T : class
        {
            RijndaelManaged rij = new RijndaelManaged
            {
                BlockSize = BlockSize,
                KeySize = KeySize,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7
            };

            // First 32 bytes are salt.
            byte[] salt = new byte[BufferKeySize];
            int nbRead = stream.Read(salt, 0, salt.Length);
            if (nbRead != salt.Length)
            {
                return null;
            }

            // Second 32 bytes are IV.
            byte[] iv = new byte[BufferKeySize];
            nbRead = stream.Read(iv, 0, BufferKeySize);
            if (nbRead != BufferKeySize)
            {
                return null;
            }
            rij.IV = iv;

            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(AddressablesConstants.MasterKey, salt);
            byte[] bufferKey = deriveBytes.GetBytes(BufferKeySize); // Convert 32 bytes of salt to password
            rij.Key = bufferKey;

            using CryptoStream cryptoStream = new CryptoStream(stream,
                                                               rij.CreateDecryptor(rij.Key, rij.IV),
                                                               CryptoStreamMode.Read);
            if (useCompress)
            {
                return await RestoreCompressedData<T>(cryptoStream);
            }
            else
            {
                return await RestorePlainData<T>(cryptoStream);
            }
        }

        /// <summary>
        /// 圧縮されたbyte配列からT型(MsgPackObj)のデータを復元
        /// </summary>
        /// <param name="raw">圧縮済みのデータ</param>
        /// <typeparam name="T">MessagePackObject</typeparam>
        public static async UniTask<T> RestoreCompressedData<T>(byte[] raw) where T : class
        {
            using var ms = new MemoryStream(raw);
            return await RestoreCompressedData<T>(ms);
        }

        /// <summary>
        /// 圧縮されたデータのStreamからT型(MsgPackObj)のデータを復元
        /// </summary>
        /// <param name="stream">圧縮済みデータのStream</param>
        /// <typeparam name="T">MessagePackObject</typeparam>
        public static async UniTask<T> RestoreCompressedData<T>(Stream stream) where T : class
        {
            using var deflateStream = new DeflateStream(stream, CompressionMode.Decompress);
            return await RestorePlainData<T>(deflateStream);
        }

        public static async UniTask<T> RestorePlainData<T>(byte[] raw) where T : class
        {
            using var ms = new MemoryStream(raw);
            return await RestorePlainData<T>(ms);
        }

        public static async UniTask<T> RestorePlainData<T>(Stream stream) where T : class
        {
            return await MessagePackSerializer.DeserializeAsync<T>(stream);
        }
#pragma warning restore CS0162
    }
}