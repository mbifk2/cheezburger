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
            public string PFP_URL { get; set; }
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
            public string? PFP_URL { get; set; }
            public bool? IsBanned { get; set; }
            public bool? IsAdmin { get; set; }
            public bool? IsVerified { get; set; }
        }


        public class TopicDto
        {
            public int TopicID { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime CreatedAt { get; set; }
            public int CreatorId { get; set; }
        }

        public class TopicCreateDto
        {
            public string Title { get; set; }
            public string ?Description { get; set; }
            public int CreatorId { get; set; }
        }

        public class TopicUpdateDto
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public bool? IsHidden { get; set; }
            public int CreatorId { get; set; }
        }

        public class ThreadDto
        {
            public int ThreadID { get; set; }
            public string Title { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsLocked { get; set; }
            public int CreatorId { get; set; }
            public bool? VerifiedOnly { get; set; }
        }

        public class ThreadCreateDto
        {
            public string Title { get; set; }
            public int CreatorId { get; set; }
            public bool? VerifiedOnly { get; set; }
        }

        public class ThreadUpdateDto
        {
            public string? Title { get; set; }
            public bool? IsLocked { get; set; }
            public int CreatorId { get; set; }
            public bool? VerifiedOnly { get; set; }
        }

        public class PostDto
        {
            public int PostID { get; set; }
            public int FthreadID { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
            public int CreatorID { get; set; }
        }

        public class PostGetDto
        {
            public int CreatorID { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class PostCreateDto
        {
            public int CreatorID { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        public class PostUpdateDto
        {
            public string? Content { get; set; }
        }

        public class LoginDto
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }
}
