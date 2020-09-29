using DeltaWebMap.ContentDatabase.Framework.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DeltaWebMap.ContentDatabase.Framework.FileIO
{
    class DatabaseFile : IDisposable
    {
        private FileStream file;
        private int pageContentSize;
        private int modifiedUtc;
        private List<DatabasePage> pages;
        private List<DatabasePage> orphanedPages;

        public DatabaseFile(FileStream file)
        {
            this.file = file;
            this.orphanedPages = new List<DatabasePage>();
        }

        public static DatabaseFile CreateFile(string path, int pageSize = 512)
        {
            //Open and create
            FileStream file = new FileStream(path, FileMode.Create);
            DatabaseFile dbFile = new DatabaseFile(file);

            //Write header
            dbFile.WriteInt32ToStream(FILE_SIN);
            dbFile.WriteInt32ToStream(1);
            dbFile.WriteInt32ToStream(pageSize);
            dbFile.WriteInt32ToStream(dbFile.GetCurrentUtcEpoch());
            dbFile.WriteInt64ToStream(0);

            //Open to fully generate
            dbFile.ReadHeader();

            return dbFile;
        }

        public static DatabaseFile LoadFile(string path)
        {
            //Open and create
            FileStream file = new FileStream(path, FileMode.Open);
            DatabaseFile dbFile = new DatabaseFile(file);

            //Read header
            dbFile.ReadHeader();

            return dbFile;
        }

        public void ReadHeader()
        {
            //Jump to beginning
            file.Position = 0;

            //Read file signature
            if (ReadInt32FromStream() != FILE_SIN)
                throw new Exception("This is not a Delta Web Map Content file.");

            //Read version
            if (ReadInt32FromStream() != 1)
                throw new Exception("Unknown file version.");

            //Read header data
            pageContentSize = ReadInt32FromStream();
            modifiedUtc = ReadInt32FromStream();
            long pageCount = ReadInt64FromStream();

            //Read data for each page
            pages = new List<DatabasePage>();
            for (long i = 0; i < pageCount; i += 1)
            {
                //Jump to page
                file.Position = GetPageFileOffset(i);

                //Read
                int pageType = ReadInt32FromStream();
                int pageModifiedUtc = ReadInt32FromStream();
                int pageContentSize = ReadInt32FromStream();
                int pageReserved = ReadInt32FromStream();
                long pageNextIndex = ReadInt64FromStream();

                //Make
                var page = new DatabasePage
                {
                    index = i,
                    type = pageType,
                    modified_utc = pageModifiedUtc,
                    total_blob_length = pageContentSize,
                    flags = pageReserved,
                    next_page_index = pageNextIndex
                };

                //Add
                pages.Add(page);

                //If this is an orphaned page, set it as so
                if (pageTotalSize == PAGE_TYPE_ORPHAN)
                    orphanedPages.Add(page);
            }
        }

        public int pageTotalSize { get { return pageContentSize + FILE_HEADER_SIZE; } } //The total size of pages, including the content and the header

        public const byte PAGE_TYPE_ORPHAN = 0x00;
        public const int FILE_SIN = 1414415172;

        public const int FILE_HEADER_SIZE = 24;
        //0     Int32   File sign "DCNT" (Delta Content)
        //4     Int32   File version const 1
        //8     Int32   Page Content Size
        //12    Int32   Last modified UTC (seconds since Jan 1, 1970)
        //16    Int64   Page count

        public const int PAGE_HEADER_SIZE = 24;
        //0     Int32   Type
        //4     Int32   Modified UTC
        //8     Int32   Total Object Length
        //12     Int32   Reserved
        //16     Int64   Next Page Index

        //PAGE FLAGS
        //0     If set, indicates that this is a continuation of an existing page

        /// <summary>
        /// Returns the offset from the beginning of the file to the begining of the page header
        /// </summary>
        /// <param name="pageIndex"></param>
        private long GetPageFileOffset(long pageIndex)
        {
            return FILE_HEADER_SIZE + (pageIndex * pageTotalSize);
        }

        /// <summary>
        /// Gets the current UTC epoch as an int32
        /// </summary>
        /// <returns></returns>
        private int GetCurrentUtcEpoch()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        private void WriteInt32ToStream(int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            file.Write(data, 0, 4);
        }

        private void WriteInt64ToStream(long value)
        {
            byte[] data = BitConverter.GetBytes(value);
            file.Write(data, 0, 8);
        }

        private int ReadInt32FromStream()
        {
            byte[] buffer = new byte[4];
            file.Read(buffer, 0, 4);
            return BitConverter.ToInt32(buffer);
        }

        private long ReadInt64FromStream()
        {
            byte[] buffer = new byte[8];
            file.Read(buffer, 0, 8);
            return BitConverter.ToInt64(buffer);
        }

        private DatabasePage CreateDatabasePage()
        {
            //Jump to new page location
            file.Position = GetPageFileOffset(pages.Count);

            //Write placeholder data
            for (int i = 0; i < pageTotalSize; i++)
                file.WriteByte(0x00);

            //Add page info
            DatabasePage pageInfo = new DatabasePage
            {
                index = pages.Count,
                type = PAGE_TYPE_ORPHAN,
                modified_utc = GetCurrentUtcEpoch(),
                next_page_index = -1
            };
            pages.Add(pageInfo);
            orphanedPages.Add(pageInfo);

            //Jump to counter and update
            file.Position = 16;
            WriteInt64ToStream(pages.Count);

            //Jump to new page location
            file.Position = GetPageFileOffset(pageInfo.index);

            return pageInfo;
        }

        private void UpdateDatabasePage(DatabasePage page, int pageType, long nextPageIndex, int totalBlobLength, int flags)
        {
            //Get current last modified time
            int modifiedUtc = GetCurrentUtcEpoch();

            //Jump to file header to write the current UTC time
            file.Position = 12;
            WriteInt32ToStream(modifiedUtc);

            //Jump to the header for this page
            file.Position = GetPageFileOffset(page.index);

            //Update type
            WriteInt32ToStream(pageType);
            page.type = pageType;

            //Update UTC time
            WriteInt32ToStream(modifiedUtc);
            page.modified_utc = modifiedUtc;

            //Update blob length
            WriteInt32ToStream(totalBlobLength);
            page.total_blob_length = totalBlobLength;

            //Update flags
            WriteInt32ToStream(flags);
            page.flags = flags;

            //Update next page index
            WriteInt64ToStream(nextPageIndex);
            page.next_page_index = nextPageIndex;

            //Add/remove to orphaned pages if needed
            bool isInOrphaned = orphanedPages.Contains(page);
            if (pageType == PAGE_TYPE_ORPHAN && !isInOrphaned)
                orphanedPages.Add(page);
            if (pageTotalSize != PAGE_TYPE_ORPHAN && isInOrphaned)
                orphanedPages.Remove(page);
        }

        private void UpdateDatabasePage(DatabasePage page, int pageType, long nextPageIndex, byte[] buffer, int pageIndex, int flags)
        {
            //Update metadata
            UpdateDatabasePage(page, pageType, nextPageIndex, buffer.Length, flags);

            //Calculate where to read from the buffer
            int bufferReadOffset = pageIndex * pageContentSize;
            int bufferReadLength = Math.Min(pageContentSize, buffer.Length - bufferReadOffset);

            //Copy content
            file.Write(buffer, bufferReadOffset, bufferReadLength);
        }

        public long WriteNewBlob(int objectType, byte[] payload)
        {
            //Calculate number of chunks we need
            int objectPageCount = (payload.Length / pageContentSize) + 1;

            //Find orphaned pages. These are pages that are unused
            DatabasePage[] objectPages = new DatabasePage[objectPageCount];
            int objectPageIndex = 0;
            foreach(var p in orphanedPages)
            {
                if(p.type == PAGE_TYPE_ORPHAN)
                {
                    objectPages[objectPageIndex] = p;
                    objectPageIndex++;
                    if (objectPageIndex == objectPageCount)
                        break;
                }
            }

            //If we need additional pages, create them
            while(objectPageIndex < objectPageCount)
            {
                objectPages[objectPageIndex] = CreateDatabasePage();
                objectPageIndex++;
            }

            //Now write content to each page
            for(int i = 0; i<objectPageCount; i+=1)
            {
                //Find the index of the next page
                long nextPageIndex;
                if (i + 1 == objectPageCount)
                    nextPageIndex = -1;
                else
                    nextPageIndex = objectPages[i + 1].index;

                //Make flags
                int flags = 0;
                if(i != 0) //If this is not the first one, set the continuation flag to true
                    flags |= 1 << DatabasePage.FLAG_CONTINUATION;

                //Write content
                UpdateDatabasePage(objectPages[i], objectType, nextPageIndex, payload, i, flags);
            }

            return objectPages[0].index;
        }

        public long UpdateBlob(long pageIndex, int newObjectType, byte[] payload)
        {
            //First, orphan all pages used by the page we're updating. We just remove them
            DeleteBlob(pageIndex);

            //Now, recreate it
            return WriteNewBlob(newObjectType, payload);
        }

        public void DeleteBlob(long pageIndex)
        {
            //Get first page
            long nextPageIndex = pageIndex;

            //Loop through this page and it's decendants
            while(nextPageIndex != -1)
            {
                //Get page
                DatabasePage page = pages[(int)nextPageIndex];

                //Get index of the next page before it's removed
                nextPageIndex = page.next_page_index;

                //Update the page selected
                UpdateDatabasePage(page, PAGE_TYPE_ORPHAN, -1, 0, 0x00);
            }
        }

        public byte[] ReadBlob(long pageIndex)
        {
            //Create buffer from the length specified in the first page
            byte[] buffer = new byte[pages[(int)pageIndex].total_blob_length];

            //Begin reading pages
            long nextPageIndex = pageIndex;
            int currentPageIndex = 0; //Read index for writing to our buffer

            //Loop through this page and it's decendants
            while (nextPageIndex != -1)
            {
                //Get page
                DatabasePage page = pages[(int)nextPageIndex];

                //Get index of the next page
                nextPageIndex = page.next_page_index;

                //Calculate read parameters
                int readOffset = currentPageIndex * pageContentSize;
                int readLength = Math.Min(pageContentSize, buffer.Length - readOffset);

                //Jump to and read
                file.Position = GetPageFileOffset(page.index) + PAGE_HEADER_SIZE;
                file.Read(buffer, readOffset, readLength);
                currentPageIndex++;
            }

            return buffer;
        }

        public long FindBlob(int type, long after = -1)
        {
            //Search for a blob of this type
            for(long i = after + 1; i<pages.Count; i++)
            {
                if (pages[(int)i].type == type && !pages[(int)i].CheckFlag(DatabasePage.FLAG_CONTINUATION)) //Look for the type that is NOT a continuation
                    return pages[(int)i].index;
            }
            return -1;
        }

        public void Dispose()
        {
            file.Flush();
            file.Close();
        }
    }
}
