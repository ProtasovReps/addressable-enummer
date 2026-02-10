namespace Editor
{
    public struct GroupEntry
    {
        public GroupEntry(string name, bool isSelected)
        {
            Name = name;
            IsSelected = isSelected;
        }

        public string Name { get; }
        public bool IsSelected { get; }
    }
}