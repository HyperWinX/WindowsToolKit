using System;

namespace WindowsToolKit
{
    /// <summary>
    /// Standard Unix-style file system permissions.
    /// </summary>
    [Flags]
    public enum UnixFilePermissions
    {
        /// <summary>
        /// No permissions.
        /// </summary>
        None = 0,

        /// <summary>
        /// Any user execute permission.
        /// </summary>
        OthersExecute = 0x001,

        /// <summary>
        /// Any user write permission.
        /// </summary>
        OthersWrite = 0x002,

        /// <summary>
        /// Any user read permission.
        /// </summary>
        OthersRead = 0x004,

        /// <summary>
        /// Any user all permissions.
        /// </summary>
        OthersAll = OthersExecute | OthersWrite | OthersRead,

        /// <summary>
        /// Group execute permission.
        /// </summary>
        GroupExecute = 0x008,

        /// <summary>
        /// Group write permission.
        /// </summary>
        GroupWrite = 0x010,

        /// <summary>
        /// Group read permission.
        /// </summary>
        GroupRead = 0x020,

        /// <summary>
        /// Group all permissions.
        /// </summary>
        GroupAll = GroupExecute | GroupWrite | GroupRead,

        /// <summary>
        /// Owner execute permission.
        /// </summary>
        OwnerExecute = 0x040,

        /// <summary>
        /// Owner write permission.
        /// </summary>
        OwnerWrite = 0x080,

        /// <summary>
        /// Owner read permission.
        /// </summary>
        OwnerRead = 0x100,

        /// <summary>
        /// Owner all permissions.
        /// </summary>
        OwnerAll = OwnerExecute | OwnerWrite | OwnerRead,

        /// <summary>
        /// Sticky bit (meaning ill-defined).
        /// </summary>
        Sticky = 0x200,

        /// <summary>
        /// Set GUID on execute.
        /// </summary>
        SetGroupId = 0x400,

        /// <summary>
        /// Set UID on execute.
        /// </summary>
        SetUserId = 0x800
    }
}