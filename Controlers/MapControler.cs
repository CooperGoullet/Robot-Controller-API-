using Microsoft.AspNetCore.Mvc;
using robot_controller_api;
using System.Collections.Generic;
using System.Linq;
using MapController;
using robot_controller_api.Persistence;
using MapController.Persistence;
using System;
using Microsoft.AspNetCore.Authorization;





/// <summary>
/// Controller for handling operations related to Maps.
/// </summary>
namespace FullImplementaionAPI.Controlers
{
    [ApiController]
    [Route("api/maps")]
    public class MapsController : ControllerBase
    {
        private readonly IMapCommandDataAccess _mapDataAccess;

        public MapsController(IMapCommandDataAccess mapDataAccess)
        {
            _mapDataAccess = mapDataAccess;
        }





        /// <summary>
        /// Gets all maps.
        /// </summary>
        /// <returns>A list of all maps.</returns>
        [Authorize(Policy = "UserOnly")]
        [HttpGet]
        public IEnumerable<Map> GetAllMaps()
        {
            return _mapDataAccess.GetMaps();
        }





        /// <summary>
        /// Gets all square maps (where Columns == Rows).
        /// </summary>
        /// <returns>A list of square maps.</returns>
        /// <response code="200">Returns the list of square maps</response>
        /// <response code="404">If no square maps are found</response>
        [HttpGet("square")]
        public ActionResult<IEnumerable<Map>> GetSquareMaps()
        {
            var maps = _mapDataAccess.GetMaps();
            var squareMaps = maps.Where(map => map.Columns == map.Rows).ToList();

            if (squareMaps.Any())
            {
                return Ok(squareMaps);
            }

            return NotFound("No square maps found.");
        }





        /// <summary>
        /// Gets a map by its ID.
        /// </summary>
        /// <param name="id">The ID of the map.</param>
        /// <returns>The map with the given ID.</returns>
        /// <response code="200">Returns the requested map</response>
        /// <response code="404">If the map with the given ID does not exist</response>
        [HttpGet("{id}")]
        public ActionResult<Map> GetMapById(int id)
        {
            var maps = _mapDataAccess.GetMaps();
            var map = maps.FirstOrDefault(m => m.Id == id);

            if (map == null)
            {
                return NotFound($"Map with ID {id} not found.");
            }

            return Ok(map);
        }





        /// <summary>
        /// Creates a new map.
        /// </summary>
        /// <param name="map">The map object to create.</param>
        /// <returns>The created map object.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// POST /api/maps
        /// {
        ///   "name": "Moon Surface",
        ///   "columns": 5,
        ///   "rows": 5
        /// }
        /// </remarks>
        /// <response code="201">Returns the newly created map</response>
        /// <response code="400">If map is invalid or too small</response>
        /// <response code="409">If a map with the same name already exists</response>
        
        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        public ActionResult<Map> PostMap(Map map)
        {
            if (map == null || map.Columns <= 2 || map.Rows <= 2)
            {
                return BadRequest("Map must have Columns and Rows greater than 2.");
            }

            var existingMaps = _mapDataAccess.GetMaps();
            if (existingMaps.Any(m => m.Name == map.Name))
            {
                return Conflict("A map with the same name already exists.");
            }

            map.CreatedDate = DateTime.Now;
            map.ModifiedDate = DateTime.Now;

            _mapDataAccess.InsertMaps(map);

            return CreatedAtAction(nameof(GetMapById), new { id = map.Id }, map);
        }





        /// <summary>
        /// Updates an existing map.
        /// </summary>
        /// <param name="id">The ID of the map to update.</param>
        /// <param name="updatedMap">The updated map object.</param>
        /// <response code="204">If the map was updated successfully</response>
        /// <response code="400">If map data is invalid</response>
        /// <response code="404">If the map with the given ID does not exist</response>
        /// <response code="409">If another map with the same name already exists</response>
        [HttpPut("{id}")]
        public IActionResult UpdateMap(int id, Map updatedMap)
        {
            if (updatedMap.Columns <= 2 || updatedMap.Rows <= 2 || string.IsNullOrWhiteSpace(updatedMap.Name))
            {
                return BadRequest("Invalid map data provided.");
            }

            var allMaps = _mapDataAccess.GetMaps();
            var existingMap = allMaps.FirstOrDefault(m => m.Id == id);

            if (existingMap == null)
                return NotFound();

            if (allMaps.Any(m => m.Name == updatedMap.Name && m.Id != id))
                return Conflict("Another map with the same name already exists.");

            _mapDataAccess.UpdateMap(id, updatedMap);

            return NoContent();
        }





        /// <summary>
        /// Deletes a map by its ID.
        /// </summary>
        /// <param name="id">The ID of the map to delete.</param>
        /// <response code="204">If the map was deleted successfully</response>
        /// <response code="404">If the map with the given ID does not exist</response>
        [HttpDelete("{id}")]
        public IActionResult DeleteMap(int id)
        {
            var allMaps = _mapDataAccess.GetMaps();
            var mapToDelete = allMaps.FirstOrDefault(m => m.Id == id);

            if (mapToDelete == null)
            {
                return NotFound($"Map with ID {id} not found.");
            }

            _mapDataAccess.DeleteMap(id);

            return NoContent();
        }





        /// <summary>
        /// Checks if given coordinates exist within the map boundaries.
        /// </summary>
        /// <param name="id">The ID of the map.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>True if the coordinates are on the map; otherwise false.</returns>
        /// <response code="200">Returns whether the coordinates are on the map</response>
        /// <response code="400">If coordinates are negative</response>
        /// <response code="404">If the map does not exist</response>
        [HttpGet("{id}/{x}-{y}")]
        public IActionResult CheckCoordinate(int id, int x, int y)
        {
            if (x < 0 || y < 0)
                return BadRequest("Coordinates can't be negative.");

            var map = _mapDataAccess.GetMaps().FirstOrDefault(m => m.Id == id);
            if (map == null)
                return NotFound();

            bool isOnMap = x < map.Columns && y < map.Rows;
            return Ok(isOnMap);
        }
    }
}
