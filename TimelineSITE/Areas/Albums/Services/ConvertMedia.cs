using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;
using Microsoft.AspNetCore.Components.Forms;

namespace TimelineSITE.Areas.Albums.Services
{
    public static class ConvertMedia
    {
        public static async Task<bool> ConvertVideoToMP4(IFormFile UploadedFile)
        {
            byte[] thumbnailBytes = new Byte[10];

            var videoFilePath = Path.Combine(Path.GetTempPath(), "temp.mp4");
            var videoFilePath2 = Path.Combine(Path.GetTempPath(), "temp2.mp4");

            using Stream fileStream = new FileStream(videoFilePath, FileMode.Create);
            await UploadedFile.CopyToAsync(fileStream);

           
            var imageStream = new MemoryStream();
                await FFMpegArguments
                        .FromFileInput(videoFilePath)
                        .OutputToFile(videoFilePath2, true, options => options
                            .WithVideoCodec(VideoCodec.LibX264)
                            .ForceFormat("mp4")
                            )
                       .ProcessAsynchronously();
            thumbnailBytes = imageStream.ToArray();

            File.WriteAllBytes(videoFilePath2, thumbnailBytes);


            return true;
            
            

        }
    }
}
