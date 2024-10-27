namespace CheezAPI
{
    public class Models
    {
        public class User
        {
            public int UserID { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsBanned { get; set; }
            public bool IsGuest { get; set; }
            public bool IsOnline { get; set; }
            public bool IsAdmin { get; set; }
        }

        public class Topic
        {
            public int TopicID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsHidden { get; set; }
        }

        public class Fthread
        {
            public int FthreadID { get; set; }
            public int TopicID { get; set; }
            public string Title { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsLocked { get; set; }
        }

        public class Post
        {
            public int PostID { get; set; }
            public int FthreadID { get; set; }
            public int UserID { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
