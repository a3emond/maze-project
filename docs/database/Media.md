# Media.cs

## Overview
The `Media` class represents an **uploaded media file** in the MazeGameBlazor application.  
Each media file has a **URL** and a **type** (Image, Video, Audio, or Document).

## Table Structure

| Column Name  | Type             | Nullable | Description |
|-------------|----------------|----------|-------------|
| `Id`        | `int`           | ❌ No    | Primary Key (auto-incremented). |
| `Url`       | `string(200)`   | ❌ No    | The URL or path where the media file is stored. |
| `Type`      | `MediaType`     | ❌ No    | Enum representing media type (Image, Video, Audio, Document). |

## Relationships

### **BlogPost ⇄ Media** (One-to-One, Optional)
- A blog post **can** have **one** attached media file.
- A media file **must** belong to a blog post.

## Enum: MediaType
The `MediaType` enum defines the possible types of media:

| Enum Value | Description |
|------------|------------|
| `Image`    | Represents an image file (e.g., PNG, JPG). |
| `Video`    | Represents a video file (e.g., MP4, AVI). |
| `Audio`    | Represents an audio file (e.g., MP3, WAV). |
| `Document` | Represents a document file (e.g., PDF, DOCX). |

## Example Usage

### **Creating a New Media Entry**
```csharp
var newMedia = new Media
{
    Url = "https://example.com/media/image1.jpg",
    Type = MediaType.Image
};

dbContext.Media.Add(newMedia);
dbContext.SaveChanges();
