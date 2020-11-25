using DeltaWebMap.ContentDatabase.CommitBuilders.Find;
using DeltaWebMap.ContentDatabase.CommitBuilders.Write;
using DeltaWebMap.ContentDatabase.Framework;
using DeltaWebMap.ContentDatabase.Framework.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace DeltaWebMap.ContentDatabase
{
    public class ContentDatabaseSession : IDisposable, INameTableProvider
    {
        private DatabaseFile file;
        private long tocPage;
        private long nameTablePage;
        private bool nameTableDirty;

        private List<DatabaseObjectHeader> objects;
        private List<string> nameTable;

        public const byte PAGE_TYPE_TOC = 0x01;
        public const byte PAGE_TYPE_NAMETABLE = 0x02;
        public const byte PAGE_TYPE_OBJECT = 0x03;

        public const int TOC_ENTRY_LENGTH = 32;
        //0     UInt64      Object ID
        //8     UInt64      Page ID
        //16    UInt64      Commit ID
        //24    UInt32      Group ID
        //28    Byte        Commit Type
        //29    Byte        Flags
        //30    UShort      Reserved

        private ContentDatabaseSession(DatabaseFile file)
        {
            this.file = file;
            tocPage = file.FindBlob(PAGE_TYPE_TOC);
            nameTablePage = file.FindBlob(PAGE_TYPE_NAMETABLE);
            objects = new List<DatabaseObjectHeader>();
            nameTable = new List<string>();
        }

        public static ContentDatabaseSession CreateDatabase(string path, int pageSize = 512)
        {
            //Open file
            DatabaseFile file = DatabaseFile.CreateFile(path, pageSize);

            //Open DB
            var db = new ContentDatabaseSession(file);

            //Save TOC and NameTable
            db.UpdateToc();
            db.UpdateNameTable();

            return db;
        }

        public static ContentDatabaseSession OpenDatabase(string path)
        {
            //Open file
            DatabaseFile file = DatabaseFile.LoadFile(path);

            //Open DB
            var db = new ContentDatabaseSession(file);

            //Read TOC
            if(db.tocPage != -1)
            {
                //Read blob
                byte[] tocBytes = file.ReadBlob(db.tocPage);
                int count = tocBytes.Length / TOC_ENTRY_LENGTH;
                BufferBinaryTool reader = new BufferBinaryTool(tocBytes);

                //Add all
                for (int i = 0; i<count; i++)
                {
                    int offset = TOC_ENTRY_LENGTH * i;
                    DatabaseObjectHeader entry = new DatabaseObjectHeader();
                    entry.object_id = reader.ReadUInt64(offset + 0);
                    entry.page_id = reader.ReadUInt64(offset + 8);
                    entry.commit_id = reader.ReadUInt64(offset + 16);
                    entry.group_id = reader.ReadInt32(offset + 24);
                    entry.commit_type = tocBytes[offset + 28];
                    entry.flags = tocBytes[offset + 29];
                    entry.reserved = reader.ReadUInt16(offset + 30);
                    db.objects.Add(entry);
                }
            }

            //Read Name Table
            if(db.nameTablePage != -1)
            {
                //Read blob
                byte[] ntBytes = file.ReadBlob(db.nameTablePage);
                BufferBinaryTool reader = new BufferBinaryTool(ntBytes);

                //Begin reading
                int index = 0;
                while(index < ntBytes.Length)
                {
                    //Read length
                    int length = reader.ReadInt32(index);

                    //Read string
                    db.nameTable.Add(Encoding.ASCII.GetString(ntBytes, index + 4, length));

                    //Update
                    index += length + 4;
                }
            }

            return db;
        }

        public void Dispose()
        {
            file.Dispose();
        }

        private void UpdateToc()
        {
            //Open buffer for all objects
            byte[] objectTocBuffer = new byte[TOC_ENTRY_LENGTH * objects.Count];
            BufferBinaryTool writer = new BufferBinaryTool(objectTocBuffer);

            //Add all
            for (int i = 0; i<objects.Count; i+=1)
            {
                int offset = TOC_ENTRY_LENGTH * i;
                writer.WriteUInt64(offset + 0, objects[i].object_id);
                writer.WriteUInt64(offset + 8, objects[i].page_id);
                writer.WriteUInt64(offset + 16, objects[i].commit_id);
                writer.WriteInt32(offset + 24, objects[i].group_id);
                objectTocBuffer[offset + 28] = objects[i].commit_type;
                objectTocBuffer[offset + 29] = objects[i].flags;
                writer.WriteUInt16(offset + 30, objects[i].reserved);
            }

            //Add or update
            if (tocPage == -1)
                tocPage = file.WriteNewBlob(PAGE_TYPE_TOC, objectTocBuffer);
            else
                tocPage = file.UpdateBlob(tocPage, PAGE_TYPE_TOC, objectTocBuffer);
        }

        private void UpdateNameTable()
        {
            //Calculate the length of the name table
            int nameTableLength = 0;
            foreach (var n in nameTable)
                nameTableLength += Encoding.ASCII.GetByteCount(n) + 4;

            //Open buffer
            byte[] buffer = new byte[nameTableLength];
            BufferBinaryTool writer = new BufferBinaryTool(buffer);

            //Write
            int byteIndex = 0;
            foreach(var n in nameTable)
            {
                byte[] nameBytes = Encoding.ASCII.GetBytes(n);
                nameBytes.CopyTo(buffer, byteIndex + 4);
                writer.WriteInt32(byteIndex, nameBytes.Length);
                byteIndex += nameBytes.Length + 4;
            }

            //Add or update
            if (nameTablePage == -1)
                nameTablePage = file.WriteNewBlob(PAGE_TYPE_NAMETABLE, buffer);
            else
                nameTablePage = file.UpdateBlob(nameTablePage, PAGE_TYPE_NAMETABLE, buffer);

            //Set flag
            nameTableDirty = false;
        }

        /* File store */

        private void LowLevelUpsertObject(ulong id, ulong commit_id, int group_id, byte commit_type, byte[] payload)
        {
            //Look for an existing entry with this ID
            DatabaseObjectHeader entry = null;
            foreach(var e in objects)
            {
                if (e.object_id == id)
                    entry = e;
            }

            //Update or add the payload data
            long payloadPage;
            if (entry == null)
                payloadPage = file.WriteNewBlob(PAGE_TYPE_OBJECT, payload); //Make a new blob
            else
                payloadPage = file.UpdateBlob((long)entry.page_id, PAGE_TYPE_OBJECT, payload); //Update existing blob

            //Update or create entry
            if(entry == null)
            {
                entry = new DatabaseObjectHeader
                {
                    object_id = id,
                    page_id = (ulong)payloadPage,
                    commit_id = commit_id,
                    group_id = group_id,
                    commit_type = commit_type,
                    flags = 0,
                    reserved = 0
                };
                objects.Add(entry);
            } else
            {
                entry.page_id = (ulong)payloadPage;
                entry.commit_id = commit_id;
                entry.group_id = group_id;
                entry.commit_type = commit_type;
                entry.flags = 0;
                entry.reserved = 0;
            }

            //Note that we do NOT update the TOC. That will be done when this is fully commited
        }

        /// <summary>
        /// Finds blobs matching the expression. This method is fast, but you need to deserialize it elsewhere. Run this from the worker thread.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private FindCommitResults Find(Expression<Func<DatabaseObjectHeader, bool>> expression, int skip = 0, int limit = int.MaxValue)
        {
            //Compile
            var expressionCompiled = expression.Compile();

            //Search for objects matching
            List<DatabaseObjectHeader> matchingHeaders = new List<DatabaseObjectHeader>();
            int found = 0;
            for (int i = 0; i < objects.Count; i++)
            {
                //Check
                if (expressionCompiled(objects[i]))
                {
                    //Count. This might have to go before the check
                    found++;

                    //Check if we should add
                    if (found > skip)
                        matchingHeaders.Add(objects[i]);

                    //Check if we've hit the limit
                    if (matchingHeaders.Count == limit)
                        break;
                }
            }

            //Load all
            FindCommitResults pack = new FindCommitResults();
            foreach (var h in matchingHeaders)
            {
                //Load blob
                byte[] blob = file.ReadBlob((long)h.page_id);

                //Pack
                FindCommitResultsObject obj = new FindCommitResultsObject
                {
                    blob = blob,
                    commit_id = h.commit_id,
                    commit_type = h.commit_type,
                    group_id = h.group_id,
                    object_id = h.object_id,
                    nameTable = this
                };
                pack.results.Add(obj);
            }
            return pack;
        }

        public short GetNameTableIndex(string value)
        {
            //If this contains it, return it
            if (nameTable.Contains(value))
                return (short)nameTable.IndexOf(value);

            //Doesn't exist, add it and mark dirty
            int index = nameTable.Count;
            nameTable.Add(value);
            nameTableDirty = true;

            return (short)index;
        }

        public string GetNameTableValue(short index)
        {
            return nameTable[index];
        }

        /* Higher level, fast operations */

        /// <summary>
        /// Applies a write commit to the disk
        /// </summary>
        /// <param name="commit"></param>
        public int ApplyWriteCommit(WriteCommit commit)
        {
            //Loop through and push each object
            int totalSize = 0;
            foreach (var obj in commit.commits)
            {
                //Serialize. Get length
                int len = obj.GetLength();
                totalSize += len;

                //Open buffer
                byte[] payload = new byte[len];

                //Serialize to this
                obj.SerializeTo(this, payload, 0);

                //Insert
                LowLevelUpsertObject(obj.objectId, commit.commitId, obj.groupId, commit.commitType, payload);
            }

            //Update name table if needed
            if (nameTableDirty)
                UpdateNameTable();

            //Update TOC
            UpdateToc();

            return totalSize;
        }

        /// <summary>
        /// Returns all results in the specified group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public FindCommitResults FindByGroup(int groupId, int skip = 0, int limit = int.MaxValue)
        {
            return Find(x => x.group_id == groupId, skip, limit);
        }

        /// <summary>
        /// Returns all results
        /// </summary>
        /// <param name="skip"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public FindCommitResults FindAll(int skip = 0, int limit = int.MaxValue)
        {
            return Find(x => true, skip, limit);
        }

        /// <summary>
        /// Counts the number of items total
        /// </summary>
        /// <returns></returns>
        public long CountAllItems()
        {
            return objects.Count;
        }

        /// <summary>
        /// Counts the number of items belonging to a team
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public long CountTeamItems(int groupId)
        {
            long count = 0;
            foreach (var o in objects)
                if (o.group_id == groupId)
                    count++;
            return count;
        }
    }
}
