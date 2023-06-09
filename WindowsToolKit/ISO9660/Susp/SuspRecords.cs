﻿using System.Collections.Generic;
using WindowsToolKit.Streams;

namespace WindowsToolKit.ISO9660
{
    internal sealed class SuspRecords
    {
        private readonly Dictionary<string, Dictionary<string, List<SystemUseEntry>>> _records;

        public SuspRecords(IsoContext context, byte[] data, int offset)
        {
            _records = new Dictionary<string, Dictionary<string, List<SystemUseEntry>>>();

            ContinuationSystemUseEntry contEntry = Parse(context, data, offset + context.SuspSkipBytes);
            while (contEntry != null)
            {
                context.DataStream.Position = contEntry.Block * (long)context.VolumeDescriptor.LogicalBlockSize +
                                              contEntry.BlockOffset;
                byte[] contData = StreamUtilities.ReadExact(context.DataStream, (int)contEntry.Length);

                contEntry = Parse(context, contData, 0);
            }
        }

        public static bool DetectSharingProtocol(byte[] data, int offset)
        {
            if (data == null || data.Length - offset < 7)
            {
                return false;
            }

            return data[offset] == 83
                   && data[offset + 1] == 80
                   && data[offset + 2] == 7
                   && data[offset + 3] == 1
                   && data[offset + 4] == 0xBE
                   && data[offset + 5] == 0xEF;
        }

        public List<SystemUseEntry> GetEntries(string extension, string name)
        {
            if (string.IsNullOrEmpty(extension))
            {
                extension = string.Empty;
            }

            Dictionary<string, List<SystemUseEntry>> extensionData;
            if (!_records.TryGetValue(extension, out extensionData))
            {
                return null;
            }

            List<SystemUseEntry> result;
            if (extensionData.TryGetValue(name, out result))
            {
                return result;
            }

            return null;
        }

        public T GetEntry<T>(string extension, string name)
            where T : SystemUseEntry
        {
            List<SystemUseEntry> entries = GetEntries(extension, name);
            if (entries == null)
            {
                return null;
            }

            foreach (T entry in entries)
            {
                return entry;
            }

            return null;
        }

        public bool HasEntry(string extension, string name)
        {
            List<SystemUseEntry> entries = GetEntries(extension, name);
            return entries != null && entries.Count != 0;
        }

        private ContinuationSystemUseEntry Parse(IsoContext context, byte[] data, int offset)
        {
            ContinuationSystemUseEntry contEntry = null;
            SuspExtension extension = null;

            if (context.SuspExtensions != null && context.SuspExtensions.Count > 0)
            {
                extension = context.SuspExtensions[0];
            }

            int pos = offset;
            while (data.Length - pos > 4)
            {
                byte len;
                SystemUseEntry entry = SystemUseEntry.Parse(data, pos, context.VolumeDescriptor.CharacterEncoding,
                    extension, out len);
                pos += len;

                if (entry == null)
                {
                    // A null entry indicates SUSP parsing must terminate.
                    // This will occur if a termination record is found,
                    // or if there is a problem with the SUSP data.
                    return contEntry;
                }

                switch (entry.Name)
                {
                    case "CE":
                        contEntry = (ContinuationSystemUseEntry)entry;
                        break;

                    case "ES":
                        ExtensionSelectSystemUseEntry esEntry = (ExtensionSelectSystemUseEntry)entry;
                        extension = context.SuspExtensions[esEntry.SelectedExtension];
                        break;

                    case "PD":
                        break;

                    case "SP":
                    case "ER":
                        StoreEntry(null, entry);
                        break;

                    default:
                        StoreEntry(extension, entry);
                        break;
                }
            }

            return contEntry;
        }

        private void StoreEntry(SuspExtension extension, SystemUseEntry entry)
        {
            string extensionId = extension == null ? string.Empty : extension.Identifier;

            Dictionary<string, List<SystemUseEntry>> extensionEntries;
            if (!_records.TryGetValue(extensionId, out extensionEntries))
            {
                extensionEntries = new Dictionary<string, List<SystemUseEntry>>();
                _records.Add(extensionId, extensionEntries);
            }

            List<SystemUseEntry> entries;
            if (!extensionEntries.TryGetValue(entry.Name, out entries))
            {
                entries = new List<SystemUseEntry>();
                extensionEntries.Add(entry.Name, entries);
            }

            entries.Add(entry);
        }
    }
}