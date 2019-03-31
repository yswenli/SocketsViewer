namespace SocketsViewer.Model
{
    public class DiskInfo
    {
        public string Name { get; set; }

        public long Size { get; set; }

        public long FreeSpace { get; set; }

        public DiskInfo()
        {

        }

        public DiskInfo(string name, long size, long freeSpace)
        {
            Name = name;
            Size = size;
            FreeSpace = freeSpace;
        }
    }
}
