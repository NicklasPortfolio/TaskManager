namespace TaskManager
{
    internal class Task
    {
        public string DueDate { get; set; } // När ska det vara klart?
        public PriorityType Priority { get; set; } // Hur viktigt är det?
        public string Description { get; set; } // Vad ska du göra?
        public enum PriorityType { Low, Medium, High } // Enum Priority Type

        // Constructor för listbox
        public override string ToString()
        {
            return $"{DueDate}, {Priority}, {Description}";
        }
    }
}
