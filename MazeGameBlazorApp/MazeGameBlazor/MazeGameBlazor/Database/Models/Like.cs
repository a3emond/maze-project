namespace MazeGameBlazor.Database.Models
{
    public class Like
    {
        public int Id { get; set; } // Primary Key

        public int BlogPostId { get; set; } // Foreign Key
        public BlogPost BlogPost { get; set; }

        public string UserId { get; set; } // Foreign Key
        public User User { get; set; }
    }
}
