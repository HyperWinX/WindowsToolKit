namespace WindowsToolKit.ISO9660
{
    /// <summary>
    /// Enumeration of known file system variants.
    /// </summary>
    /// <remarks>
    /// <para>ISO9660 has a number of significant limitations, and over time
    /// multiple schemes have been devised for extending the standard
    /// to support the richer file system semantics typical of most modern
    /// operating systems.  These variants differ functionally and (in the
    /// case of RockRidge) may represent a logically different directory
    /// hierarchy to that encoded in the vanilla iso9660 standard.</para>
    /// <para>Use this enum to control which variants to honour / prefer
    /// when accessing an ISO image.</para>
    /// </remarks>
    public enum Iso9660Variant
    {
        /// <summary>
        /// No known variant.
        /// </summary>
        None,

        /// <summary>
        /// Vanilla ISO9660.
        /// </summary>
        Iso9660,

        /// <summary>
        /// Joliet file system (Windows).
        /// </summary>
        Joliet,

        /// <summary>
        /// Rock Ridge (Unix).
        /// </summary>
        RockRidge
    }
}