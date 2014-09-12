namespace BallAndRock
{
    public class MessageParameters
    {

        public string Title { get; set; }

        public string Content { get; set; }
        public bool DisplayAppName { get; set; }


        public MessageParameters(string title, string content, bool displayAppName)
        {
            this.Title = title;
            this.Content = content;
            this.DisplayAppName = displayAppName;
        }
    }
}