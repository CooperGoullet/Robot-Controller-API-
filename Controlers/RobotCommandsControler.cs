using Microsoft.AspNetCore.Mvc;
using robot_controller_api;
using System.Collections.Generic;
using System.Linq;
using robot_controller_api.Persistence;

namespace FullImplementaionAPI.Controlers
{
    
    [ApiController]
    [Route("api/robot-commands")]
    public class RobotCommandsController : ControllerBase
    {
        private readonly IRobotCommandDataAccess _robotCommandDataAccess;

        public RobotCommandsController(IRobotCommandDataAccess robotCommandDataAccess)
        {
            _robotCommandDataAccess = robotCommandDataAccess;
        }






        /// <summary>
        /// Retrieves all robot commands.
        /// </summary>
        /// <returns>A list of all robot commands.</returns>
        [HttpGet]
        public IEnumerable<RobotCommand> GetAllRobotCommands()
        {
            return _robotCommandDataAccess.GetRobotCommands();
        }






        /// <summary>
        /// Retrieves only robot commands that involve movement.
        /// </summary>
        /// <returns>A list of move commands if found; otherwise, a NotFound response.</returns>
        [HttpGet("move")]
        public IActionResult GetMoverCommandsOnly()
        {
            var moveCommands = _robotCommandDataAccess
                .GetRobotCommands()
                .Where(cmd => cmd.IsMoveCommand)
                .ToList();

            if (moveCommands.Any())
            {
                return Ok(moveCommands);
            }

            return NotFound("No move commands found.");
        }






        /// <summary>
        /// Retrieves a robot command by its ID.
        /// </summary>
        /// <param name="id">The ID of the robot command.</param>
        /// <returns>The robot command if found; otherwise, a NotFound response.</returns>
        [HttpGet("{id}", Name = "GetRobotCommand")]
        public IActionResult GetRobotCommandById(int id)
        {
            var command = _robotCommandDataAccess.GetRobotCommands()
                .FirstOrDefault(c => c.Id == id);

            if (command == null)
            {
                return NotFound();
            }

            return Ok(command);
        }






        /// <summary>
        /// Adds a new robot command.
        /// </summary>
        /// <param name="newCommand">The robot command to add.</param>
        /// <returns>A Created response with the new robot command, or an error if invalid or duplicate.</returns>
        [HttpPost]
        public IActionResult AddRobotCommand(RobotCommand newCommand)
        {
            if (newCommand == null)
            {
                return BadRequest("Invalid robot command data.");
            }

            var existingCommand = _robotCommandDataAccess.GetRobotCommands()
                .FirstOrDefault(c => c.Name == newCommand.Name);

            if (existingCommand != null)
            {
                return Conflict("A robot command with the same name already exists.");
            }

            _robotCommandDataAccess.InsertRobotCommand(newCommand);

            return CreatedAtRoute("GetRobotCommand", new { id = newCommand.Id }, newCommand);
        }






        /// <summary>
        /// Updates an existing robot command.
        /// </summary>
        /// <param name="id">The ID of the robot command to update.</param>
        /// <param name="updatedCommand">The updated robot command data.</param>
        /// <returns>NoContent if successful; otherwise, appropriate error responses.</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
        {
            if (updatedCommand == null || string.IsNullOrEmpty(updatedCommand.Name))
            {
                return BadRequest("Invalid data provided for update.");
            }

            var existingCommand = _robotCommandDataAccess.GetRobotCommands()
                .FirstOrDefault(c => c.Id == id);

            if (existingCommand == null)
            {
                return NotFound();
            }

            updatedCommand.Id = id; // ensure ID consistency

            _robotCommandDataAccess.UpdateRobotCommand(id, updatedCommand);

            return NoContent();
        }






        /// <summary>
        /// Deletes a robot command by its ID.
        /// </summary>
        /// <param name="id">The ID of the robot command to delete.</param>
        /// <returns>NoContent if successful; otherwise, a NotFound response.</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteRobotCommand(int id)
        {
            var existingCommand = _robotCommandDataAccess.GetRobotCommands()
                .FirstOrDefault(c => c.Id == id);

            if (existingCommand == null)
            {
                return NotFound();
            }

            _robotCommandDataAccess.DeleteRobotCommand(id);

            return NoContent();
        }
    }
}
