﻿//-----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Isam.Esent.Interop
{
    /// <summary>
    /// Native interop for functions in esent.dll.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        #region Configuration Constants

        /// <summary>
        /// The name of the DLL that the methods should be loaded from.
        /// </summary>
        private const string EsentDll = "esent.dll";

        /// <summary>
        /// The CharSet for the methods in the DLL.
        /// </summary>
        private const CharSet EsentCharSet = CharSet.Ansi;

        /// <summary>
        /// Initializes static members of the NativeMethods class.
        /// </summary>
        static NativeMethods()
        {
            // This must be changed when the CharSet is changed.
            NativeMethods.Encoding = Encoding.ASCII;
        }

        /// <summary>
        /// Gets encoding to be used when converting data to/from byte arrays.
        /// This should match the CharSet above.
        /// </summary>
        public static Encoding Encoding { get; private set; }

        #endregion Configuration Constants

        #region init/term

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetCreateInstance(out IntPtr instance, string szInstanceName);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetCreateInstance2(out IntPtr instance, string szInstanceName, string szDisplayName, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetInit(ref IntPtr instance);

        [DllImport(EsentDll)]
        public static extern int JetInit2(ref IntPtr instance, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetTerm(IntPtr instance);

        [DllImport(EsentDll)]
        public static extern int JetTerm2(IntPtr instance, uint grbit);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetSetSystemParameter(IntPtr pinstance, IntPtr sesid, uint paramid, IntPtr lParam, string szParam);

        // The param is ref because it is an 'in' parameter when getting error text
        [DllImport(EsentDll)]
        public static extern int JetGetSystemParameter(IntPtr instance, IntPtr sesid, uint paramid, ref IntPtr plParam, StringBuilder szParam, uint cbMax);

        [DllImport(EsentDll)]
        public static extern int JetGetVersion(IntPtr sesid, out uint dwVersion);

        #endregion

        #region Databases

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetCreateDatabase(IntPtr sesid, string szFilename, string szConnect, out uint dbid, uint grbit);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetAttachDatabase(IntPtr sesid, string szFilename, uint grbit);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetDetachDatabase(IntPtr sesid, string szFilename);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetOpenDatabase(IntPtr sesid, string database, string szConnect, out uint dbid, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetCloseDatabase(IntPtr sesid, uint dbid, uint grbit);

        #endregion

        #region sessions

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetBeginSession(IntPtr instance, out IntPtr session, string username, string password);

        [DllImport(EsentDll)]
        public static extern int JetSetSessionContext(IntPtr session, IntPtr context);

        [DllImport(EsentDll)]
        public static extern int JetResetSessionContext(IntPtr session);

        [DllImport(EsentDll)]
        public static extern int JetEndSession(IntPtr sesid, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetDupSession(IntPtr sesid, out IntPtr newSesid);

        #endregion

        #region tables

        [DllImport(EsentDll)]
        public static extern int JetOpenTable(IntPtr sesid, uint dbid, string tablename, IntPtr pvParameters, uint cbParameters, uint grbit, out IntPtr tableid);

        [DllImport(EsentDll)]
        public static extern int JetCloseTable(IntPtr sesid, IntPtr tableid);

        [DllImport(EsentDll)]
        public static extern int JetDupCursor(IntPtr sesid, IntPtr tableid, out IntPtr tableidNew, uint grbit);

        #endregion

        #region transactions

        [DllImport(EsentDll)]
        public static extern int JetBeginTransaction(IntPtr sesid);

        [DllImport(EsentDll)]
        public static extern int JetCommitTransaction(IntPtr sesid, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetRollback(IntPtr sesid, uint grbit);

        #endregion

        #region DDL

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetCreateTable(IntPtr sesid, uint dbid, string szTableName, int pages, int density, out IntPtr tableid);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetAddColumn(IntPtr sesid, IntPtr tableid, string szColumnName, ref NATIVE_COLUMNDEF columndef, byte[] pvDefault, uint cbDefault, out uint columnid);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetDeleteColumn(IntPtr sesid, IntPtr tableid, string szColumnName);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetDeleteIndex(IntPtr sesid, IntPtr tableid, string szIndexName);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetDeleteTable(IntPtr sesid, uint dbid, string szTableName);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetCreateIndex(IntPtr sesid, IntPtr tableid, string szIndexName, uint grbit, string szKey, uint cbKey, uint lDensity);
        
        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetGetTableColumnInfo(IntPtr sesid, IntPtr tableid, string szColumnName, ref NATIVE_COLUMNDEF columndef, uint cbMax, uint InfoLevel);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetGetTableColumnInfo(IntPtr sesid, IntPtr tableid, ref uint pcolumnid, ref NATIVE_COLUMNDEF columndef, uint cbMax, uint InfoLevel);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetGetTableColumnInfo(IntPtr sesid, IntPtr tableid, string szIgnored, ref NATIVE_COLUMNLIST columnlist, uint cbMax, uint InfoLevel);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetGetColumnInfo(IntPtr sesid, uint dbid, string szTableName, string szColumnName, ref NATIVE_COLUMNDEF columndef, uint cbMax, uint InfoLevel);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetGetColumnInfo(IntPtr sesid, uint dbid, string szTableName, string szColumnName, ref NATIVE_COLUMNLIST columnlist, uint cbMax, uint InfoLevel);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetGetObjectInfo(IntPtr sesid, uint dbid, uint objtyp, string szContainerName, string szObjectName, ref NATIVE_OBJECTLIST objectlist, uint cbMax, uint InfoLevel);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetGetCurrentIndex(IntPtr sesid, IntPtr tableid, StringBuilder szIndexName, uint cchIndexName);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetGetIndexInfo(IntPtr sesid, uint dbid, string szTableName, string szIndexName, ref NATIVE_INDEXLIST indexlist, uint cbResult, uint InfoLevel);

        [DllImport(EsentDll, CharSet = EsentCharSet)]
        public static extern int JetGetTableIndexInfo(IntPtr sesid, IntPtr tableid, string szIndexName, ref NATIVE_INDEXLIST indexlist, uint cbResult, uint InfoLevel);

        #endregion

        #region Navigation

        [DllImport(EsentDll)]
        public static extern int JetGetBookmark(IntPtr sesid, IntPtr tableid, byte[] pvBookmark, uint cbMax, out uint cbActual);

        [DllImport(EsentDll)]
        public static extern int JetGotoBookmark(IntPtr sesid, IntPtr tableid, byte[] pvBookmark, uint cbBookmark);

        // This doesn't take a ref NATIVE_RETINFO because the parameter can be null
        [DllImport(EsentDll)]
        public static extern int JetRetrieveColumn(IntPtr sesid, IntPtr tableid, uint columnid, IntPtr pvData, uint cbData, out uint cbActual, uint grbit, IntPtr pretinfo);

        [DllImport(EsentDll)]
        public static extern int JetMove(IntPtr sesid, IntPtr tableid, int cRow, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetMakeKey(IntPtr sesid, IntPtr tableid, IntPtr pvData, uint cbData, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetRetrieveKey(IntPtr sesid, IntPtr tableid, byte[] pvData, uint cbMax, out uint cbActual, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetSeek(IntPtr sesid, IntPtr tableid, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetSetIndexRange(IntPtr sesid, IntPtr tableid, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetIntersectIndexes(IntPtr sesid, NATIVE_INDEXRANGE[] rgindexrange, uint cindexrange, ref NATIVE_RECORDLIST recordlist, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetSetCurrentIndex(IntPtr sesid, IntPtr tableid, string szIndexName);

        [DllImport(EsentDll)]
        public static extern int JetIndexRecordCount(IntPtr sesid, IntPtr tableid, out uint crec, uint crecMax);

        [DllImport(EsentDll)]
        public static extern int JetSetTableSequential(IntPtr sesid, IntPtr tableid, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetResetTableSequential(IntPtr sesid, IntPtr tableid, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetGetRecordPosition(IntPtr sesid, IntPtr tableid, out NATIVE_RECPOS precpos, uint cbRecpos);

        [DllImport(EsentDll)]
        public static extern int JetGotoPosition(IntPtr sesid, IntPtr tableid, ref NATIVE_RECPOS precpos);

        #endregion

        #region DML

        [DllImport(EsentDll)]
        public static extern int JetDelete(IntPtr sesid, IntPtr tableid);

        [DllImport(EsentDll)]
        public static extern int JetPrepareUpdate(IntPtr sesid, IntPtr tableid, uint prep);

        [DllImport(EsentDll)]
        public static extern int JetUpdate(IntPtr sesid, IntPtr tableid, byte[] pvBookmark, uint cbBookmark, out uint cbActual);

        // Doesn't take a ref NATIVE_SETINFO because the parameter can be null
        [DllImport(EsentDll)]
        public static extern int JetSetColumn(IntPtr sesid, IntPtr tableid, uint columnid, IntPtr pvData, uint cbData, uint grbit, IntPtr psetinfo);

        [DllImport(EsentDll)]
        public static extern int JetGetLock(IntPtr sesid, IntPtr tableid, uint grbit);

        [DllImport(EsentDll)]
        public static extern int JetEscrowUpdate(
            IntPtr sesid,
            IntPtr tableid,
            uint columnid,
            byte[] pv,
            uint cbMax,
            byte[] pvOld,
            uint cbOldMax,
            out uint cbOldActual,
            uint grbit);

        #endregion
    }
}