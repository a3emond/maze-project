using System;
using System.ComponentModel.DataAnnotations;

namespace MazeGameBlazor.Database.Models
{
    /// <summary>
    /// Represents a media file (image, video, audio, or document) in the system.
    /// </summary>
    public class Media
    {
        /// <summary>
        /// Primary Key: Unique identifier for the media file.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The URL or path where the media file is stored.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// The type of media (Image, Video, Audio, Document).
        /// </summary>
        public MediaType Type { get; set; }
    }

    /// <summary>
    /// Enum representing the different media types.
    /// </summary>
    public enum MediaType
    {
        Image,
        Video,
        Audio,
        Document
    }
}