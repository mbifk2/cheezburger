namespace CheezAPI
{
    public class Dtos
    {
        public class UserDto
        {
            public int UserID { get; set; }
            public string Username { get; set; }
            //public string Email { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class UserCreateDto
        {
            public string Username { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class UserUpdateDto
        {
            public string? Username { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
            public bool? IsBanned { get; set; } 
            public bool? IsAdmin { get; set; }
        }


        public class TopicDto
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class TopicCreateDto {
            public string Title { get; set; }
            public string Description { get; set; }
        }

        public class FthreadDto
        {
            public int FthreadID { get; set; }
            public int TopicID { get; set; }
            public string Title { get; set; }
        }

        public class PostDto
        {
            public int PostID { get; set; }
            public int FthreadID { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
        }

    }
}
