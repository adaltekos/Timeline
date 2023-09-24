using FFMpegCore;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Components.Forms;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TimelineSITE.Areas.Albums.Services
{
    public static class Thumbnail
    {
        public static async Task<byte[]> GenerateThumbnail(IFormFile UploadedFile)
        {
            if (UploadedFile.ContentType == "image/jpeg")
            {
                return await GenerateFromImage(UploadedFile);
            }
            if (UploadedFile.ContentType == "image/png")
            {
                return await GenerateFromImage(UploadedFile);
            }
            if (UploadedFile.ContentType.StartsWith("video/"))
            {
                return await GenerateFromVideo(UploadedFile);
            }

            return new byte[0];
        }

        private static async Task<byte[]> GenerateFromImage(IFormFile UploadedFile) {

            byte[] thumbnailBytes = new Byte[10];

            using (var imageStream = UploadedFile.OpenReadStream())
            using (var image = Image.Load(imageStream))
            {
                // Define the dimensions of the thumbnail
                int thumbnailWidth = 250; // Adjust this to your desired width
                int thumbnailHeight = 250; // Adjust this to your desired height

                // Resize the image to create the thumbnail
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(thumbnailWidth, thumbnailHeight),
                    Mode = ResizeMode.Crop
                }));

                // Convert the thumbnail to a byte array
                using (var thumbnailStream = new MemoryStream())
                {
                    image.Save(thumbnailStream, new JpegEncoder());
                    thumbnailBytes = thumbnailStream.ToArray();
                }
            }

            return thumbnailBytes;
        }

        private static async Task<byte[]> GenerateFromVideo(IFormFile UploadedFile)
        {
            byte[] thumbnailBytes = new Byte[10];


            var thumbFilePath = Path.Combine(Path.GetTempPath(), "temp.png");
            var videoFilePath = Path.Combine(Path.GetTempPath(), "temp.mp4");



            using Stream fileStream = new FileStream(videoFilePath, FileMode.Create);
            await UploadedFile.CopyToAsync(fileStream);


            var realHight = 250;
            var realWidth = 250;

            var mediaInfo = await FFProbe.AnalyseAsync(videoFilePath);


            if (mediaInfo.PrimaryVideoStream.Rotation == -90)
            {
                realHight = mediaInfo.PrimaryVideoStream.Width;
                realWidth = mediaInfo.PrimaryVideoStream.Height;
            }
            else {
                realHight = mediaInfo.PrimaryVideoStream.Height;
                realWidth = mediaInfo.PrimaryVideoStream.Width;
            }


            var snapshotResult = await FFMpeg
                .SnapshotAsync(
                    videoFilePath,
                    thumbFilePath,
                    new System.Drawing.Size(realWidth, realHight),
                    TimeSpan.FromSeconds(0)
                    );

            if (snapshotResult) { thumbnailBytes = File.ReadAllBytes(thumbFilePath); }

            using (var imageStream = new MemoryStream(thumbnailBytes))
            using (var image = Image.Load(imageStream))
            {
                // Define the dimensions of the thumbnail
                int thumbnailWidth = 250; // Adjust this to your desired width
                int thumbnailHeight = 250; // Adjust this to your desired height

                // Resize the image to create the thumbnail
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(thumbnailWidth, thumbnailHeight),
                    Mode = ResizeMode.Crop
                }));

                // Convert the thumbnail to a byte array
                using (var thumbnailStream = new MemoryStream())
                {
                    image.Save(thumbnailStream, new JpegEncoder());
                    thumbnailBytes = thumbnailStream.ToArray();
                }
            }
           
            

            return thumbnailBytes;
        }

        //public static async Task<byte[]> GenerateFromVideo2()
        //{
        //    byte[] thumbnailBytes = new Byte[10];


        //    var thumbFilePath = Path.Combine(Path.GetTempPath(), "temp.png");
        //    var videoFilePath = Path.Combine(Path.GetTempPath(), "temp2.mp4");



           
        //    var realHight = 250;
        //    var realWidth = 250;

        //    var mediaInfo = await FFProbe.AnalyseAsync(videoFilePath);


        //    if (mediaInfo.PrimaryVideoStream.Rotation == -90)
        //    {
        //        realHight = mediaInfo.PrimaryVideoStream.Width;
        //        realWidth = mediaInfo.PrimaryVideoStream.Height;
        //    }
        //    else
        //    {
        //        realHight = mediaInfo.PrimaryVideoStream.Height;
        //        realWidth = mediaInfo.PrimaryVideoStream.Width;
        //    }


        //    var snapshotResult = await FFMpeg
        //        .SnapshotAsync(
        //            videoFilePath,
        //            thumbFilePath,
        //            new System.Drawing.Size(realWidth, realHight),
        //            TimeSpan.FromSeconds(0)
        //            );

        //    if (snapshotResult) { thumbnailBytes = File.ReadAllBytes(thumbFilePath); }

        //    using (var imageStream = new MemoryStream(thumbnailBytes))
        //    using (var image = Image.Load(imageStream))
        //    {
        //        // Define the dimensions of the thumbnail
        //        int thumbnailWidth = 250; // Adjust this to your desired width
        //        int thumbnailHeight = 250; // Adjust this to your desired height

        //        // Resize the image to create the thumbnail
        //        image.Mutate(x => x.Resize(new ResizeOptions
        //        {
        //            Size = new Size(thumbnailWidth, thumbnailHeight),
        //            Mode = ResizeMode.Crop
        //        }));

        //        // Convert the thumbnail to a byte array
        //        using (var thumbnailStream = new MemoryStream())
        //        {
        //            image.Save(thumbnailStream, new JpegEncoder());
        //            thumbnailBytes = thumbnailStream.ToArray();
        //        }
        //    }



        //    return thumbnailBytes;
        //}
    }
}
