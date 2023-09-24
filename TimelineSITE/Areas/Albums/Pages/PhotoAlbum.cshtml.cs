using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Security.Claims;
using TimelineSITE.Data;
using TimelineSITE.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using TimelineSITE.Areas.Albums.Services;
using System.IO;

namespace TimelineSITE.Areas.Albums.Pages
{
    [Authorize(Policy = "Users")]
    public class PhotoAlbumModel : PageModel
    {
        private readonly UsersDbContext _usersDbContext;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly AlbumPermissions _albumPermissions;

        public PhotoAlbumModel(UsersDbContext usersDbContext, BlobServiceClient blobServiceClient, AlbumPermissions albumPermissions)
        {
            _usersDbContext = usersDbContext;
            _blobServiceClient = blobServiceClient;
            _albumPermissions = albumPermissions;
        }

        public List<ImageModelPlusUrl> Images { get; set; } = new List<ImageModelPlusUrl>();
        public class ImageModelPlusUrl
        {
            public ImageModel Image { get; set; } = new ImageModel();
            public string Url { get; set; }
        };

        public AlbumModel Album { get; private set; } = new AlbumModel();

        private async Task<string> GetSASTokenAsync(BlobContainerClient containerClient)
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                Resource = "c", // Container-level access
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1), // Expiration time
            };
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read | BlobContainerSasPermissions.Write | BlobContainerSasPermissions.List);
            string sasToken = containerClient.GenerateSasUri(sasBuilder).Query;

            return sasToken;
        }

        public async Task<IActionResult> OnGetAsync(string AlbumId)
        {
            //Set who can have access
            var requiredAccessLevels = new List<AccessLevels> { AccessLevels.Owner, AccessLevels.Guest, AccessLevels.ViewOnly };
            var accessIsGranted = await _albumPermissions.CheckAlbumAccess(AlbumId, User, requiredAccessLevels);
            if (!accessIsGranted)
            {
                return RedirectToPage("./Index");
            }

            Album = await _usersDbContext.Albums.FindAsync(new Guid(AlbumId));

            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(AlbumId);

            string sasToken = await GetSASTokenAsync(containerClient);

            try
            {
                Images = await _usersDbContext.Images
                .Where(a => a.AlbumId.ToString() == AlbumId)
                .Select(image => new ImageModelPlusUrl
                {
                    Image = image,
                    Url = containerClient.Uri.ToString() + "/" + image.Id.ToString().ToLower() + sasToken
                })
                .ToListAsync();
            }
            catch (RequestFailedException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUploadFile(IFormFile UploadedFile, string AlbumId)
        {
            //Set who can have access
            var requiredAccessLevels = new List<AccessLevels> { AccessLevels.Owner, AccessLevels.Guest };
            var accessIsGranted = await _albumPermissions.CheckAlbumAccess(AlbumId, User, requiredAccessLevels);
            if (!accessIsGranted)
            {
                return RedirectToPage("./Index");
            }

            var allowedContentTypes = new List<string>
            {
            "image/jpeg",
            "image/png",
            "video/mp4",
            //"video/quicktime"
            };
            if (!(UploadedFile != null && UploadedFile.Length > 0 && allowedContentTypes.Contains(UploadedFile.ContentType)) )
            {
                return new BadRequestObjectResult("File is empty or not allowed type ");
            }

            byte[] thum = new Byte[10];

            //if (UploadedFile.ContentType.StartsWith("video/quicktime"))
            //{
            //    if (await ConvertMedia.ConvertVideoToMP4(UploadedFile)) { thum = await Thumbnail.GenerateFromVideo2(); }
                
            //}

            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(AlbumId);

            try
            {
                var imageModel = new ImageModel
                {
                    AlbumId = new Guid(AlbumId),
                    ContentType = UploadedFile.ContentType,
                    Name = UploadedFile.FileName,
                    ThumbnailImage = UploadedFile.ContentType.StartsWith("video/quicktime")
                    ? thum // Use 'thum' if the ContentType starts with 'video/quicktime'.
                    : await Thumbnail.GenerateThumbnail(UploadedFile) // Otherwise, generate a thumbnail.
                };
                _usersDbContext.Images.Add(imageModel);
                await _usersDbContext.SaveChangesAsync();

                var blobName = imageModel.Id.ToString();
                var blobClient = containerClient.GetBlobClient(blobName);

                using (var stream = UploadedFile.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream);
                }

                return new OkObjectResult("File uploaded successfully.");

            }
            catch (Exception ex)
            {
                // Handle errors and return an error response
                return new BadRequestObjectResult($"An error occurred during file upload: {ex.Message}");
            }

            return new BadRequestObjectResult("Something went wrong");
        }

        public async Task<IActionResult> OnPostDeleteFile(string SelectedCheckboxes, string AlbumId)
        {
            //Set who can have access
            var requiredAccessLevels = new List<AccessLevels> { AccessLevels.Owner, AccessLevels.Guest };
            var accessIsGranted = await _albumPermissions.CheckAlbumAccess(AlbumId, User, requiredAccessLevels);
            if (!accessIsGranted)
            {
                return RedirectToPage("./Index");
            }

            try
            {
                var jsonArray = JArray.Parse(SelectedCheckboxes);
                foreach (string checkboxId in jsonArray)
                {
                    ImageModel image = await _usersDbContext.Images.FindAsync(new Guid(checkboxId));
                    if (image == null)
                    {
                        //return RedirectToPage("./Index");
                    }
                    else
                    {
                        _usersDbContext.Images.Remove(image);
                        await _usersDbContext.SaveChangesAsync();

                        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(AlbumId);

                        var blobClient = containerClient.GetBlobClient(checkboxId);
                        await blobClient.DeleteAsync();
                    }

                    Console.WriteLine("Selected checkbox ID: " + checkboxId);
                }

                return new OkObjectResult("Selected checkboxes deleted successfully.");
            }
            catch (Exception ex)
            {
                // Handle any errors and return an appropriate error response
                return new BadRequestObjectResult("An error occurred: " + ex.Message);
            }
        }
    }
}
