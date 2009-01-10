﻿//-----------------------------------------------------------------------
// <copyright file="Update.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Isam.Esent.Interop
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// A class that encapsulates an update on a JET_TABLEID
    /// </summary>
    public class Update : EsentResource
    {
        /// <summary>
        /// The underlying JET_SESID.
        /// </summary>
        private readonly JET_SESID sesid;

        /// <summary>
        /// The underlying JET_TABLEID.
        /// </summary>
        private readonly JET_TABLEID tableid;

        /// <summary>
        /// Initializes a new instance of the Update class. This automatically
        /// begins an update. The update will be cancelled if
        /// not explicitly saved.
        /// </summary>
        /// <param name="sesid">The session to start the transaction for.</param>
        /// <param name="tableid">The tableid to prepare the update for.</param>
        /// <param name="prep">The type of update.</param>
        public Update(JET_SESID sesid, JET_TABLEID tableid, JET_prep prep)
        {
            if (JET_prep.Cancel == prep)
            {
                throw new ArgumentException("Cannot create an Update for JET_prep.Cancel", "prep");
            }

            this.sesid = sesid;
            this.tableid = tableid;
            Api.JetPrepareUpdate(this.sesid, this.tableid, prep);
            this.ResourceWasAllocated();
        }

        /// <summary>
        /// Update the tableid.
        /// </summary>
        /// <param name="bookmark">Returns the bookmark of the updated record. This can be null.</param>
        /// <param name="bookmarkSize">The size of the bookmark buffer.</param>
        /// <param name="actualBookmarkSize">Returns the actual size of the bookmark.</param>
        /// <remarks>
        /// Save is the final step in performing an insert or an update. The update is begun by
        /// calling creating an Update object and then by calling JetSetColumn or JetSetColumns one or more times
        /// to set the record state. Finally, Update is called to complete the update operation.
        /// Indexes are updated only by Update or and not during JetSetColumn or JetSetColumns
        /// </remarks>
        public void Save(byte[] bookmark, int bookmarkSize, out int actualBookmarkSize)
        {
            this.CheckObjectIsNotDisposed();
            if (!this.HasResource)
            {
                throw new InvalidOperationException("Not in an update");
            }

            Api.JetUpdate(this.sesid, this.tableid, bookmark, bookmarkSize, out actualBookmarkSize);
            this.ResourceWasReleased();
        }

        /// <summary>
        /// Update the tableid.
        /// </summary>
        /// <remarks>
        /// Save is the final step in performing an insert or an update. The update is begun by
        /// calling creating an Update object and then by calling JetSetColumn or JetSetColumns one or more times
        /// to set the record state. Finally, Update is called to complete the update operation.
        /// Indexes are updated only by Update or and not during JetSetColumn or JetSetColumns
        /// </remarks>
        public void Save()
        {
            int ignored;
            this.Save(null, 0, out ignored);
        }

        /// <summary>
        /// Cancel the update.
        /// </summary>
        public void Cancel()
        {
            this.CheckObjectIsNotDisposed();
            if (!this.HasResource)
            {
                throw new InvalidOperationException("Not in an update");
            }

            Api.JetPrepareUpdate(this.sesid, this.tableid, JET_prep.Cancel);
            this.ResourceWasReleased();
        }

        /// <summary>
        /// Called when the transaction is being disposed while active.
        /// This should rollback the transaction.
        /// </summary>
        protected override void ReleaseResource()
        {
            this.Cancel();
        }
    }
}