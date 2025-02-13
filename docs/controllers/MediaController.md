# MediaController

## Overview
The `MediaController` provides API endpoints for handling media files, including streaming, uploading, and attaching media to blog posts. 

## Endpoints

### 📌 Stream Media
**GET** `/api/media/stream/{filename}`

#### Description
Streams a media file for playback.

#### Request Parameters
- `filename` (string): The name of the media file to be streamed.

#### Response
- **200 OK**: Returns the media file stream.
- **404 Not Found**: If the file does not exist.

---

### 📌 Upload Media
**POST** `/api/media/upload`

#### Description
Uploads a media file to the server and stores metadata in the database.

#### Request Body
- `file` (IFormFile): The file to be uploaded.

#### Response
- **200 OK**: Returns uploaded media details (ID, URL, Type).
- **400 Bad Request**: If no file is uploaded.
- **500 Internal Server Error**: If an error occurs.

---

### 📌 Retrieve Available Media
**GET** `/api/media`

#### Description
Retrieves all media records from the database.

#### Response
- **200 OK**: A list of media objects.

---

### 📌 Attach Media to Blog Post
**POST** `/api/media/attach`

#### Description
Attaches media files to a blog post.

#### Request Body (JSON)
```json
{
  "blogPostId": 1,
  "mediaIds": [101, 102, 103]
}
```

#### Response
- **200 OK**: `"Media successfully attached to blog post."`
- **404 Not Found**: If the blog post or media files do not exist.

---

## 🔧 Helper Functionality

### DetermineMediaType Method
This function categorizes files into:
- **Image**: `image/jpeg`, `image/png`, `image/gif`
- **Video**: `video/mp4`, `video/webm`
- **Audio**: `audio/mpeg`, `audio/wav`
- **Document** (default)

---

## Example Usage

### Uploading a Media File
#### cURL Example:
```sh
curl -X POST "http://localhost:5000/api/media/upload" \
  -F "file=@video.mp4"
```

### Streaming a Video
#### cURL Example:
```sh
curl -X GET "http://localhost:5000/api/media/stream/sample.mp4" --output sample.mp4
```

### Attaching Media to a Blog Post
#### cURL Example:
```sh
curl -X POST "http://localhost:5000/api/media/attach" \
  -H "Content-Type: application/json" \
  -d '{
        "blogPostId": 1,
        "mediaIds": [101, 102]
      }'
```

---

