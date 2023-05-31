namespace WindowsToolKit.ISO9660
{
    internal class BuildParameters
    {
        public BuildParameters()
        {
            VolumeIdentifier = string.Empty;
            UseJoliet = true;
        }

        public bool UseJoliet { get; set; }

        public string VolumeIdentifier { get; set; }
    }
}