using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using NASAClient;
using Microsoft.Extensions.Caching.Memory;
using NASAClient.Models;
using MarsRoverAPI.Models;

namespace MarsRoverAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PictureController : ControllerBase
    {
        private readonly ILogger<PictureController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;

        public PictureController(ILogger<PictureController> logger, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> GetInfo()
        {
            var marsRoverEndpoint = new MarsRoverApi(_httpClientFactory);
            var dates = GetDatesFromFile();
            var photoInfo = new EarthDateResponse();
            foreach (var dateTime in dates)
            {
                var results = await marsRoverEndpoint.GetPicturesInformationByEarthDay(dateTime);
                photoInfo.Photos.AddRange(results.Photos);

            }
            return Ok(Map(photoInfo));
        }

        private List<DateTime> GetDatesFromFile()
        {
            var dates = new List<DateTime>();
            var file = System.IO.File.ReadLines("dates.txt");
            foreach (var date in file)
            {
                DateTime tempDate;
                if (DateTime.TryParse(date, out tempDate))
                    dates.Add(DateTime.Parse(date));
            }

            return dates;

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id, DateTime earthDay)
        {
            var bytes = await GetImageFromCache(id, earthDay);
            string mimeType = "image/jpeg";
            return new FileContentResult(bytes, mimeType)
            {
                FileDownloadName = $"{id}.jpg"
            };
        }

        private async Task<byte[]> GetImageFromCache(int id, DateTime earthDay)
        {
            byte[] image;
            if (_memoryCache.TryGetValue(id, out image))
                return image;
            var marsRoverEndpoint = new MarsRoverApi(_httpClientFactory);
            var results = await marsRoverEndpoint.GetPicturesInformationByEarthDay(earthDay);
            Map(results);
            _memoryCache.TryGetValue(id, out image);
            return image;
        }

        private IList<PhotoInformation> Map(EarthDateResponse response)
        {
            var returnList = new List<PhotoInformation>();
            foreach(var photo in response.Photos)
            {
                var camera = new CameraInformation
                {
                    Id = photo.Camera.Id,
                    Name = photo.Camera.Name,
                    FullName = photo.Camera.FullName,
                    RoverId = photo.Camera.RoverId
                };
                var rover = new RoverInformation
                {
                    Id = photo.Rover.Id,
                    Name = photo.Rover.Name,
                    LandingDate = photo.Rover.LandingDate,
                    LaunchDate = photo.Rover.LaunchDate,
                    Status = photo.Rover.Status
                };
                var item = new PhotoInformation
                {
                    Id = photo.Id,
                    Sol = photo.Sol,
                    EarthDate = photo.EarthDate,
                    Camera = camera,
                    Rover = rover,
                };
                AddPhotoToCache(photo.Id, photo.ImgSrc);
                returnList.Add(item);
            }
            return returnList;
        }

        private void AddPhotoToCache(int id, string imgSrc)
        {
            var client = new MarsRoverApi(_httpClientFactory);
            var pic = client.GetPicture(imgSrc).Result;
            _memoryCache.Set(id, pic, TimeSpan.FromDays(1));
        }
    }
}
