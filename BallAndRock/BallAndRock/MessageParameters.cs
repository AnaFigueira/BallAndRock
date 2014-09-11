namespace BallAndRock
{
    public class MessageParameters
    {

        public string Title { get; set; }

        public string Content { get; set; }
        public bool DisplayAppName { get; set; }

        public bool cancelButtonActive { get; set; }


        public MessageParameters(string title, string content, bool displayAppName, bool showCancel)
        {
            this.Title = title;
            this.Content = content;
            this.DisplayAppName = displayAppName;
            this.cancelButtonActive = showCancel;
        }
    }
}